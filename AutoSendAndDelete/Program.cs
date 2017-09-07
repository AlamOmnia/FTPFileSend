using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using System.IO;
using System.Timers;

namespace AutoSendAndDelete
{
    class Program
    {
        static Timer timerCreatJobs = new Timer();
        static Timer timerRunJobs = new Timer();
        static Timer timerDeleteJobs = new Timer();
        static void Main(string[] args)
        {

          
            
            try
            {
                timerCreatJobs.Interval = 5000;
                timerRunJobs.Interval = 9000;
                timerDeleteJobs.Interval = 10000;

               timerCreatJobs.Elapsed += ProcessCreateJobs;
               timerRunJobs.Elapsed += ProcessRunJobs;
               timerDeleteJobs.Elapsed += ProcessDeleteFiles;

                timerCreatJobs.Enabled = false;
                timerRunJobs.Enabled = true;
                timerDeleteJobs.Enabled = false;
                // _myTimer.Elapsed += RunJobs;
                //  _myTimer.AutoReset = true;
                //_myTimer.Elapsed += new ElapsedEventHandler(ProcessCreateJobs);


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadLine();
        }

        
        static  void ProcessCreateJobs(Object source, ElapsedEventArgs e)
        {
            timerCreatJobs.Enabled = false;
            try
            {
                Console.WriteLine("Creating Jobs: " + DateTime.Now);
                using (telcobrightmediationEntities context = new telcobrightmediationEntities())
                {
                    new ProcessBackup(GetFtpLocations(), context).CreateBackupJobs();
                }
               
            }
            catch (Exception e1)
            {
                Console.WriteLine(e1);
            }

            timerCreatJobs.Enabled = true;
        }

        static List<FtpBackupLocation> GetFtpLocations()
        {
            return new List<FtpBackupLocation>()
            {
                new FtpBackupLocation("12Machine","192.168.2.12","Administrator","Habib321")
               // new FtpBackupLocation("BtrcCas","192.168.100.137","adnpurple","puricx276#")
            };
        }


        static void ProcessDeleteFiles(Object source, ElapsedEventArgs e)
        {
            timerDeleteJobs.Enabled = false;
            using (telcobrightmediationEntities context = new telcobrightmediationEntities())
            {
                var dir = new DirectoryInfo(@"C:\CDR\Purple Telecom\PurDhkHW\");
                List<FileInfo> allfiles = dir.GetFiles().ToList();
                try
                {
                    Console.WriteLine("Deleting Jobs: " + DateTime.Now);
                    foreach (FileInfo file in allfiles)
                    {
                        List<FtpBackupLocation> locations = GetFtpLocations();
                        List<string> jobNamesForThisFile = new List<string>();
                        foreach (FtpBackupLocation location in locations)
                        {
                            string fileName = file.Name;
                            string jobName = location.Name+"/"+ fileName.Substring(2,7);

                            jobNamesForThisFile.Add(jobName);
                        }
                        bool deleteFlag = true;
                        foreach (string jobName in jobNamesForThisFile)
                        {
                            int status = context.jobs.Where(j => j.jobname == jobName).ToList().Select(c => c.status).First();
                            if (Convert.ToInt32(status) != 1)//if status not complete
                            {
                                deleteFlag = false;
                            }
                            if (deleteFlag == false) break;
                        }
                        if (deleteFlag == true) File.Delete(@"C:\CDR\Purple Telecom\PurDhkHW\" + file);
                    }

                }

                catch (Exception e1)
                {
                    Console.WriteLine(e1);
                }
            }
            timerDeleteJobs.Enabled = true;
        }

        static void ProcessRunJobs(Object source, ElapsedEventArgs e)
        {
            timerRunJobs.Enabled = false;
            try
            {
                Console.WriteLine("Running Jobs: " + DateTime.Now);
                using (telcobrightmediationEntities context = new telcobrightmediationEntities())
                {
                    List<job> unfinishedJobs = context.jobs.Where(c => c.status != 1).ToList();
                    foreach (job j in unfinishedJobs)
                    {
                        FtpBackupLocation location = JsonConvert.DeserializeObject<FtpBackupLocation>(j.parameters);
                        FtpManager ftpManager = new FtpManager
                            (ftpUrl: "ftp://" + location.Server,
                                ftpUser: location.User,
                                ftpPass: location.Password
                            );
                        string fileName = string.Concat("p0" + j.jobname.Split('/')[1] + ".dat");
                        ftpManager.FTPSendSingleFile(@"C:/CDR/Purple Telecom/PurDhkHW/" + fileName, fileName);
                        if (ftpManager.FtpCheckSingleFile(fileName))
                        {
                            //   string Sql = "update telcobrightmediation.job status=1,finishtime={DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'where jobname='{j.jobname}'";
                            context.Database.ExecuteSqlCommand($@"update telcobrightmediation.job 
                                                                   set status=1,finishtime='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'
                                                                    where jobname='{j.jobname}'");


                        }


                    }

                }
            }
            catch (Exception e1)
            {
                Console.WriteLine(e1);
            }
            timerRunJobs.Enabled = true;
        }
        
    }
}


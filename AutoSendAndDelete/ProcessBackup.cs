using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Configuration;


namespace AutoSendAndDelete
{
    public class ProcessBackup
    {
        private List<FtpBackupLocation> Locations { get; }
        private telcobrightmediationEntities Context { get; }
        public ProcessBackup(List<FtpBackupLocation> locations, telcobrightmediationEntities Context)
        {
            this.Locations = locations;
            this.Context = Context;
        }

        public void CreateBackupJobs()
        {
            long lastJobCreatedFileNumber = Convert.ToInt64(Context.lastjobcreateds.Select(c => c.value).First().ToString());
            List<long> unSentFileNumbers = Context.cdrreceiveds.Where(c => c.FileSerialNumber > lastJobCreatedFileNumber)
                .Select(c => c.FileSerialNumber).ToList();
            long lastSerialNumber = 0;
            List<job> jobsToBeCreated = new List<job>();
            foreach (FtpBackupLocation location in this.Locations)
            {
                List<job> newJobsForThisLocation = new List<job>();
                CreateJobsForSingleLocation(location, unSentFileNumbers, out lastSerialNumber, out newJobsForThisLocation);
                jobsToBeCreated.AddRange(newJobsForThisLocation);
            }
            Console.WriteLine("JobCreator: File found:" + jobsToBeCreated.Count);
            string Sql = "insert into job (jobname,status,creationtime,parameters) values "
                + string.Join("," + Environment.NewLine,
                jobsToBeCreated.Select(j => "('" + j.jobname + "',0,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + j.parameters + "') ").ToList());
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["telcobrightSql"].ConnectionString))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("set autocommit=0;", con) { CommandType = System.Data.CommandType.Text })
                {
                    try
                    {
                        cmd.CommandText = Sql;
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "update lastjobcreated set value=" + lastSerialNumber;
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "commit;";
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("JobCreator: Job created:" + jobsToBeCreated.Count);
                    }
                    catch (Exception e)
                    {
                        cmd.CommandText = "rollback;";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            
        }



        void CreateJobsForSingleLocation(FtpBackupLocation location, List<long> unsent,
            out long lastSerialNumber, out List<job> jobsToBeCreated)
        {
            lastSerialNumber = 0;
            jobsToBeCreated = new List<job>();
            List<string> newJobNamesAll = unsent.Select(c => location.Name + "/" + c.ToString()).ToList();
            List<string> existingAmongNewJobNames = Context.jobs.Where(c => c.jobname.StartsWith(location.Name)
                          && newJobNamesAll.Contains(c.jobname)).Select(c => c.jobname).ToList();
            List<string> jobNamesToBeCreated = newJobNamesAll.Except(existingAmongNewJobNames).ToList();
            foreach (string newJobName in jobNamesToBeCreated)
            {
                job newJob = new job()
                {
                    jobname = newJobName,
                    creationtime = DateTime.Now,
                    status = 0,
                    parameters = JsonConvert.SerializeObject(location)
                };
                long newSerialNumber = Convert.ToInt64(newJob.jobname.Split('/')[1]);
                lastSerialNumber = newSerialNumber > lastSerialNumber ? newSerialNumber : lastSerialNumber;
                jobsToBeCreated.Add(newJob);
            }

            Context.SaveChanges();
        }

    }
}

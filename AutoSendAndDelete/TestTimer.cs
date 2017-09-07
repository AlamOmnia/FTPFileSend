using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoSendAndDelete
{
    class TestTimer
    {


        //static void Main(string[] args)
        //{
        //    // Create a Timer object that knows to call our TimerCallback
        //    // method once every 2000 milliseconds.
        //    Timer t = new Timer(CreateJobs, null, 0, 2000);
        //    //Timer t1 = new Timer(RunJobs, null, 0, 3000);
        //    // Wait for the user to hit <Enter>
        //    Console.ReadLine();
        //}



        private static void CreateJobs(Object o)
        {
            Console.WriteLine("Creating Jobs: " + DateTime.Now);
        }


        private static void RunJobs(Object o)
        {
            Console.WriteLine("Running copy jobs: " + DateTime.Now);
        }

    }
}

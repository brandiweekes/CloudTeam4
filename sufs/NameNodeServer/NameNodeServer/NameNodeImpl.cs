using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Amazon.EC2.Model.Internal.MarshallTransformations;
using Microsoft.SqlServer.Server;
using Sufs; //Project -> Add Reference -> Project -> select project

namespace NameNodeServer
{
    class NameNodeImpl : NameNode.NameNodeBase
    {
        List<HealthRecords> recordList = new List<HealthRecords>();

        public static void Main()
        {
            //Stuff
            
            //HB call

            //Stuff
        }

        public bool HeartBeat(string DNid)
        {
            // 1. Save DNid to HealthRecord (Done)
            // 2. set timer (Done)
            //     2.1 start timer (Done)
            // 3. check timer (Done)
            // 3.1 If over timer, HealthCheck(DNid) (Done)

            bool ack = true;
            HealthRecords curHR = new HealthRecords(DNid);
            // check if it exists yet
            if (recordList.Any(x => x.DNid == DNid))
            {
                foreach(HealthRecords hr in recordList)
                {
                    if (hr.DNid == DNid)
                    {
                        curHR = hr;
                    }
                }
            }
            else
            {
                recordList.Add(curHR);
            }
            
            curHR.AlertInt.Start();
            curHR.AlertInt.Elapsed += HealthCheck;

            return ack;
        }

        private static void HealthCheck(Object source, ElapsedEventArgs e)
        {

            Console.WriteLine("Do HealthCheck");
        }
    }

    class HealthRecords : NameNode.NameNodeBase
    {
        public string DNid { get; set; }
        public int BlockId { get; set; }
        public Timer AlertInt = new Timer(3000);
        public bool IsAlive { get; set; }

        public HealthRecords (string DN)
        {
            DNid = DN;
            AlertInt.Interval = 3000;
            IsAlive = true;
        }
    }
}

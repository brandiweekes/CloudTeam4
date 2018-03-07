using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool HeartBeat(string DNid)
        {
            bool ack = true;
            // 1. Save DNid to HealthRecord
            // 2. set timer
            // 3. check timer
            return ack;
        }
    }

    class HealthRecords : NameNode.NameNodeBase
    {
        public string DNid { get; set; }
        public Timer = new Timer(3000);
        public bool isAlive { get; set; }
    }
}

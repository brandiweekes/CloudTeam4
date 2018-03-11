using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Amazon.EC2;
using Amazon.EC2.Model;

namespace AwsGroupProj1
{
    class AWSInstance
    {
        public static void Main(string[] args)
        {
            GetServiceOutput();
            Console.Read();
        }

        public static void GetServiceOutput()
        {
            String amiID = "ami-c1f37db9";
            String keyPairName = "Neha Cloud";
            List<string> groups = new List<string>() { "sg-2b617657" };
            IAmazonEC2 ec2Client = new AmazonEC2Client();

            var launchRequest = new RunInstancesRequest()
            {
                ImageId = amiID,
                InstanceType = "t2.micro",
                MinCount = 1,
                MaxCount = 1,
                KeyName = keyPairName,
                SecurityGroupIds = groups,
            };

            RunInstancesResponse launchResponse = ec2Client.RunInstances(launchRequest);

            List<String> instanceIds = new List<string>();
            foreach (Instance instance in launchResponse.Reservation.Instances)
            {
                Console.WriteLine(instance.InstanceId);
                Console.WriteLine(instance.PrivateIpAddress);
                instanceIds.Add(instance.InstanceId);
            }
        }
    }
}

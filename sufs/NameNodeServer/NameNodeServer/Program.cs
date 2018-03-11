using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Amazon.S3;
using Amazon.S3.Model;

using Grpc.Core;
using Google.Protobuf;
using Sufs;

namespace NameNodeServer
{
    class Program
    {
        const int Port = 50051;

        public static void Main()
        {
            NameNodeImpl nn_test = new NameNodeImpl();
            if(nn_test.mkdir("/foo/bar/"))
            {
                Console.WriteLine("success making path");
            }
            else
            {
                Console.WriteLine("directory already exists");
            }

            if (nn_test.mkdir("/foo/baz/"))
            {
                Console.WriteLine("success making path");
            }
            else
            {
                Console.WriteLine("directory already exists");
            }

            if (nn_test.mkdir("/foo/bar/"))
            {
                Console.WriteLine("success making path");
            }
            else
            {
                Console.WriteLine("directory already exists");
            }


            Server server = new Server
            {
                Services = { NameNode.BindService(new NameNodeImpl()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("NameNode server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();


            
        }
    }
}

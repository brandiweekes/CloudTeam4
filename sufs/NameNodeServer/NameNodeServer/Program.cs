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
            if (nn_test.mkdir("/foo/bar/"))
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

            nn_test.Add_File_To_Namespace_Dir("/foo/bar/", "baz.txt");
            Console.WriteLine("added file baz.txt to /foo/bar/");

            if (nn_test.File_Deleted("/foo/bar/baz.txt"))
            {
                Console.WriteLine("success deleting file");
            }
            else
            {
                Console.WriteLine("file not deleted or file does not exist");
            }
            Console.WriteLine("\n attempting to delete file from above:");
            if (nn_test.File_Deleted("/foo/bar/baz.txt"))
            {
                Console.WriteLine("success deleting file");
            }
            else
            {
                Console.WriteLine("file not deleted or file does not exist");
            }


            nn_test.Add_DNids("172.31.32.209");
            nn_test.Add_DNids("172.31.35.42");
            nn_test.Add_DNids("172.31.35.219");
            nn_test.Add_DNids("172.31.40.19");

            ///*test for function
            // * need to rebuild function to test as below
            // */
            //nn_test.Add_CreateResponse("/foo/bar/", "baz.txt", 2);


            Server server = new Server
            {
                Services = { NameNode.BindService(new NameNodeImpl()) },
                Ports = { new ServerPort("0.0.0.0", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("NameNode server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();



        }
    }
}

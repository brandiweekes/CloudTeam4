using System;
using System.Collections.Generic;
using System.Timers;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using Sufs;
using Grpc.Core;
using Google.Protobuf;

namespace DataNodeServer
{
    class DataNodeServerImpl : DataNode.DataNodeBase
    {
        //For server
        const int Port = 50051;
        string path = @"C:\Users\Administrator\Desktop\";
        List<string> DataNodeIDList = null;

        //For client
        readonly NameNode.NameNodeClient client1;
        readonly DataNode.DataNodeClient client2;
        string NameNodeIPAddress = "54.200.72.18";

        //For server
        public DataNodeServerImpl() {}

        //For client
        public DataNodeServerImpl(NameNode.NameNodeClient client) { this.client1 = client; }
        public DataNodeServerImpl(DataNode.DataNodeClient client) { this.client2 = client; }
       
        public override Task<WriteResponse> WriteFile(IAsyncStreamReader<WriteRequest> requestStream, ServerCallContext context)
        {
            Console.WriteLine("Write..");
            //create a file and write into it
            CreateAndWriteFile(requestStream);

            //send block report
            BlockReport(requestStream.Current.Block.BlockID);

            //replicate data
            DataNodeIDList = requestStream.Current.DataNodeID.ToList();
            if (DataNodeIDList.Count != 0)
            {
                string nextDataNode = DataNodeIDList.First();
                DataNodeIDList.RemoveAt(1);

                Channel channel = new Channel(nextDataNode, 50051, ChannelCredentials.Insecure);
                var client = new DataNode.DataNodeClient(channel);
                using(var reply = client.ReplicateBlock())
                {
                    WriteRequest wr = new WriteRequest
                    {
                        Block = requestStream.Current.Block
                    };
                    while (DataNodeIDList.Count != 0)
                    {
                        wr.DataNodeID.Add(DataNodeIDList.First());
                        DataNodeIDList.RemoveAt(1);
                    }
                    wr.Data = requestStream.Current.Data;
                    wr.Offset = requestStream.Current.Offset;
                    reply.RequestStream.WriteAsync(wr);
                }
                channel.ShutdownAsync();
            }

            return Task.FromResult(new WriteResponse { Response = true });
        }
        public override async Task<WriteResponse> ReplicateBlock(IAsyncStreamReader<WriteRequest> requestStream, ServerCallContext context)
        {
            Console.WriteLine("inside ReplicateBlock");
            List<string> DataNodes = new List<string>();
            //string path = @"C:\Users\brandiweekes\Workspace\CPSC5910\DataNode";
            
            while (await requestStream.MoveNext())
            {
                
                var block = requestStream.Current;
                Console.WriteLine("block.Data.Length {0}", block.Data.Length);
                foreach (var dnid in block.DataNodeID)
                {
                    Console.WriteLine("dnID {0}", dnid);
                    DataNodes.Add(dnid);
                }
                Console.WriteLine("Datanodes list: {0}",DataNodes);
                Console.WriteLine("path + block.Block.BlockID: {0}", path + block.Block.BlockID);
                Console.WriteLine("block.Data.ToByteArray() " + block.Data.ToByteArray());
                File.WriteAllBytes(path + block.Block.BlockID,
                        block.Data.ToByteArray());
            }

            return new WriteResponse { Response = true };

        }
        //public override Task<WriteResponse> ReplicateBlock(IAsyncStreamReader<WriteRequest> requestStream, ServerCallContext context)
    //    {
    //        //create a file and write into it
    //        CreateAndWriteFile(requestStream);



    //        //send block report
    //        BlockReport(requestStream.Current.Block.BlockID);

    //        //replicate data
    //        DataNodeIDList = requestStream.Current.DataNodeID.ToList();
    //        if (DataNodeIDList.Count != 0)
    //        {
    //            string nextDataNode = DataNodeIDList.First();
    //            DataNodeIDList.RemoveAt(1);

    //            Channel channel = new Channel(nextDataNode, 50051, ChannelCredentials.Insecure);
    //            var client = new DataNode.DataNodeClient(channel);
    //}
    //            using (var reply = client.ReplicateBlock())
    //            {
    //                WriteRequest wr = new WriteRequest {
    //                    Block = requestStream.Current.Block
    //                };
    //                while (DataNodeIDList.Count != 0)
    //                {
    //                    wr.DataNodeID.Add(DataNodeIDList.First());
    //                    DataNodeIDList.RemoveAt(1);
    //                }
    //                wr.Data = requestStream.Current.Data;
    //                wr.Offset = requestStream.Current.Offset;
    //                reply.RequestStream.WriteAsync(wr);
    //            }
    //            channel.ShutdownAsync();
    //        }

    //        return Task.FromResult(new WriteResponse{ Response = true });
    //    }

        public override Task ReadFile(ReadBlockRequest request, IServerStreamWriter<ReadBlockResponse> responseStream, ServerCallContext context)
        {
            int blockId = request.BlockID;
            path = path + "\\Block" + blockId + ".txt";

            byte[] data = new byte[1024];
            using (var stream = new FileStream(path, FileMode.Open))
            {
                if(stream.CanRead)
                    stream.Read(data, 0, data.Length);
            }

            ByteString bs = ByteString.CopyFrom(data);
            ReadBlockResponse RBR = new ReadBlockResponse();
            RBR.Data = bs;
            RBR.Offset = 0;

            return responseStream.WriteAsync(RBR);
        }

        public override Task<DeleteResponse> DeleteFile(DeleteRequest request, ServerCallContext context)
        {
            DeleteData(request.BlockID);
            return Task.FromResult(new DeleteResponse { Response = true });
        }

        public override Task<HealthResponse> HealthCheck(HealthRequest request, ServerCallContext context)
        {
            bool instruction = request.Instruction;
            if (instruction == true) //replicate block
            {
                //connect to other datanode
                int blockId = request.Block.BlockID;
                string DataNodeIP = request.DataNodeID;
                Channel channel = new Channel(DataNodeIP, 50051, ChannelCredentials.Insecure);
                var client = new DataNode.DataNodeClient(channel);
                var reply = client.ReadFile(new ReadBlockRequest { BlockID = blockId });

                //read the data
                byte[] data = reply.ResponseStream.Current.Data.ToByteArray();

                //write block
                try
                {
                    path = path + "\\Block" + blockId + ".txt";
                    using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        stream.Write(data, 0, data.Length);
                        stream.Flush();
                    }
                }
                catch (IOException e) { Console.WriteLine("Exception: " + e); }
                catch (ObjectDisposedException e) { Console.WriteLine("Exception: " + e); }
            }
            else if (instruction == false) //delete block
                DeleteData(request.Block.BlockID);
            return Task.FromResult(new HealthResponse { Response = true });
        }

        //fetch IP address of an instance
        public string GetMyIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return null;
        }

        //send heartbeat every three seconds
        public void Heartbeat()
        {
            var myTimer = new Timer();
            myTimer.Elapsed += new ElapsedEventHandler(SendHeartbeatEvent);
            myTimer.Interval = 3000;
            myTimer.Enabled = true;
        }

        private void SendHeartbeatEvent(object source, ElapsedEventArgs e)
        {

            Channel channel = new Channel(NameNodeIPAddress, 50051, ChannelCredentials.Insecure);
            var client = new NameNode.NameNodeClient(channel);
            string myIPAddress = GetMyIPAddress();
            //Console.WriteLine("Sending heartbeart..");
            var reply =  client.Heartbeat(new HBrequest { DNid = myIPAddress });
            //Console.WriteLine("Heartbeat send..");
            channel.ShutdownAsync();
        }

        public void BlockReport(int blockID)
        {
            Channel channel = new Channel(NameNodeIPAddress, 50051, ChannelCredentials.Insecure);
            var client = new NameNode.NameNodeClient(channel);
            string myIPAddress = GetMyIPAddress();
            var reply = client.BlockReportAsync(new ReportRequest { DNid = myIPAddress, BlockID = blockID });
            channel.ShutdownAsync();
        }

        public void CreateAndWriteFile(IAsyncStreamReader<WriteRequest> requestStream)
        {
            //fetch metadata
            int blockId = requestStream.Current.Block.BlockID;
            float blockSize = requestStream.Current.Block.BlockSize;

            try
            {
                //create file and write data to file
                path = path + "\\Block" + blockId + ".txt";

                while (requestStream.MoveNext().IsCompleted)
                {
                    byte[] data = requestStream.Current.Data.ToByteArray();
                    int offset = requestStream.Current.Offset;

                    using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        if (stream.CanWrite)
                        {
                            stream.Write(data, offset, data.Length);
                            stream.Flush();
                        }
                    }
                }
            }
            catch (IOException e) { Console.WriteLine("Exception: " + e); }
            catch (ObjectDisposedException e) { Console.WriteLine("Exception: " + e); }
        }

        public void DeleteData(int blockID)
        {
            path = path + "\\Block" + blockID + ".txt";
            File.Delete(path);
        }

        public static void Main(string[] args)
        {
            DataNodeServerImpl dataNodeServerImpl = new DataNodeServerImpl();
            

            Server server = new Server
            {
                Services = { DataNode.BindService(new DataNodeServerImpl()) },
                Ports = { new ServerPort("0.0.0.0", Port, ServerCredentials.Insecure) }
            };

            server.Start();
            //Console.WriteLine("My IP Address : "+dataNodeServerImpl.GetMyIPAddress());
            dataNodeServerImpl.Heartbeat();
            //dataNodeServerImpl.BlockReport(1);
            Console.WriteLine("DataNode server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}

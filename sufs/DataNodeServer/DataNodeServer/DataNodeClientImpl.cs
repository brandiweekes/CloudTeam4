using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sufs;
using Grpc.Core;

namespace DataNodeServer
{
    class DataNodeClientImpl
    {
        readonly Sufs.NameNode.NameNodeClient client1;
        readonly Sufs.DataNode.DataNodeClient client2;

        public DataNodeClientImpl(sufs.NameNodeClient client)
        {
            this.client1 = client;
        }

        public DataNodeClientImpl(sufs.DataNodeClient client)
        {
            this.client2 = client;
        }

        public Task Heartbeat(string id)
        {
            try
            {
                var startTimeSpan = TimeSpace.Zero;
                var periodTimeSpan = TimeSpan.FromSeconds(3);
                var timer = new System.Threading.Timer((e) =>
                {
                    var reply = client1.Heartbeat(new HBrequest { DNid = id });
                }, null, startTimeSpan, periodTimeSpan);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Rpc Failed" + e);
                throw;
            }
        }

        public Task BlockReport(string id, int blockId, float blockSize)
        {
            try
            {

            }
            catch (RpcException e)
            {
                Console.WriteLine("Rpc Failed" + e);
                throw;
            }
        }

        public Task ReplicateBlock()
        {

        }

        public static void Main(string[] args) 
        {
            Channel channel = new Channel("NameNodeIPAddress", ChannelCredentials.Insecure);
            var client1 = new NameNodeClient(new sufs.NameNodeClient(channel));
            //var client2 = new DataNodeClient(new sufs.DataNodeClient(channel));
            
            client1.Heartbeat("IP Address");
        }
    }
}

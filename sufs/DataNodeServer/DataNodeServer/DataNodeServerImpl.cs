using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Sufs;
using Grpc.Core;

namespace DataNodeServer
{
    class DataNodeServerImpl : DataNode.DataNodeBase
    {
        string path = "C:\\Users\\Neha\\Documents\\Sufs";
        List<string> DataNodeIDList = null;

        public override Task<WriteResponse> WriteFile(IAsyncStreamReader<WriteRequest> requestSream, ServerCallContext context)
        {   
            if(requestSream.Current.Request.Block.BlockID != null) 
            {
                int blockID = requestSream.Current.Request.Block.BlockID;
                float blockSize = requestSream.Current.Request.Block.BlockSize;
                DataNodeIDList = requestSream.Current.Request.DataNodeID;
            }
            else
            {
                byte[] data = requestSream.Current.Chunk.Data;
                long offset = requestSream.Current.Chunk.Offset;
                path = path + "\\Block" + blockID;
                using (var stream = new FileStream(path, FileMode.Append))
                {
                    stream.Write(data, offset, data.Length);
                    stream.Flush;
                }
                if(DataNodeIDList != null)
                    DataNodeIDList.RemoveAt(1);
            }
            return Task.FromResult(new WriteResponse{Response = 1});
        }

        public override Task<WriteResponse> ReplicateBlock(IAsyncStreamReader<WriteRequest> requestSream, ServerCallContext context)
        {
            int blockID = requestSream.Current.Request.Block.BlockID;
            float blockSize = requestSream.Current.Request.Block.BlockSize;
            DataNodeIDList = requestSream.Current.Request.DataNodeID;
            byte[] data = requestSream.Current.Chunk.Data;
            long offset = requestSream.Current.Chunk.Offset;
            path = path + "\\Block" + blockID;
            using (var stream = new FileStream(path, FileMode.Append))
            {
                stream.Write(data, offset, data.Length);
                stream.Flush;
            }
            if(DataNodeIDList != null)
                DataNodeIDList.RemoveAt(1);
            return Task.FromResult(new WriteResponse{Response = 1});
        }

        public override Task<DeleteResponse> DeleteFile(BlockList request, ServerCallContext context)
        {
            foreach(List<Tuple<int, List<string>>> blocks in request.BlockList_)
            {
                //delete file
            }
        }

        public override Task<HealthResponse> HealthCheck(HealthRequest request, ServerCallContext context)
        {
        }
    }
}

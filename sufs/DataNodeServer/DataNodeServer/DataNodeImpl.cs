using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;
using Sufs;

namespace DataNodeImpl
{
    class DataNodeServer : FileHandling.FileHandlingBase
    {
        public override Task<WriteResponse> WriteFile(IAsyncStreamReader<WriteRequest> requestSream, ServerCallContext context)
        {

        }

        public override Task<HealthResponse> HealthCheck(IAsyncStreamReader<HealthRequest> requestSream, ServerCallContext context) 
        {
            //if reponse is 0 then replicate else delete the block
        }

        public override Task<ReplicateResponse> ReplicateBlock(IAsyncStreamReader<ReplicateRequest> requestSream, ServerCallContext context)
        {

        }
    }
}

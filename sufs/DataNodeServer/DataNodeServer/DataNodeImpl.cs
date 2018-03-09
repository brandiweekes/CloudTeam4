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
        public override Task<WriteResponse> WriteFile(WriteRequest request, ServerCallContext context)
        {

        }
    }
}

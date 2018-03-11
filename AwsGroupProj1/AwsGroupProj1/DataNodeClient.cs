using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;

namespace AwsGroupProj1
{
    class DataNodeClient
    {
        Channel channel = new Channel("", ChannelCredentials.Insecure);
         
    }
}

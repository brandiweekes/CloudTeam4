using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsGroupProj1
{
    class Namenode
    {
        NS_File_Info file_info;
        Dictionary<string, NS_File_Info> NN_namespace;




        class NS_File_Info
        {
            public string Filename { get; set; }
            public int Replication_Factor { get; }

            private List<int> block_ids;
            

            public NS_File_Info(string fn)
            {
                Filename = fn;
                Replication_Factor = 3;
                this.block_ids = new List<int>();
            }

            public void Add_BlockIDs (List<int> ids)
            {
                foreach(var i in ids)
                {
                    this.block_ids.Add(i);
                }
            }
            

        }
    }
}

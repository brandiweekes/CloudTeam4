using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;
using Grpc.Core.Utils;

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

            public override bool Equals(object obj)
            {
                var fi = obj as NS_File_Info;
                if (object.ReferenceEquals(fi, null))
                    return false;
                return Filename == fi.Filename && 
                    Replication_Factor == fi.Replication_Factor && 
                    block_ids == fi.block_ids;
            }

            public override int GetHashCode()
            {
                var hc = 0;
                var sum = 0;

                foreach(var b in this.block_ids)
                {
                    sum += b;
                }

                if (Filename != null)
                    hc = Filename.GetHashCode();
                hc = unchecked((hc * 7) ^ Replication_Factor);
                hc = unchecked((hc * 21) ^ sum);
                return hc;
            }

            public override string ToString()
            {
                string bIDs = "";
                foreach(var b in block_ids)
                {
                    bIDs += b.ToString() + " ";
                }

                return string.Format("{{ Filename = {0}, " + 
                                        "Replication = {1}, " + 
                                        "Block IDs = {2} }}",
                                        Filename, Replication_Factor, bIDs);
            }

        }
    }
}

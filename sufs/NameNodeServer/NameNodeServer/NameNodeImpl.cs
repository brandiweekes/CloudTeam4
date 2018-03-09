using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Google.Protobuf;
using Sufs; //Project -> Add Reference -> Project -> select project

namespace NameNodeServer
{
    class NameNodeImpl : NameNode.NameNodeBase
    {
        NS_File_Info file_info;
        Dictionary<string, NS_File_Info> NN_namespace_file;
        Dictionary<string, NS_Dir_Info> NN_namespace_dir;
        Dictionary<string, List<string>> NN_namespace_blockMap;

        //rpc CreateFile (CreateRequest) returns (stream CreateResponse){}
        public override Task CreateFile(CreateRequest request, IServerStreamWriter<CreateResponse> responseStream, ServerCallContext context)
        {
            //will return "Not Implemented"
            return base.CreateFile(request, responseStream, context); //TODO update return
        }



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

            public void Add_BlockIDs(List<int> ids)
            {
                foreach (var i in ids)
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

                foreach (var b in this.block_ids)
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
                foreach (var b in block_ids)
                {
                    bIDs += b.ToString() + " ";
                }

                return string.Format("{{ Filename = {0}, " +
                                        "Replication = {1}, " +
                                        "Block IDs = {2} }}",
                                        Filename, Replication_Factor, bIDs);
            }

        }

        class NS_Dir_Info
        {
            private List<string> subdirectories;
            private List<string> fileNames;


            public NS_Dir_Info()
            {
                this.subdirectories = new List<string>();
                this.fileNames = new List<string>();
            }

            public void Add_SubDirectories(string sd)
            {
               this.subdirectories.Add(sd);
            }

            public void Add_FileNames(string fn)
            {
                this.fileNames.Add(fn);
            }

            public override bool Equals(object obj)
            {
                var di = obj as NS_Dir_Info;
                if (object.ReferenceEquals(di, null))
                    return false;
                return fileNames == di.fileNames &&
                    subdirectories == di.subdirectories;
            }

            public override int GetHashCode()
            {
                var hc = 0;
                var sum = 0;
                var contentCount = fileNames.Count + subdirectories.Count;

                foreach (var f in this.fileNames)
                {
                    sum += f.Length;
                }
                foreach (var s in this.subdirectories)
                {
                    sum += s.Length;
                }

                if (this.fileNames[0] != null)
                    hc = fileNames[0].GetHashCode();
                if (this.subdirectories[0] != null)
                    hc += subdirectories[0].GetHashCode();
                hc = unchecked((hc * 7) ^ contentCount);
                hc = unchecked((hc * 21) ^ sum);
                return hc;
            }

            public override string ToString()
            {
                string files = "";
                string subdirs = "";
                foreach (var f in fileNames)
                {
                    files += f + "\n";
                }
                foreach (var s in subdirectories)
                {
                    subdirs += s + "\n";
                }

                return string.Format("{{ FileNames = {0}, " + 
                                        "SubDirectories = {1} }}",
                                        files, subdirs);
            }

        }
    }
}

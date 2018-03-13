using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Amazon.EC2.Model.Internal.MarshallTransformations;
using Microsoft.SqlServer.Server;
using Grpc.Core;
using Google.Protobuf;
using Sufs; //Project -> Add Reference -> Project -> select project

namespace NameNodeServer
{

    class NameNodeImpl : NameNode.NameNodeBase
    {

        //NS_File_Info file_info;
        //NS_Dir_Info dir_info;
        Dictionary<string, NS_File_Info> NN_namespace_file;
        Dictionary<string, NS_Dir_Info> NN_namespace_dir;
        Dictionary<String, List<int>> FileBlocks; // For client use cases
        Dictionary<int, List<string>> BlockMap;   // For DN use cases
        private List<KeyValuePair<int, string>> missedRepList;
        private List<HealthRecords> recordList; // = new List<HealthRecords>();
        private Timer repCheckTimer;
        public const int repFactor = 3;

        public NameNodeImpl()
        {
            this.NN_namespace_dir = 
                new Dictionary<string, NS_Dir_Info> { ["/"] = new NS_Dir_Info() };

            this.NN_namespace_file =
                new Dictionary<string, NS_File_Info>();

            this.FileBlocks = 
                new Dictionary<string, List<int>>();

            this.BlockMap = 
                new Dictionary<int, List<string>>();

            this.recordList = 
                new List<HealthRecords>();

            this.missedRepList = new List<KeyValuePair<int, string>>();

            this.repCheckTimer = new Timer(60000);
            this.repCheckTimer.Enabled = true;
            this.repCheckTimer.AutoReset = true;
            this.repCheckTimer.Elapsed += repCheck;
        }

        //rpc CreateFile (CreateRequest) returns (stream CreateResponse){}
        public override Task CreateFile(CreateRequest request, IServerStreamWriter<CreateResponse> responseStream, ServerCallContext context)
        {

            //will return "Not Implemented"
            return base.CreateFile(request, responseStream, context); //TODO update return
        }

        /// <summary>
        /// client requests an absolute path to be requested
        /// checks whether path exists, and if doesn't will create
        /// and update directory structure's subdirectories
        /// </summary>
        /// <param name="request">client absolute path</param>
        /// <param name="context">passed from grpc</param>
        /// <returns>F: dir already exists; T: created path</returns>
        public override Task<PathResponse> AddDirectory(PathRequest request, ServerCallContext context)
        {
            //returning an acknowledgement: f=path already exists; t=path created
            return Task.FromResult(new PathResponse { ReqAck = mkdir(request.DirPath) });
        }

        public override Task<ListPathResponse> ListDirectory(PathRequest request, ServerCallContext context)
        {
            ListPathResponse LPR = new ListPathResponse();

            foreach (string dir in NN_namespace_dir.Keys)
            {
                LPR.DirPathContents.Add(dir);
            }

            return Task.FromResult(LPR);
        }
        
        public override Task<PathResponse> DeleteDirectory(PathRequest request, ServerCallContext context)
        {
            PathResponse pr = new PathResponse();
            string target = request.DirPath;

            pr.ReqAck = true;
            return Task.FromResult(pr);
        }

        private void repCheck(Object source, ElapsedEventArgs e)
        {
            missedRepList.Clear();
            foreach (KeyValuePair<int, List<string>> kv in BlockMap)
            {
                if (kv.Value.Count < 3)
                {
                    missedRepList.Add(new KeyValuePair<int, string>(kv.Key, kv.Value[0]));
                }
            }
        }

        public override Task<ReportResponse> BlockReport(ReportRequest request, ServerCallContext context)
        {
            string DNid = request.DNid;
            ReportResponse rr = new ReportResponse();

            foreach (BlockID_Size r in request.BlockList)
            {
                foreach (KeyValuePair<int, List<string>> kv in BlockMap)
                {
                    if (kv.Key == r.BlockID && !kv.Value.Contains(DNid))
                    {
                        kv.Value.Add(DNid);
                    }
                }
            }
            rr.Acknowledged = true;
            return Task.FromResult(rr);
        }

        private void RemoveDeadDN(string DNid)
        {
            //Triggered

            //Go through block mapp
            //delete DNid from blockmap
            foreach (KeyValuePair<int, List<string>> kv in BlockMap)
            {
                if (kv.Value.Contains(DNid))
                {
                    kv.Value.Remove(DNid);
                }
            }
        }

        public override Task<HBresponse> Heartbeat(HBrequest request, ServerCallContext context)
        {
            // 1. Save DNid to HealthRecord (Done)
            // 2. set timer (Done)
            //     2.1 start timer (Done)
            // 3. check timer (Done)
            //     3.1 If over timer, Dead (Done)

            HBresponse hbr = new HBresponse();
            hbr.Acknowledged = true;

            HealthRecords curHR = new HealthRecords(request.DNid);
            // check if it exists yet
            if (recordList.Any(x => x.DNid == request.DNid))
            {
                foreach(HealthRecords hr in recordList)
                {
                    if (hr.DNid == request.DNid && hr.IsAlive == true)
                    {
                        curHR = hr;
                        curHR.AlertTimer.Interval = 5000;
                    }else if (hr.IsAlive == false)
                    {
                        RemoveDeadDN(hr.DNid);
                    }
                }
            }
            else
            {
                recordList.Add(curHR);
            }

            return Task.FromResult(hbr);
        }

        /// <summary>
        /// checks each directory of complete path 
        /// until finds a directory that doesn't exist
        /// foreach of those directories that doesn't exist:
        ///     update directory with subdirectory 
        ///     continue until end of client requested directory
        /// if complete client path requested already exists, exit
        /// </summary>
        /// <param name="cliPath">absolute path requested</param>
        /// <returns>f: path arleady existed; t: made new</returns>
        public bool mkdir(string cliPath)
        {
            if(fullDirPathExists(cliPath))
            {
                //directory already exists
                return false;
            }
            else
            {
                string latestDir = findExistingPath(cliPath);
                string newPath = cliPath.Substring(latestDir.Length);
                string[] forwardSlash = new String[] { "/" };
                string[] newDirs = newPath.Split(forwardSlash, StringSplitOptions.RemoveEmptyEntries);
                foreach (string d in newDirs)
                {
                    if(!this.NN_namespace_dir.ContainsKey(latestDir))
                    {
                        this.NN_namespace_dir.Add(latestDir, new NS_Dir_Info());
                    }
                    this.NN_namespace_dir[latestDir].Add_SubDirectories(d);
                    latestDir += d + "/";
                }
                this.NN_namespace_dir.Add(latestDir, new NS_Dir_Info());
                return true;
            }
        }
        
        /// <summary>
        /// starting at root (/), splices client path request into tokens
        /// checks next directory by each path token 
        /// to see if exists in current directory list of subdirectories
        /// if true: concat to current, 
        /// check that directory subdirectory for next directory
        /// </summary>
        /// <param name="cliPath">entire path client requests to be created</param>
        /// <returns>string of the longest path found in the dictionary</returns>
        private string findExistingPath(string cliPath)
        {
            //root path to start
            string current = "/";

            //tokenize cliPath using "/" as delimiter
            string[] forwardSlash = new String[] { "/" };
            string[] newDirs = cliPath.Split(forwardSlash, StringSplitOptions.RemoveEmptyEntries);

            //index for string array
            int newDirs_i = 0;

            //nextDir will check if the next directory exists in subdirectory
            string nextDir = newDirs[newDirs_i] + "/";

            //update current to have the complete path, 
            //and check if its subdirectory contains the next directory
            while (NN_namespace_dir[current].Find_SubDirectories(nextDir))
            {
                current += nextDir;
                nextDir = newDirs[newDirs_i++] + "/";
            }
         
            return current;           
        }

        /// <summary>
        /// checks complete path requested by client
        /// </summary>
        /// <param name="cliPath"></param>
        /// <returns>t: already exists; f: new dirs found</returns>
        private bool fullDirPathExists(string cliPath)
        {
            return this.NN_namespace_dir.ContainsKey(cliPath);
                //this.NN_namespace_dir[cliPath] != null)
            
        }

        class HealthRecords
        {
            public string DNid { get; set; }
            public int BlockId { get; set; }
            public Timer AlertTimer = new Timer(5000);
            public bool IsAlive { get; set; }

            public HealthRecords (string DN)
            {
                DNid = DN;
                AlertTimer.Enabled = true;
                AlertTimer.Elapsed += Dead;
                IsAlive = true;
            }

            private void Dead(Object source, ElapsedEventArgs e)
            {
                IsAlive = false;
            }
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
            public List<string> subdirectories;
            public List<string> fileNames;


            public NS_Dir_Info()
            {
                this.subdirectories = new List<string>();
                this.fileNames = new List<string>();
            }

            public void Add_SubDirectories(string sd)
            {
               this.subdirectories.Add(sd + "/");
            }

            public bool Find_SubDirectories(string cliDir)
            {
                return this.subdirectories.Contains(cliDir);
            }

            public void Add_FileNames(string fn)
            {
                this.fileNames.Add(fn);
            }

            public bool Find_FileNames(string cliFile)
            {
                return this.fileNames.Contains(cliFile);
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

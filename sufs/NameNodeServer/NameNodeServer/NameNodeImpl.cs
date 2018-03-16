using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Amazon.EC2.Model.Internal.MarshallTransformations;
using Amazon.Runtime.Internal;
using Microsoft.SqlServer.Server;
using Grpc.Core;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Sufs; //Project -> Add Reference -> Project -> select project

namespace NameNodeServer
{

    class NameNodeImpl : NameNode.NameNodeBase
    {
        private const int REPLICATION_FACTOR = 3;
        private const int DN_PORT = 50051;
        
        //NS_File_Info file_info;
        //NS_Dir_Info dir_info;
        Dictionary<string, NS_File_Info> NN_namespace_file; //not used, now FileBlocks
        Dictionary<string, NS_Dir_Info> NN_namespace_dir;
        Dictionary<String, List<int>> FileBlocks; // For client use cases
        Dictionary<int, List<string>> BlockMap;   // For DN use cases
        private List<string> availDNList;
        private List<KeyValuePair<int, string>> missedRepList;
        private List<HealthRecords> recordList; // = new List<HealthRecords>();
        private Timer repCheckTimer;
        private int DN_ID_current;
        private int create_block_id;

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

            this.availDNList = new List<string>();

            this.DN_ID_current = 0;

            this.create_block_id = 1;

            this.recordList = 
                new List<HealthRecords>();

            this.missedRepList = new List<KeyValuePair<int, string>>();

            this.repCheckTimer = new Timer(60000);
            this.repCheckTimer.Enabled = true;
            this.repCheckTimer.AutoReset = true;
            this.repCheckTimer.Elapsed += repCheck;
        }

        /// <summary>
        /// client requests a file created at given directory
        /// plus requests block ids for file 
        /// and which DataNode's to write block
        /// </summary>
        /// <param name="request">
        /// contains: path, filename, number of blocks
        /// </param>
        /// <param name="responseStream">
        /// stream returns to client: 
        /// block id with corresponding DataNode ids
        /// </param>
        /// <param name="context">given by grpc</param>
        /// <returns>response stream block id with DNids</returns>

       
        public override async Task CreateFile(CreateRequest request, IServerStreamWriter<CreateResponse> responseStream, ServerCallContext context)
        {
            Console.WriteLine("client call CreateFile");
            CreateResponse cr = new CreateResponse();
            Console.WriteLine("before update NN_dir");
            //update NN_namespace_dir (directory maps to list of filenames)
            Add_File_To_Namespace_Dir(request.Dir, request.FileName);

            Console.WriteLine("after update NN_dir, before FileBlocks created");
            //update FileBlocks (filename maps to list of blockIDs)
            //FileBlocks.Add(request.FileName, new List<int>());
            FileBlocks[request.FileName] = new List<int>();
            Console.WriteLine("after FileBlocks, before blockID assigned, num request = {0}", request.NumBlocks);

            //var responses = features.FindAll((feature) => feature.Exists() && request.Contains(feature.Location));

            
            //populate FileBlocks: filename map to list of blockIDs &
            //populate CreateResponse with BlockID and list of DNids          
            for (int i = 0; i < request.NumBlocks; i++)
            {
                Console.WriteLine("FileBlocks[{0}].Add({1})", request.FileName, create_block_id);
                FileBlocks[request.FileName].Add(create_block_id);

                var check = Add_CreateResponse();
                Console.WriteLine(check.BlockID);
                Console.WriteLine(check.DNid);
                Console.WriteLine("Before Await");
                await responseStream.WriteAsync(check);

            }
            
            Console.WriteLine("After Await");

        }

        


        /// <summary>
        /// client requests a file deleted at given directory
        /// </summary>
        /// <param name="request">
        /// contains: 
        /// DirPath, path and filename ("/foo/bar/baz.txt")</param>
        /// <param name="context">given from grpc</param>
        /// <returns>
        /// t: success; f: !success || file doesn't exist</returns>
        public override Task<PathResponse> DeleteFile(PathRequest request, ServerCallContext context)
        {
            //element 0 = directory, element 1 = filename;
            string[] dir_fn = Parse_Directory_Filename(request.DirPath);

            //call DN to delete blocks
            foreach (var bID in FileBlocks[dir_fn[1]])
            {     
                foreach(var DNid in BlockMap[bID])
                {
                    //make client
                    Channel channel = new Channel(DNid, DN_PORT, ChannelCredentials.Insecure);
                    var client = new DataNode.DataNodeClient(channel);

                    //rpc call
                    var DeleteResponse = client.DeleteFile(new DeleteRequest { BlockID = bID });
                    channel.ShutdownAsync().Wait();
                }

                

            }

            //update BlockMap to remove the entries of given BlockIDs
            foreach(var bID in FileBlocks[dir_fn[1]])
            {
                BlockMap.Remove(bID);
            }

            //finally, remove file & blockIDs from FileBlocks dict
            FileBlocks.Remove(dir_fn[1]);

            //removes fileName from NN_namespace_dir and sends bool success
            return Task.FromResult(new PathResponse { ReqAck = File_Deleted(request.DirPath) });           
        }
        
        public override Task<ReadResponse> ReadFile(PathRequest request, ServerCallContext context)
        {
            ReadResponse rr = new ReadResponse();
            string cliPath = request.DirPath;
            if (cliPath[0] != '/')
            {
                string temp = "/" + cliPath;
                cliPath = temp;
            }

            string[] forwardSlash = new String[] { "/" };
            string[] dir = cliPath.Split(forwardSlash, StringSplitOptions.RemoveEmptyEntries);
            string target = dir[dir.Length - 1];
            List<int> tempBList = new List<int>();
            List<List<string>> DNList = new List<List<string>>();
            List<string> tmp = new List<string>();

            //foreach (var bid in FileBlocks[target])
            //{
            //    rr.BlockRecord
            //}

            foreach (var bid in FileBlocks[target])
            {
                tempBList.Add(bid);
            }
            foreach (int b in tempBList)
            {
                tmp = BlockMap.Where(d => d.Key.Equals(b))
                    .SelectMany(d => d.Value)
                    .ToList();
                DNList.Add(tmp);
            }
            for (int i = 0; i < tempBList.Count; i++)
            {
                rr.BlockRecord[i].BlockID = tempBList[i];
                for (int j = 0; j < DNList[i].Count; j++)
                {
                    rr.BlockRecord[i].DNread[j] = DNList[i][j];
                }
            }

            return Task.FromResult(rr);
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
        { //TODO: remove debug lines
            Console.WriteLine("inside AddDirectory");

            //returning an acknowledgement: f=path already exists; t=path created
            //return Task.FromResult(new PathResponse { ReqAck = mkdir(request.DirPath) });

            Console.WriteLine("Creating PathResponse");
            PathResponse pr = new PathResponse { ReqAck = mkdir(request.DirPath) };

            Add_File_To_Namespace_Dir(request.DirPath, "cloud.txt");

            FileBlocks["cloud.txt"] = new List<int>();
            Console.WriteLine("after FileBlocks, before blockID assigned, num request = {0}", 3);

            //var responses = features.FindAll((feature) => feature.Exists() && request.Contains(feature.Location));


            //populate FileBlocks: filename map to list of blockIDs &
            //populate CreateResponse with BlockID and list of DNids          
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("FileBlocks[{0}].Add({1})", "cloud.txt", create_block_id);
                FileBlocks["cloud.txt"].Add(create_block_id);

                var check = Add_CreateResponse();
                Console.WriteLine(check.BlockID);
                Console.WriteLine(check.DNid);
            }


            Console.WriteLine("Creating Task");
            Task<PathResponse> tPR = Task.FromResult(pr);
            Console.WriteLine("doing Return");
            return tPR;
        }

        public override Task<ListPathResponse> ListDirectory(PathRequest request, ServerCallContext context)
        {
            ListPathResponse LPR = new ListPathResponse();
            if (NN_namespace_dir.ContainsKey(request.DirPath))
            {
                foreach (var sd in NN_namespace_dir[request.DirPath].subdirectories)
                {
                    LPR.DirPathContents.Add(sd);
                }
                foreach (var fn in NN_namespace_dir[request.DirPath].fileNames)
                {
                    LPR.DirPathContents.Add(fn);
                }
            }
            return Task.FromResult(LPR);
        }
        
        public override Task<PathResponse> DeleteDirectory(PathRequest request, ServerCallContext context)
        {
            PathResponse pr = new PathResponse();
            string target = request.DirPath;

            //Find all subdir that contains clientPath
            foreach (KeyValuePair<string, NS_Dir_Info> dict in NN_namespace_dir)
            {
                NN_namespace_dir.Remove(dict.Key);
            }
            pr.ReqAck = true;
            return Task.FromResult(pr);
        }

        private void repCheck(Object source, ElapsedEventArgs e)
        {
            foreach (KeyValuePair<int, List<string>> kv in BlockMap)
            {
                if (kv.Value.Count < 3)
                {
                    Channel channel = new Channel(kv.Value[0], DN_PORT, ChannelCredentials.Insecure);
                    var client = new DataNode.DataNodeClient(channel);

                    //rpc call
                    BlockDetails bd = new BlockDetails();
                    bd.BlockID = kv.Key;
                    var HealthResponse = client.HealthCheck(new HealthRequest() { Block = bd, DataNodeID = availDNList[DN_ID_current++], Instruction = true});
                    channel.ShutdownAsync().Wait();
                }
            }
        }

        public override Task<ReportResponse> BlockReport(ReportRequest request, ServerCallContext context)
        {
            Console.WriteLine("BlockReport received from {0}", request.DNid);
            string DNid = request.DNid;
            ReportResponse rr = new ReportResponse();

            if (!this.BlockMap.ContainsKey(request.BlockID))
            {
                this.BlockMap.Add(request.BlockID, new List<string>());
                this.BlockMap[request.BlockID].Add(DNid);
                rr.Acknowledged = true;
            }
            else if (this.BlockMap.ContainsKey(request.BlockID) && !this.BlockMap[request.BlockID].Contains(DNid)) 
            {

                this.BlockMap[request.BlockID].Add(DNid);
                Console.Write("   " + this.BlockMap[request.BlockID].Last());
                rr.Acknowledged = true;
            }
            else
            {
                rr.Acknowledged = false;
            }

            //this.BlockMap.Add("172.31.40.19", new List<string>());


            //foreach (KeyValuePair<int, List<string>> kv in BlockMap)
            //{
            //    Console.WriteLine("updating BlockMap for BlockID {0}:", kv.Key);
            //    else if (!this.BlockMap.ContainsKey(request.BlockID))
            //    {
            //        this.BlockMap.Add(request.BlockID, new List<string>());
            //        this.BlockMap[request.BlockID].Add(DNid);
            //    }
            //    Console.WriteLine();
            //}
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

        public override async Task<HBresponse> Heartbeat (HBrequest request, ServerCallContext context)
        {
            // 1. Save DNid to HealthRecord (Done)
            // 2. set timer (Done)
            //     2.1 start timer (Done)
            // 3. check timer (Done)
            //     3.1 If over timer, Dead (Done)
            //Console.WriteLine("heartbeat from {0}", request.DNid);
            HBresponse hbr = new HBresponse();
            hbr.Acknowledged = true;

            HealthRecords curHR = new HealthRecords(request.DNid);
            // check if it exists yet
            if (recordList.Any(x => x.DNid == request.DNid))
            {
                foreach(HealthRecords hr in recordList)
                {
                    //Console.WriteLine("updating HealthRecord for {0}", hr.DNid);
                    if (hr.DNid == request.DNid && hr.IsAlive == true)
                    {
                        curHR = hr;
                        curHR.AlertTimer.Interval = 8000;
                    }else if (hr.IsAlive == false)
                    {
                        RemoveDeadDN(hr.DNid);
                        availDNList.Remove(hr.DNid);
                        //remove from Brandi's new DS
                    }
                }
            }
            else
            {
                recordList.Add(curHR);
                availDNList.Add(curHR.DNid);
                //Add to brandi's new DS
            }

            return await Task.FromResult(hbr);
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
            if(cliPath[0] != '/')
            {
                string temp = "/" + cliPath;
                cliPath = temp;
            }
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

        /// <summary>
        /// populate CreateResponse with BlockID and list of DNids
        /// used for client rpc CreateFile
        /// </summary>
        /// <returns>blockID and list of DNids</returns>
        private CreateResponse Add_CreateResponse()
        {
            //populate CreateResponse with BlockID and list of DNids
            CreateResponse cr = new CreateResponse();

            for (int j = 0; j < REPLICATION_FACTOR; j++)
            {
                if (DN_ID_current >= availDNList.Count)
                {
                    DN_ID_current = 0;
                }
                
                cr.DNid.Add(availDNList[DN_ID_current++]);
            }

            cr.RepFactor = REPLICATION_FACTOR;
            cr.BlockID = create_block_id++;
            
            return cr;
        }

        /// <summary>
        /// adds DNids to namespace list
        /// </summary>
        /// <param name="dn">DataNode id</param>
        public void Add_DNids(string dn)
        {
            this.availDNList.Add(dn);
        }

        /// <summary>
        /// adds file to Namenode Directory 
        /// </summary>
        /// <param name="cd">
        /// client requested path, where to create file</param>
        /// <param name="cf">
        /// client requested file to create</param>
        public void Add_File_To_Namespace_Dir(string cd, string cf)
        {
            Console.WriteLine("inside Add file to namespace dir");
            //make directory if doesn't exist
            bool checkDir = mkdir(cd);

            //check client path to make sure it is correct key
            if (cd[0].Equals('/'))
            {
                NN_namespace_dir[cd].Add_FileNames(cf);
            }
            else
            {
                string correctPath = "/" + cd;
                NN_namespace_dir[correctPath].Add_FileNames(cf);
            }
        }

        /// <summary>
        /// removes file from NameNode Directory
        /// </summary>
        /// <param name="cp">
        /// client requested path, where to delete file from</param>
        /// <returns></returns>
        public bool File_Deleted(string cp)
        {
            //element 0 = directory, element 1 = filename
            string[] dir_fn = Parse_Directory_Filename(cp);

            return(NN_namespace_dir[dir_fn[0]].fileNames.Remove(dir_fn[1]));
        }


        public string[] Parse_Directory_Filename(string cp)
        {
            string directory = "/";
            string fileName;
            string[] dir_fn = new string[2];
            string[] forwardSlash = new String[] { "/" };
            string[] newDirs = cp.Split(forwardSlash, StringSplitOptions.RemoveEmptyEntries);

            //build string for directory path (key) 
            for (int i = 0; i < newDirs.Length - 1; i++)
            {
                directory += newDirs[i] + "/";
            }

            fileName = newDirs[newDirs.Length - 1];

            dir_fn[0] = directory;
            dir_fn[1] = fileName;

            return dir_fn;
        }


        public DataNode.DataNodeClient createChannel(string id, int port)
        {
            Channel channel = new Channel(id, port, ChannelCredentials.Insecure);
            var client = new DataNode.DataNodeClient(channel);
            return client;
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

using Amazon.S3;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Amazon.S3.Model;

namespace SUFS_ClientApplication
{
    public class S3Client
    {
        //string text = System.IO.File.ReadAllText(@"C:\Users\Public\TestFolder\WriteText.txt");
        static string secretKey = System.IO.File.ReadLines(@"Pass").ElementAt(1);
        static string accessKey = System.IO.File.ReadLines(@"Pass").First();

        //string bucketName = "wordcount-kaurj7";
        //long bucketSize = -1;

        AmazonS3Config config1 = new AmazonS3Config();
        //config1.ServiceURL = "objects.dreamhost.com";

        AmazonS3Client s3Client = new AmazonS3Client(
                 accessKey, secretKey
                );
        public long getBucketSize(string bucketName, string keyName)
        {
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = keyName,
                
            };
            long size;
            using (GetObjectResponse response = s3Client.GetObject(request))
            {
                 size = response.ContentLength;
            }
            return size;

        }
        public void retrieveObjectFromS3(string bucketName, string keyName, int startByte, int endByte)
        {
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = keyName,
                ByteRange = new ByteRange(startByte, endByte)
            };

            using (GetObjectResponse response = s3Client.GetObject(request))
            {
                string dest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), keyName);
                if (!File.Exists(dest))
                {
                    response.WriteResponseStreamToFile(dest);
                }
            }


        }
    }
}
using Microsoft.Spark.CSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SparkSchema;
using Microsoft.Spark.CSharp.Common;
using Microsoft.AdCenter.BI.UET.Schema;
using Newtonsoft.Json;
using SerializaType = System.Byte;
using System.IO;
using SparkCommon;

namespace UserAdActivitySummaryPipeline
{
    class Program
    {
        private const int Partitions = 0;
        private static SparkContext sparkContext;
        private static string FileDirectorPath = "hdfs:///user/svcspark/database/zhuzh/uetuserid_searchclicksummary/csv";
        private static string FileSummaryName = "SummaryFileName.txt";
        private static int PartitionSummary = 500;

        private static RDD<string> getRawData(string filename)
        {
            return sparkContext.TextFile(filename, Partitions);
        }
        private static RDD<string> getUserAdActivitySummary(RDD<string> data)
        {
            data = data.Filter(line=> !line.StartsWith("#"));
            var res = data.Map<string>(line => 
            {
                string[] value = line.Split('\t');
                UserAdActivitySummaryRaw usc = new UserAdActivitySummaryRaw();
                usc.UETUserId = new Microsoft.Bond.GUID(Guid.Parse(value[0]));
                usc.UserSearchClickData = Convert.FromBase64String(value[1]).DeserializeBondObject<UserSearchClickSummary>();
                return JsonConvert.SerializeObject(usc);
            });
            return res;
        }
        private static List<string> getSummaryFileName(string filename)
        {
            Inputer FileInPut = new Inputer(filename);
            List<string> fileList = new List<string>();
            string data = "";
            while((data = FileInPut.ReadLine()) != null)
            {
                if (data == "")
                    continue;
                fileList.Add(data);
            }
            return fileList;
        }
        private static List<RDD<string>> getAllSummaryData(string path)
        {
            List<RDD<string>> res = new List<RDD<string>>();
            RDD<string> data = sparkContext.Parallelize(new[] { "#test string" }).Filter(line => !line.StartsWith("#"));
            var container = getSummaryFileName(FileSummaryName);
            int count = 1;
            foreach (var filename in container)
            {
                if (count % PartitionSummary == 0)
                {
                    res.Add(data);
                    data = sparkContext.Parallelize(new[] { "#test string" }).Filter(line => !line.StartsWith("#"));
                }
                var value = getRawData(path +"/" +filename);
                data = data.Union(value);
                count++;
            }
            return res;
        }
        static void Main(string[] args)
        {
            sparkContext = new SparkContext(new SparkConf().SetAppName("UserAdActivitySummaryPipeline"));
            var SummaryData = getAllSummaryData(FileDirectorPath);
            int partion = 1;
            foreach (var data in SummaryData)
            {
                Console.WriteLine(string.Format("---------AllSummaryData {0} count: {1}", partion, data.Count()));
                partion++;
            }
        }
    }
}

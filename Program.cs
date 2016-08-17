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

namespace UserAdActivitySummaryPipeline
{
    class Program
    {
        private const int Partitions = 10000;
        private static SparkContext sparkContext;
        private static string FileDirectorPath = "hdfs:///user/svcspark/database/zhuzh/uetuserid_searchclicksummary/csv";

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
        private static RDD<string> getAllSummaryData(string path)
        {
            DirectoryInfo folder = new DirectoryInfo(path);
            RDD<string> data = sparkContext.Parallelize(new[] {"#test string"});
            foreach (FileInfo file in folder.GetFiles("*.csv"))
            {
                var value = getRawData(file.FullName);
                data = data.Union(value);
            }
            return data;
        }
        static void Main(string[] args)
        {
            sparkContext = new SparkContext(new SparkConf().SetAppName("UserAdActivitySummaryPipeline"));
            var SummaryData = getAllSummaryData(FileDirectorPath);
            Console.WriteLine("---------AllSummaryData count: " + SummaryData.Count());
        }
    }
}

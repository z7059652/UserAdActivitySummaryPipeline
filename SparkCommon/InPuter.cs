using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparkCommon
{
    public class Inputer
    {
        string filename;
        FileStream fs;
        StreamReader sr;
        public Inputer(string file)
        {
            filename = file;
            fs = new FileStream(filename, FileMode.Open);
            sr = new StreamReader(fs);
        }
        public string ReadLine()
        {
            return sr.ReadLine();
        }
        public string ReadToEnd()
        {
            return sr.ReadToEnd();
        }
        public void Close()
        {
            sr.Close();
            fs.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparkCommon
{
    public class Outputer
    {
        string filename;
        FileStream fs;
        StreamWriter sw;
        public Outputer(string file)
        {
            filename = file;
            if (File.Exists(filename))
                File.Delete(filename); 
            fs = new FileStream(filename, FileMode.OpenOrCreate);
            sw = new StreamWriter(fs);
        }
        public void WriteLine(string msg,int repeat = 1)
        {
            //fs.Write(msg,0,len);
            while(repeat-- != 0)
                sw.WriteLine(msg);
        }
        public void Close()
        {
            sw.Close();
            fs.Close();
        }
    }

}

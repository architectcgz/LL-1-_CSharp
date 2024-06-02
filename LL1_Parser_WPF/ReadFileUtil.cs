using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exp2_WPF
{
    public class ReadFileUtil
    {
        public static List<string> readFile(string path)
        {
            return File.ReadAllLines(path).ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UONlife
{
    class GlobalMethod
    {
        // Method: get numbers in a string
        // Source: http://zhidao.baidu.com/question/112902754.html
        public string getNumber(string contact)
        {
            string numStr = null;
            foreach (char item in contact)
            {
                if (item >= 48 && item <= 58)
                {
                     numStr+= item;
                }
            }
            return numStr;
        }

        // input file size (bytes), return file size string
        public static string getFileSizeStr(long size)
        {
            string sizeStr = "";
            if (size < 1024)
                sizeStr = size.ToString() + " B";
            else if (size > 1024 && size < 1024 * 1024)
            {
                double kb = size / 1024;
                sizeStr = kb.ToString() + " KB";
            }
            else
            {
                double mb = size / 1024 / 1024;
                sizeStr = mb.ToString() + " MB";
            }
            return sizeStr;
        }
    }
}

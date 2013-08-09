using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmfData
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, object> srcData = new Dictionary<string, object>();
            srcData["test"] = 123;
            srcData["array"] = new string[] { "a", "b", "c" };

            byte[] buf0 = CAmf0Helper.GetBytes(srcData);
            Console.WriteLine("Amf0: {0}", BitConverter.ToString(buf0));
            object readBack0 = CAmf0Helper.GetObject(buf0);
            Console.WriteLine("decode: {0}", readBack0.ToString());
            Console.WriteLine();

            byte[] buf3 = CAmf3Helper.GetBytes(srcData);
            Console.WriteLine("Amf3: {0}", BitConverter.ToString(buf3));
            object readBack3 = CAmf3Helper.GetObject(buf3);
            Console.WriteLine("decode: {0}", readBack3.ToString());
            Console.WriteLine();
        }
    }
}

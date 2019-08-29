using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    class utils
    {
        public static void Log(byte[] bs)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bs);
            }
            string msg = "[";
            //
            for (int i = 0; i < bs.Length; i += 1)
            {
                msg += " " + bs[i];
            }
            msg += " ]";
            Console.WriteLine(msg);
        }

    }
}

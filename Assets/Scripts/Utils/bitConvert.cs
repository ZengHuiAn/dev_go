using convert = System.BitConverter;

namespace Snake

{
    public static class bitConvert
    {

        public static long DoubleToInt64Bits(double value)
        {
            return convert.DoubleToInt64Bits(value);
        }

        public static byte[] GetBytes(bool value)
        {
            byte[] bs = convert.GetBytes(value);
            return Get(bs);
        }

        public static byte[] Get(byte[] value)
        {
            if (convert.IsLittleEndian)
            {
                System.Array.Reverse(value);
            }
            return value;
        }

        public static byte[] GetBytes(char value)
        {
            byte[] bs = convert.GetBytes(value);

            return Get(bs);
        }

        public static byte[] GetBytes(double value)
        {
            byte[] bs = convert.GetBytes(value);
            return Get(bs);
        }

        public static byte[] GetBytes(short value)
        {
            byte[] bs = convert.GetBytes(value);

            return Get(bs);
        }

        public static byte[] GetBytes(int value)
        {
            byte[] bs = convert.GetBytes(value);

            return Get(bs);
        }

        public static byte[] GetBytes(long value)
        {
            byte[] bs = convert.GetBytes(value);

            return Get(bs);
        }

        public static byte[] GetBytes(float value)
        {
            byte[] bs = convert.GetBytes(value);

            return Get(bs);
        }


        public static byte[] GetBytes(ushort value)
        {
            byte[] bs = convert.GetBytes(value);

            return Get(bs);
        }

        public static byte[] GetBytes(uint value)
        {
            byte[] bs = convert.GetBytes(value);

            return Get(bs);
        }

        public static byte[] GetBytes(ulong value)
        {
            byte[] bs = convert.GetBytes(value);

            return Get(bs);
        }



        public static double Int64BitsToDouble(long value)
        {
            return convert.Int64BitsToDouble(value);
        }

        public static bool ToBoolean(byte[] value, int startIndex)
        {
            value = Get(value);
            return convert.ToBoolean(value, startIndex);
        }

        public static char ToChar(byte[] value, int startIndex)
        {
            value = Get(value);
            return convert.ToChar(value, startIndex);
        }

        public static double ToDouble(byte[] value, int startIndex)
        {
            value = Get(value);
            return convert.ToDouble(value, startIndex);
        }

        public static short ToInt16(byte[] value, int startIndex)
        {
            value = Get(value);
            return convert.ToInt16(value, startIndex);
        }

        public static int ToInt32(byte[] value, int startIndex)
        {
            value = Get(value);
            return convert.ToInt32(value, startIndex);
        }

        public static long ToInt64(byte[] value, int startIndex)
        {
            value = Get(value);
            return convert.ToInt64(value, startIndex);
        }

        public static float ToSingle(byte[] value, int startIndex)
        {
            value = Get(value);
            return convert.ToSingle(value, startIndex);
        }

        public static string ToString(byte[] value)
        {
            value = Get(value);
            return convert.ToString(value);
        }

        public static string ToString(byte[] value, int startIndex)
        {
            value = Get(value);
            return convert.ToString(value, startIndex);
        }

        public static string ToString(byte[] value, int startIndex, int length)
        {
            value = Get(value);
            return convert.ToString(value, startIndex, length);
        }

        public static ushort ToUInt16(byte[] value, int startIndex)
        {
            value = Get(value);
            return convert.ToUInt16(value, startIndex);
        }

        public static uint ToUInt32(byte[] value, int startIndex)
        {
            value = Get(value);
            return convert.ToUInt32(value, startIndex);
        }

        public static ulong ToUInt64(byte[] value, int startIndex)
        {
            value = Get(value);
            return convert.ToUInt64(value, startIndex);
        }

    }

}

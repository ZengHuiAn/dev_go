using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Reflection;
using Debug = Utils.utils;
using System.Threading;

public struct ReferenceLog
{
    public string[] Strings;
    public byte[][] Bytes;
};

enum AMF_TYPE
{
    Invalid,
    Bool,
    Int,
    Uint,
    Float64,
    Array,
    Struct,
    String,
    Default
}

struct MyStruct
{
    public string str;
    public int length;

}

public  class amf
{

    const byte amf_undefine = 0x00;
    const byte amf_null = 0x01;
    const byte amf_false = 0x02;
    const byte amf_true = 0x03;
    const byte amf_integer = 0x04;
    const byte amf_double = 0x05;
    const byte amf_string = 0x06;
    const byte amf_xml_doc = 0x07;
    const byte amf_date = 0x08;
    const byte amf_array = 0x09;
    const byte amf_object = 0x0A;
    const byte amf_xml = 0x0B;
    const byte amf_byte_array = 0x0C;



    static int encodeWithRef(Stream stream, ReferenceLog refLog, object value)
    {
        AMF_TYPE amfType = Utils.utils.GetObjectType(value);
        switch (amfType)
        {
            case AMF_TYPE.Invalid:
                return encodeNull(stream);
            case AMF_TYPE.Bool:
                return encodeBool(stream, Convert.ToBoolean(value));
            case AMF_TYPE.Int:
                return encodeInt(stream, Convert.ToInt64(value));
            case AMF_TYPE.Uint:
                return encodeUint(stream, Convert.ToUInt64(value));
            case AMF_TYPE.Float64:
                return encodeFloat(stream, Convert.ToDouble(value));
            case AMF_TYPE.Array:
                object[] obs = null;
                if (value is ArrayList)
                {
                    obs = ((ArrayList)value).ToArray();
                    throw new Exception("ArrayList Is Not Convert, Recommended byte[]");
                }
                else
                {
                    List<object> list = new List<object>() {  };

                    Array array = (Array)(value);
                    for (int i = 0; i < array.Length; i++)
                    {
                        list.Add(array.GetValue(i));
                    }
                    obs = list.ToArray();
                }
                return encodeArray(stream, obs, refLog);
            case AMF_TYPE.Struct:
                return encodeArrayWithStruct(stream, value, refLog);
            case AMF_TYPE.String:
                return encodeString(stream, Convert.ToString(value), refLog);
            case AMF_TYPE.Default:
                return 0;
        }


        return 0;
    }

    static int encodeFloat(Stream stream, double value)
    {
        writeAmfType(stream, amf_double);
        byte[] data = Snake.bitConvert.GetBytes(value);
        stream.Write(data, 0, data.Length);
        return 9;
    }

    static int encodeArray(Stream stream, object[] value, ReferenceLog refLog)
    {
        if (value.GetType() == typeof(System.Byte[]))
        {
            List<byte> source = new List<byte>();
            for (int i = 0; i < value.Length; i++)
            {
                source.Add((byte)(value[i]));
            }

            //直接encodeByteArray
            return encodeByteArray(stream, source.ToArray(), refLog);
        }


        writeAmfType(stream, amf_array);

        int l = value.Length;

        UInt32 el = Convert.ToUInt32((l << 1) | 1);

        if (el > 0x1FFFFFFF)
        {
            throw new ArgumentOutOfRangeException();
        }

        int nl = encodeU29(stream, el);

        stream.Write(new byte[] { 0x01 }, 0, 1);

        int tl = nl + 2;

        for (int i = 0; i < l; i++)
        {
            int nc = encodeWithRef(stream, refLog, value.GetValue(i));
            tl += nc;
        }

        return tl;
    }

    static int encodeArrayWithStruct(Stream stream, object value, ReferenceLog refLog)
    {

        //写入数组类型
        writeAmfType(stream, amf_array);



        Type tp = value.GetType();

        System.Reflection.FieldInfo[] infos = tp.GetFields();


        int l = infos.Length;


        UInt32 el = Convert.ToUInt32((l << 1) | 1);

        if (el > 0x1FFFFFFF)
        {
            throw new ArgumentOutOfRangeException();
        }

        int nl = encodeU29(stream, el);

        stream.Write(new Byte[] { 0x01 }, 0, 1);

        int tl = nl + 2;

        for (int i = 0; i < l; i += 1)
        {
            //
            int nc = encodeWithRef(stream, refLog, infos[i].GetValue(value));
            tl += nc;
        }
        //
        return tl;
    }


    static int encodeString(Stream stream, string value, ReferenceLog refLog)
    {
        //

        writeAmfType(stream, amf_string);


        bool isRef = false;


        UInt32 l = Convert.ToUInt32(System.Text.Encoding.UTF8.GetBytes(value).Length);

        for (int i = 0; i < refLog.Strings.Length; i++)
        {
            if (refLog.Strings[i] == value)
            {
                l = Convert.ToUInt32(i);
                isRef = true;
                break;
            }
        }

        if (isRef)
        {
            l = l << 1;
        }
        else
        {
            l = (l << 1) | 1;
        }


        if (l > 0x1FFFFFFF)
        {
            throw new ArgumentOutOfRangeException();
        }

        int nl = encodeU29(stream, l);

        int ns = 0;


        if (!isRef)
        {
            byte[] valueBytes = System.Text.Encoding.UTF8.GetBytes(value);

            stream.Write(valueBytes, 0, valueBytes.Length);
            refLog.Strings = new string[] { value };

            List<string> lists = new List<string>() { };
            lists.Add(value);
            lists.AddRange(refLog.Strings);

            refLog.Strings = lists.ToArray();

        }

        return ns + nl + 1;
    }

    static int encodeByteArray(Stream stream, byte[] value, ReferenceLog refLog)
    {

        writeAmfType(stream, amf_byte_array);

        bool isRef = false;

        UInt32 l = Convert.ToUInt32(value.Length);


        if (isRef)
        {
            l = l << 1;
        }
        else
        {
            l = (l << 1) | 1;
        }

        if (l > 0x1FFFFFFF)
        {
            throw new ArgumentOutOfRangeException();
        }

        int nl = encodeU29(stream, l);


        int ns = 0;

        if (!isRef)
        {
            stream.Write(value, 0, value.Length);
            ns = value.Length;
        }


        return ns + nl + 1;
    }


    static int encodeNull(Stream stream)
    {
        return writeAmfType(stream, amf_null);
    }

    static int encodeBool(Stream stream, bool value)
    {

        if (value)
        {
            return writeAmfType(stream, amf_true);
        }
        else
        {
            return writeAmfType(stream, amf_false);
        }
    }
    static int encodeInt(Stream stream, Int64 value)
    {
        if (value > 0xFFFFFFF || value < -0xFFFFFFF)
        {
            return encodeFloat(stream, (double)(value));
        }

        writeAmfType(stream, amf_integer);

        uint u = s2u(Convert.ToInt32(value));
        int n = encodeU29(stream, u);
        return n + 1;
    }

    static int encodeUint(Stream stream, UInt64 value)
    {
        Debug.Log(value);
        //
        if (value > 0xFFFFFFF)
        {
            return encodeFloat(stream, (double)(value));
        }

        writeAmfType(stream, amf_integer);
        int n = encodeU29(stream, Convert.ToUInt32(value));
        return n + 1;

    }

    static int encodeU29(Stream w, UInt32 value)
    {
        //
        byte[] bs;
        if (value <= 0x7F)
        {
            bs = new byte[] {
                Convert.ToByte(value & 0x7f)
            };
            w.Write(bs, 0, bs.Length);
            return bs.Length;
        }
        else if (value <= 0x00003FFF)
        {
            bs = new byte[] {

                Convert.ToByte(Convert.ToByte(value >> 7) | 0x80),
                Convert.ToByte(value & 0x7f)
            };
            w.Write(bs, 0, bs.Length);
            return bs.Length;
        }
        else if (value <= 0x001FFFFF)
        {
            bs = new byte[] {
                Convert.ToByte(Convert.ToByte(value >> 14) | 0x80),
                Convert.ToByte( Convert.ToByte((value >> 7)& 0x7F ) |0x80),
                Convert.ToByte(value & 0x7F),
            };
            w.Write(bs, 0, bs.Length);
            return bs.Length;
        }
        else if (value <= 0x1FFFFFFF)
        {

            bs = new byte[] {
                Convert.ToByte(Convert.ToByte(value >> 22) | 0x80),
                Convert.ToByte(Convert.ToByte((value >> 15)& 0x7F ) |0x80),
                Convert.ToByte(Convert.ToByte((value >> 8) & 0x7F) | 0x80),
                Convert.ToByte (value & 0xFF),
            };

            w.Write(bs, 0, bs.Length);
            return bs.Length;

        }
        else
        {
            return 0;
        }
    }

    static UInt32 s2u(Int32 i)
    {
        if (i > 0xFFFFFFF || i < -0xfffffff)
        {
            throw new ArgumentOutOfRangeException();
        }
        else if (i >= 0)
        {
            return Convert.ToUInt32(i);
        }
        else
        {
            return (0x10000000 | Convert.ToUInt32(-i));
        }
    }


    static int writeAmfType(Stream stream, byte b)
    {
        byte[] bytes = new byte[] { b };
        stream?.Write(bytes, 0, bytes.Length);
        return bytes.Length;
    }



    static public int Encode(Stream writer, object v)
    {
        ReferenceLog referenceLog = new ReferenceLog() { Strings = new string[] { }, Bytes = new byte[][] { } };
        Utils.utils.Log("encode:--->>",v);
        return encodeWithRef(writer, referenceLog, v);
    }


    static public object Decode(Stream reader)
    {
        reader.Position = 0;
        ReferenceLog referenceLog = new ReferenceLog() { Strings = new string[] { }, Bytes = new byte[][] { } };
        return decodeWithRef(reader, referenceLog);
    }

    static private object decodeWithRef(Stream stream, ReferenceLog refLog)
    {
        byte[] bs = new byte[1];
        int len = stream.Read(bs, 0, bs.Length);

        switch (bs[0])
        {
            case amf_undefine:
                return null;
            case amf_null:
                return null;
            case amf_false:
                return false;
            case amf_true:
                return true;
            case amf_integer:
                return decodeInteger(stream);
            case amf_double:
                return decodeFloat(stream);
            case amf_string:
                return decodeString(stream, refLog);
            case amf_byte_array:
                return decodeByteArray(stream, refLog);
            case amf_array:
                return decodeArray(stream,refLog);
            default:
                break;
        }

        return null;
    }

    static object decodeArray(Stream stream, ReferenceLog refLog)
    {
        UInt32 l = decodeU29(stream);

        l = l >> 1;

        byte[] bs = new byte[1];

        stream.Read(bs, 0,bs.Length);

        object[] arrayList = new object[l];


        for (int i = 0; i < l; i++)
        {
            arrayList[i] = decodeWithRef(stream, refLog);
        }

        return arrayList;

    }

    static byte[] decodeByteArray(Stream stream, ReferenceLog refLog)
    {
        UInt32 l = decodeU29(stream);

        bool isRef = (l & 1) == 0;

        l = l >> 1;

        if (isRef)
        {
            if (refLog.Bytes.Length >0)
            {
                return refLog.Bytes[l];
            }
            else
            {
                throw new Exception("error code byte array");
            }
        }
        else
        {
            byte[] bs = new byte[l];
            stream.Read(bs, 0, bs.Length);
            List<byte[]> lists = new List<byte[]>() { };

            lists.AddRange(refLog.Bytes);
            lists.Add(bs);
            refLog.Bytes = lists.ToArray();
            return bs;
        }
    }

    static double decodeFloat(Stream stream)
    {
        byte[] bs = new byte[8];
        stream.Read(bs, 0, bs.Length);
        return Snake.bitConvert.ToDouble(bs, 0);
    }

    static string decodeString(Stream stream, ReferenceLog refLog)
    {
        //
        UInt32 l = decodeU29(stream);

        bool isRef = (l & 1) == 0;

        l = l >> 1;

        if (isRef)
        {
            if (refLog.Strings.Length > 0)
            {
                return refLog.Strings[l];
            }
            else
            {
                throw new Exception("error code string");
            }
        }
        else
        {
            byte[] bs = new byte[l];
            stream.Read(bs, 0, bs.Length);
            string toString = System.Text.Encoding.UTF8.GetString(bs);
            List<string> lists = new List<string>() { };

            lists.AddRange(refLog.Strings);
            lists.Add(toString);
            refLog.Strings = lists.ToArray();
            return toString;
        }
    }

    static long decodeInteger(Stream stream)
    {
        UInt32 value = decodeU29(stream);
        return (long)(u2s(value));
    }
    static Int32 u2s(UInt32 u)
    {
        if (u > 0x1FFFFFFF)
        {
            throw new ArgumentOutOfRangeException();
        }

        if ((u & 0x10000000) == 0)
        {
            return (Int32)(u);
        }
        else
        {
            return -(Int32)(u & 0xFFFFFFF);
        }
    }

    static UInt32 decodeU29(Stream stream)
    {
        UInt32 value = 0;

        byte[] bs = new byte[1];

        for (int i = 0; i < 4; i++)
        {
            stream.Read(bs, 0, bs.Length);


            byte c = bs[0];

            if (i != 3)
            {

                value |= (UInt32)(c & 0x7F);

                if ((c & 0x80) != 0)
                {
                    if (i != 2)
                    {
                        value <<= 7;
                    }
                    else
                    {
                        value <<= 8;
                    }
                }
                else
                {
                    break;
                }
            }
            else
            {
                value |= (UInt32)(c);
                break;
            }

        }

        return value;
        //
    }

}

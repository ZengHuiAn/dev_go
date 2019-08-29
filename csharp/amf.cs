using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;


public struct ReferenceLog
{
    string[] Strings;
    byte[][] Bytes;
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
    int length;
}

public class amf 
{

    byte amf_undefine = 0x00;
    byte amf_null = 0x01;
    byte amf_false = 0x02;
    byte amf_true = 0x03;
    byte amf_integer = 0x04;
    byte amf_double = 0x05;
    byte amf_string = 0x06;
    byte amf_xml_doc = 0x07;
    byte amf_date = 0x08;
    byte amf_array = 0x09;
    byte amf_object = 0x0A;
    byte amf_xml = 0x0B;
    byte amf_byte_array = 0x0C;
    public void Start()
    {
        using (MemoryStream stream = new MemoryStream())
        {
            //encodeInt(stream,5);

            ReferenceLog reflog = new ReferenceLog();
            encodeArray(stream, new int[] { 1, 2, 3 }, reflog);

            byte[] datas = stream.ToArray();
            Array.Reverse(datas);
            Log(datas);
            //Console.WriteLine(this.Encode(stream, null));
            //Console.WriteLine(stream.ToArray().Length);
        }

    }

    void Log(byte[] bs)
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

    public int Encode(Stream stream, object v)
    {
        ReferenceLog reflog = new ReferenceLog();
        return this.encodeWithRef(stream, reflog, v);
    }
    int encodeWithRef(Stream stream, ReferenceLog refLog, object value)
    {
        AMF_TYPE amfType = GetObjectType(value);
        Console.WriteLine(amfType);
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
                return encodeArray(stream,  (Array)value, refLog);
            case AMF_TYPE.Struct:
                break;
            case AMF_TYPE.String:
                break;
            case AMF_TYPE.Default:
                break;
        }


        return 0;
    }

    int encodeFloat(Stream stream, double value)
    {
        writeAmfType(stream, amf_double);
        byte[] data = Snake.bitConvert.GetBytes(value);
        stream.Write(data);
        return 9;
    }

    int encodeArray(Stream stream, Array value, ReferenceLog refLog)
    {
        if (value.GetType() == typeof(System.Byte[]))
        {
            //直接encodeByteArray
            return 0;
        }

        writeAmfType(stream, amf_array);

        int l = value.Length;

        UInt32 el = (UInt32)((l << 1) | 1);

        if (el > 0x1FFFFFFF)
        {
            throw new ArgumentOutOfRangeException();
        }

        int nl = encodeU29(stream, el);

        stream.Write(new byte[] { 0x01 });

        int tl = nl + 2;

        for (int i = 0; i < l; i++)
        {
            int nc = encodeWithRef(stream, refLog,value.GetValue(i));
            tl += nc;
        }

        return tl;
    }

    int encodeArrayWithStruct(Stream stream,  value, ReferenceLog refLog)
    {
        //
    }


    int encodeNull(Stream stream)
    {
        return writeAmfType(stream, amf_null);
    }

    int encodeBool(Stream stream, bool value)
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
    int encodeInt(Stream stream, Int64 value)
    {
        if (value > 0xFFFFFFF || value < -0xFFFFFFF)
        {
            return encodeFloat(stream, (double)(value));
        }

        writeAmfType(stream, amf_integer);

        uint u = s2u((Int32)value);
        int n = encodeU29(stream, u);
        return n + 1;
    }

    int encodeUint(Stream stream, UInt64 value)
    {
        //
        if (value > 0xFFFFFFF)
        {
            return encodeFloat(stream, (double)(value));
        }

        writeAmfType(stream, amf_integer);
        int n = encodeU29(stream, (UInt32)(value));
        return n + 1;

    }

    int encodeU29(Stream w, UInt32 value)
    {
        //
        byte[] bs;
        if (value <= 0x7F)
        {
            bs = new byte[] {
                (byte)(value & 0x7f)
            };

            w.Write(bs);
            return bs.Length;
        }
        else if (value <= 0x00003FFF)
        {
            bs = new byte[] {
                (byte)((byte)(value >> 7) | 0x80),
                (byte)(value & 0x7f)
            };
            w.Write(bs);
            return bs.Length;
        }
        else if (value <= 0x001FFFFF)
        {
            bs = new byte[] {
                (byte)((byte)(value >> 14) | 0x80),
               (byte)( (byte)((value >> 7)& 0x7F ) |0x80),
                (byte)(value & 0x7F),
            };
            w.Write(bs);
            return bs.Length;
        }
        else if (value <= 0x1FFFFFFF)
        {

            bs = new byte[] {
                (byte)((byte)(value >> 22) | 0x80),
                (byte)((byte)((value >> 15)& 0x7F ) |0x80),
                 (byte)( (byte)((value >> 8) & 0x7F) | 0x80),
                 (byte) (value & 0xFF),
            };
            w.Write(bs);
            return bs.Length;

        }
        else
        {
            return 0;
        }
    }

    UInt32 s2u(Int32 i)
    {
        if (i > 0xFFFFFFF || i < -0xfffffff)
        {
            throw new ArgumentOutOfRangeException();
        }
        else if (i >= 0)
        {
            return (UInt32)i;
        }
        else
        {
            return (0x10000000 | (UInt32)(-i));
        }
    }


    int writeAmfType(Stream stream, byte b)
    {
        byte[] bytes = new byte[] { b };
        stream?.Write(bytes);
        return bytes.Length;
    }
    AMF_TYPE GetObjectType(object value)
    {
        if (value == null)
        {
            return AMF_TYPE.Invalid;
        }

        System.Type types = value.GetType();
        if (types.IsValueType && !types.IsEnum && !types.IsPrimitive)
        {
            Console.WriteLine("结构体:" + value);
        }
        else
        {
            Console.WriteLine("不是结构体:" + value);
        }

        if (types == typeof(bool))
        {
            return AMF_TYPE.Bool;
        }
        else if (types == typeof(int) || types == typeof(Int16) || types == typeof(Int32) || types == typeof(Int64) || types == typeof(long))
        {
            return AMF_TYPE.Int;
        }
        else if (types == typeof(uint) || types == typeof(ulong) || types == typeof(UInt16) || types == typeof(UInt32) || types == typeof(UInt64))
        {
            return AMF_TYPE.Uint;
        }
        else if (types == typeof(float) || types == typeof(double))
        {
            return AMF_TYPE.Float64;
        }
        else if (types == typeof(Array) || types == typeof(ArrayList))
        {
            return AMF_TYPE.Array;
        }
        else if (types.IsValueType && !types.IsEnum && !types.IsPrimitive)
        {
            return AMF_TYPE.Struct;
        }

        else if (types == typeof(string))
        {
            return AMF_TYPE.String;
        }
        return AMF_TYPE.Default;
    }


}

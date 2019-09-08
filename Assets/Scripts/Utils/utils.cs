using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public class utils
    {

        static void LogFunction(object value)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(value);
#else
            Console.WriteLine(value);
#endif
        }

        static string offest = "    ";
        public static void Log(params object[] obs)
        {

            string msg = "";
            string fileter = "";
            for (int i = 0; i < obs.Length; i++)
            {
                msg += LogObject(obs[i], fileter) + "\n";
            }

            LogFunction(msg);
        }

        public static string LogObject(object value, string filter)
        {
            string msg = filter;
            AMF_TYPE tYPE = GetObjectType(value);
            switch (tYPE)
            {
                case AMF_TYPE.Invalid:
                    break;
                case AMF_TYPE.Bool:
                case AMF_TYPE.Int:
                case AMF_TYPE.Uint:
                case AMF_TYPE.Float64:
                    msg += value;
                    break;
                case AMF_TYPE.Array:
                    msg += LogArray((Array)value, filter);
                    break;
                case AMF_TYPE.Struct:
                    return LogArray(ConvertStruct(value),filter);
                case AMF_TYPE.String:
                    msg += value;
                    break;
                case AMF_TYPE.Default:
                    msg += value;
                    break;
                default:
                    break;
            }
            return msg;
        }

        public static string LogArray(Array value, string filter)
        {
            string msg = ((filter == "" ? "\n" : "") + filter + " [\n");
            //
            for (int i = 0; i < value.Length; i += 1)
            {
                msg += (filter + utils.offest + "index:" + i + offest + LogObject(value.GetValue(i), filter + offest) + "\n");
            }
            msg += (filter + " ]");
            return msg;
        }

        public static object[] ConvertStruct(object value)
        {

            Type tp = value.GetType();
            
            System.Reflection.FieldInfo[] infos = tp.GetFields();
            object[] obs = new object[infos.Length];
            for (int i = 0; i < infos.Length; i += 1)
            {
                //
                obs[i] = infos[i].GetValue(value);
                
            }
            return obs;
        }


        public static AMF_TYPE GetObjectType(object value)
        {
            if (value == null)
            {
                return AMF_TYPE.Invalid;
            }

            System.Type types = value.GetType();
            if (types.IsValueType && !types.IsEnum && !types.IsPrimitive)
            {
                
            }
            else
            {
                
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
            else if (value is Array || value is ArrayList)
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
}
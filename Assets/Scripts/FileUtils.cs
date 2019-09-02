using System;
using UnityEngine;
using System.Text;
using System.IO;
using XLua;

namespace SGK
{
    [LuaCallCSharp]
    public static class FileUtils
    {
        public static string LuaDirectory = "ugui";
        public static string APPArea = "cn";

        public static byte[] utf8FiliterRom(byte[] bts)
        {
            if (bts == null || bts.Length == 0)
            {
                return bts;
            }
            if (bts[0] == 239 && bts[1] == 187 && bts[2] == 191)
            {
                byte[] n = new byte[bts.Length - 3];
                for (int i = 3; i < bts.Length; ++i)
                {
                    n[i - 3] = bts[i];
                }
                return n;
            }
            return bts;
        }

        public static byte[] readFromAssets(string filePath)
        {
            filePath = Application.dataPath + "/" + filePath;
            if (File.Exists(filePath))
            {
                return utf8FiliterRom(File.ReadAllBytes(filePath));
            }
            return null;
        }

        public static string readStringFromAssets(string filePath)
        {
            filePath = Application.dataPath + "/" + filePath;

            Debug.Log("filePath:"+ filePath);
            if (File.Exists(filePath))
            {
                return System.Text.Encoding.UTF8.GetString(utf8FiliterRom(File.ReadAllBytes(filePath)));
            }

            Debug.Log("不存在文件 filePath:" + filePath);
            return null;
        }

        public static byte[] Load(ref string fileName)
        {
#if UNITY_EDITOR
            string realFileName = LuaDirectory + "/" + fileName.Replace(".", "/") + ".lua";
            byte[] bs = readFromAssets(realFileName);
            if (bs != null)
            {
                return bs;
            }
#endif
            //fileName = fileName.Replace('.', '/') + ".lua";
            //TextAsset text = AssetManager.Load<TextAsset>(LuaDirectory, fileName + ".bytes");
            //if (text != null)
            //{
            //    fileName = LuaDirectory + "/" + fileName;
            //    return text.bytes;
            //}
            return null;
        }

        public static string LoadStringFromFile(string fileName)
        {
#if UNITY_EDITOR
            string realFileName = LuaDirectory + "/" + fileName.Replace(".", "/") + ".lua";
            string str = readStringFromAssets(realFileName);
            Debug.Log("读取文件内容\t"+str);
            if (str != null)
            {
                return str;
            }
#endif
            //TextAsset text = AssetManager.Load<TextAsset>(LuaDirectory, fileName + ".bytes");
            //if (text == null || text.text.Equals(""))
            //{
            //    Debug.LogFormat("LoadStringFromFile {0} failed", fileName);
            //    return "";
            //}
            //return System.Text.Encoding.UTF8.GetString(utf8FiliterRom(text.bytes));

            return "";
        }
    }

}


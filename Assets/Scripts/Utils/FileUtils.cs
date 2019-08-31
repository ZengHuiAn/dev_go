using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snake
{
    public static class FileUtils
    {

        public static string ReadFile(string path, string fileName, string suffix = ".txt")
        {
            string content = System.IO.File.ReadAllText(path + fileName + suffix);

            return content;
        }

        public static string envPath
        {
            get
            {
                string filepath = "";
#if UNITY_EDITOR
                filepath = UnityEngine.Application.dataPath + "/Lua/";

#elif UNITY_IPHONE
	        filepath = Application.dataPath +"/Raw/Lua/";
 
#elif UNITY_ANDROID
	        filepath = "jar:file://" + Application.dataPath + "!/assets/Lua/";
 
#endif
                return filepath;
            }
        }

        public static byte[] LuaLoader(ref string fileName)
        {
            string convertName = fileName.Replace('.', '/');
            string filepath;
#if UNITY_EDITOR
            filepath = UnityEngine.Application.dataPath + "/Lua/";

#elif UNITY_IPHONE
	        filepath = Application.dataPath +"/Raw/Lua/";
 
#elif UNITY_ANDROID
	        filepath = "jar:file://" + Application.dataPath + "!/assets/Lua/";
 
#endif
            string content = FileUtils.ReadFile(filepath, convertName, ".lua");

            return System.Text.Encoding.UTF8.GetBytes(content);
        }

        public static string ReadFileContent(string fileName)
        {
            string convertName = fileName.Replace('.', '/');
            string filepath;
#if UNITY_EDITOR
            filepath = UnityEngine.Application.dataPath + "/Lua/";

#elif UNITY_IPHONE
	        filepath = Application.dataPath +"/Raw/Lua/";
 
#elif UNITY_ANDROID
	        filepath = "jar:file://" + Application.dataPath + "!/assets/Lua/";
 
#endif
            string content = FileUtils.ReadFile(filepath, convertName, ".lua");
            return content;
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System;

namespace Snake
{
    public class OutExeEditor
    {
        [MenuItem("CheckMenu/CheckMeta And File")]
        public static void BuildFromUnityMenu()
        {
            UnityEngine.Debug.Log("build");
            var outputDir = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
            var packageName = "checkFile.exe";
            Process pro = StartProcess(outputDir+packageName, "-path ./Assets/");
            pro.Start();
            string fingerprint = pro.StandardOutput.ReadToEnd();
            pro.WaitForExit();
            pro.Close();
            UnityEngine.Debug.Log(fingerprint);
            //System.Diagnostics.Process.Start("notepad.exe");
        }

        public static Process StartProcess(string fileName, string args)
        {
            try
            {
                fileName = "\"" + fileName + "\"";
                Process myProcess = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo(fileName, args);
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardInput = true;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                myProcess.StartInfo = startInfo;
                return myProcess;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log("出错原因：" + ex.Message);
            }
            return null;
        }
    }
}
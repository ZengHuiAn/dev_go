using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;




[CreateAssetMenu(menuName="Create ScriptableObject ")]
public class AssetBundleSerialization : ScriptableObject
{
    [System.Serializable]
    public struct AssetBundleInfo
    {
        public string FileName;
        public bool isChange;
        public string GUID;
    }
    public List<AssetBundleInfo>  abInfos = new List<AssetBundleInfo>();
    private static AssetBundleSerialization _instance;
    public static AssetBundleSerialization Instance
    {
        get
        {

#if UNITY_EDITOR
            if (!_instance)
            {
                var abSerial = CreateInstance<AssetBundleSerialization>();   
                UnityEditor.AssetDatabase.CreateAsset(abSerial,"Assets/AssetBundle.asset");
            }
            else
            {
                UnityEditor.AssetDatabase.LoadAssetAtPath<AssetBundleSerialization>("Assets/AssetBundle.asset");
            }
#endif
            return _instance;
        }
    }

}

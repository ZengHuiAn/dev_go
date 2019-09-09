using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class AssetGUID
{
    [MenuItem("Assets/ShowGUID")]
    public static void ShowGUID()
    {
        Utils.utils.Log(Selection.assetGUIDs);
    }
}

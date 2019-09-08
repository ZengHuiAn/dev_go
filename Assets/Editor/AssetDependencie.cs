using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetDependencie
{
    static AssetDependencie instance;

    public static AssetDependencie Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AssetDependencie();
            }
            return instance;
        }
    }


    public void GetDependenciesForObject(GenerateTreeview<SaveInfo> m_SimpleTreeView, UnityEngine.Object assetOb)
    {
        var assetPath = AssetDatabase.GetAssetPath(assetOb);
        if (assetPath.Equals(""))
        {
            throw new System.Exception(assetOb.name + "\t不是可获取依赖资源");
        }
        var guid = AssetDatabase.AssetPathToGUID(assetPath);
        string[] depens = AssetDatabase.GetDependencies(assetPath);
        Utils.utils.Log(depens);
        SaveInfo si = new SaveInfo(assetOb.name, assetPath, assetOb, assetOb.GetType());
        var node = m_SimpleTreeView.AddForChild(m_SimpleTreeView.root, si.GetDisplayName(), si);
        for (int i = 0; i < depens.Length; i++)
        {
            if (!depens[i].Equals(si.path))
            {
                var tempDepen = AssetDatabase.GetDependencies(depens[i]);
                var depenAsset = AssetDatabase.LoadAssetAtPath(depens[i], typeof(UnityEngine.Object));

                Debug.Log(depenAsset.GetType());
                SaveInfo tempNodeInfo = new SaveInfo(depenAsset.name, depens[i], depenAsset, depenAsset.GetType());
                m_SimpleTreeView.AddForChild(node, tempNodeInfo.GetDisplayName(), tempNodeInfo);
            }
        }
    }
}

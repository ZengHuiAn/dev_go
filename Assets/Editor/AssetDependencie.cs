using System.Collections;
using System.Collections.Generic;
using System.IO;
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


    public void GetDependenciesForObject(GenerateTreeview<SaveInfo> m_SimpleTreeView,
        AssetTreeViewItem<SaveInfo> parent, UnityEngine.Object assetOb)
    {
        var assetPath = AssetDatabase.GetAssetPath(assetOb);
        Debug.Log(assetOb.name);
        if (assetPath.Equals(""))
        {
            throw new System.Exception(assetOb.name + "\t不是可获取依赖资源");
        }

        if (assetOb.GetType() == typeof(MonoScript))
        {
            return;
        }

        var guid = AssetDatabase.AssetPathToGUID(assetPath);
        string[] depens = AssetDatabase.GetDependencies(assetPath);
        SaveInfo si = new SaveInfo(assetOb.name, assetPath, assetOb, assetOb.GetType());
        var localParent = m_SimpleTreeView.AddForChild(parent, si.GetDisplayName(), si);

        for (int i = 0; i < depens.Length; i++)
        {
            if (!depens[i].Equals(si.path))
            {
                var tempDepen = AssetDatabase.GetDependencies(depens[i]);
                var depenAsset = AssetDatabase.LoadAssetAtPath(depens[i], typeof(UnityEngine.Object));

                GetDependenciesForObject(m_SimpleTreeView, localParent, depenAsset);
            }
        }
    }

    public void GetDependenciesForPath(GenerateTreeview<SaveInfo> m_SimpleTreeView, AssetTreeViewItem<SaveInfo> parent,
        string path)
    {
        var dir = new DirectoryInfo(path);
        SaveInfo si = new SaveInfo(dir.Name, path, null, dir.GetType());
        var localParent = m_SimpleTreeView.AddForChild(parent, si.GetDisplayName(), si);
        var subPaths = Directory.GetDirectories(path);


        foreach (var itemPath in subPaths)
        {
            GetDependenciesForPath(m_SimpleTreeView, localParent, itemPath);
        }
        
        var allAssets = Snake.FileUtils.GetRelativePath(path);
        var ret = Directory.GetFiles(path);
        Debug.Log(ret.Length);
        foreach (var assetPath in ret)
        {
            if (!assetPath.EndsWith(".meta"))
            {
                var localRelativePath = Snake.FileUtils.GetRelativePath(assetPath);
                var localAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(localRelativePath);
                GetDependenciesForObject(m_SimpleTreeView, localParent, localAsset);
            }
        }
    }
}
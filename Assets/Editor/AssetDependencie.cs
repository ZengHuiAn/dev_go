using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.TreeViewExamples;
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


    public void GetDependenciesForObject(MyTreeElement element,string assetPath,List<MyTreeElement> treeElements)
    {
        if (assetPath.Equals(""))
        {
            throw new System.Exception(assetPath + "\t不是可获取依赖资源");
        }

        var assetOb = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
        if (assetOb == null)
        {
            Debug.LogErrorFormat("Error  Path {0}", assetPath);
            return;
        }

        if (assetOb.GetType() == typeof(MonoScript))
        {
            return;
        }
    
        var guid = AssetDatabase.AssetPathToGUID(assetPath);


        var extenName = Path.GetExtension(assetPath);
        MyTreeElement child = new MyTreeElement(assetOb.name, element.depth+1, CounterID++);

        
        var replacePath = assetPath.Replace('\\', '/');
        
        child.Init(assetOb,replacePath);
        treeElements.Add(child);
        GetObjectDepencies(child, child.path, treeElements);
    }


    public void GetObjectDepencies(MyTreeElement element,string assetPath,List<MyTreeElement> treeElements)
    {
        string[] depens = AssetDatabase.GetDependencies(assetPath);
        for (int i = 0; i < depens.Length; i++)
        {
            if (!depens[i].Equals(assetPath))
            {
                var tempDepen = AssetDatabase.GetDependencies(depens[i]);
                var depenAsset = AssetDatabase.LoadAssetAtPath(depens[i], typeof(UnityEngine.Object));
                MyTreeElement child = new MyTreeElement(depenAsset.name, element.depth+1, CounterID++);
                if (depenAsset.GetType() != typeof(MonoScript))
                {
                    child.Init(depenAsset,depens[i]);
                    treeElements.Add(child);
                    GetObjectDepencies(element, depens[i], treeElements);
                }
                

            }
        }
    }

    private int CounterID = 0;

    /*
     * 根据路径获取依赖
     */
    public List<MyTreeElement> GetForPath(string path)
    {
        var localRelativePath = Snake.FileUtils.GetRelativePath(path);
        
        // 初始化ID计数器
        CounterID = 0; 
        // 初始化树的数据
        var treeElements = new List<MyTreeElement>();
        //初始化一个看不见的Root节点 depth 设置为-1 则不渲染这个节点
        var root = new MyTreeElement(localRelativePath, -1, CounterID);
        root.path = localRelativePath;
        treeElements.Add(root);

        GetDependenciesForPath(root,root.path,treeElements);
        return treeElements;
    }
    public void GetDependenciesForPath(MyTreeElement element,string path,List<MyTreeElement> treeElements)
    {
        var localRelativePath = Snake.FileUtils.GetRelativePath(path);
        var dir = new DirectoryInfo(localRelativePath);
        MyTreeElement child = new MyTreeElement(dir.Name, element.depth+1, CounterID++);
        var eObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(localRelativePath);
        child.Init(eObject, localRelativePath);
        treeElements.Add(child);
        var subPaths = Directory.GetDirectories(localRelativePath);
        foreach (var itemPath in subPaths)
        {
            GetDependenciesForPath(child, itemPath.Replace('\\', '/'),treeElements);
        }

        var allAssets = Snake.FileUtils.GetRelativePath(localRelativePath);
        var ret = Directory.GetFiles(localRelativePath);
        Debug.Log(ret.Length);
        foreach (var assetPath in ret)
        {
            if (!assetPath.EndsWith(".meta"))
            {
                var relativePath = Snake.FileUtils.GetRelativePath(assetPath);
                GetDependenciesForObject(child, relativePath, treeElements);
            }
        }
    }
    
}
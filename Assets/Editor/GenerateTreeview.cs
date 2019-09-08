using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class GenerateTreeview<T> : TreeView
{
    public AssetTreeViewItem<T> root;
    int itemIndex = 0;
    public GenerateTreeview(string disName,T currentData, TreeViewState treeViewState) : base(treeViewState)
    {
        this.AddRoot("root", currentData);
        this.AddForChild(root, disName, currentData);
    }

    public void AddRoot(string displayName, T currentData)
    {
        itemIndex = 1;
        root = new AssetTreeViewItem<T>(0,-1,displayName, currentData);
    }

    public AssetTreeViewItem<T> AddForChild(AssetTreeViewItem<T> node, string displayName, T data)
    {
        var newNode = new AssetTreeViewItem<T>(GetIndex(), node.depth + 1, displayName, data);
        node.AddChild(newNode);

        return newNode;
    }

    /// <summary>
    /// 获取自增的索引值
    /// </summary>
    /// <returns></returns>
    public int GetIndex()
    {
        itemIndex++;
        return itemIndex;
    }

    protected override TreeViewItem BuildRoot()
    {
        SetupDepthsFromParentsAndChildren(root);
        return root;
    }

}


public class AssetTreeViewItem<T> : TreeViewItem
{
    public string aPath;
    public T data;
    public AssetTreeViewItem(int id, int depth, string displayName, T currentData) : base(id, depth, displayName)
    {
        aPath = displayName;
        this.data = currentData;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * 选择的是什么类型的资源
 */
[System.Serializable]
public enum ShowType
{
    SingleAsset = 0,
    DirAsset = 1,
}
[System.Serializable]
public class SaveInfo
{
    public string displayName;
    public string path;
    public UnityEngine.Object asset;
    public Type type;

    public SaveInfo()
    {
    }

    public SaveInfo(string displayName)
    {
        this.displayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
    }

    public SaveInfo(string displayName, string path) : this(displayName)
    {
        this.path = path ?? throw new ArgumentNullException(nameof(path));
    }

    public SaveInfo(string displayName, string path, UnityEngine.Object asset)
    {
        this.displayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        this.path = path ?? throw new ArgumentNullException(nameof(path));
        this.asset = asset;
    }

    public SaveInfo(string displayName, string path, UnityEngine.Object asset, Type type) : this(displayName, path, asset)
    {
        this.type = type ?? throw new ArgumentNullException(nameof(type));
    }

    public string GetDisplayName()
    {
        return string.Format("{0}\t type:{1} \t path:{2}", this.displayName, this.type.Name,this.path);
    }
}

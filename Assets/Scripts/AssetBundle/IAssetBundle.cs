using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAssetBundle
{
    string bundleName { get; }
    IEnumerator DownLoadBundle();
}
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Editor.MultifyTreeViewPro
{
    
    // Asset Base Class
    
    
    public class TreeAsset : ScriptableObject
    {
         [SerializeField] List<TreeElement> mTreeElements = new List<TreeElement>();
        
        internal List<TreeElement> treeElements
        {
            get { return mTreeElements; }
            set { mTreeElements = value; }
        }

        public virtual void Init(TreeElement element)
        {
        }
    }
}
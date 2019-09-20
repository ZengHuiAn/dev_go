using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.TreeViewExamples
{
	
	[CreateAssetMenu (fileName = "TreeDataAsset", menuName = "Tree Asset", order = 1)]
	public class MyTreeAsset : ScriptableObject
	{
		[SerializeField] string path =  "";
		[SerializeField] List<MyTreeElement> m_TreeElements = new List<MyTreeElement> ();

		internal List<MyTreeElement> treeElements
		{
			get { return m_TreeElements; }
			set { m_TreeElements = value; }
		}
		
		internal string Path
		{
			get { return path; }
			set { path = value; }
		}

		void Awake ()
		{
//				m_TreeElements = MyTreeElementGenerator.GenerateRandomTree(160);
		}

		public void Init()
		{
			this.m_TreeElements = AssetDependencie.Instance.GetForPath(this.Path);
		}
	}
}

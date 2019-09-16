using System;
using UnityEngine;


namespace UnityEditor.TreeViewExamples
{

	[Serializable]
	public class MyTreeElement : TreeElement
	{
		public string path;
		public UnityEngine.Object eObject;

		public MyTreeElement (string name, int depth, int id) : base (name, depth, id)
		{
			eObject = null;
			path = Application.dataPath;
		}

		public void Init( UnityEngine.Object eObject,string path)
		{
			this.eObject = eObject;
			this.path = path;
		}
	}
}

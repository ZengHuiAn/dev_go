using System;
using UnityEngine;


namespace UnityEditor.TreeViewExamples
{

	[Serializable]
	internal class MyTreeElement : TreeElement
	{
		public string name;
		public string path;
		public UnityEngine.Object eObject;

		public MyTreeElement (string name, int depth, int id) : base (name, depth, id)
		{
			eObject = null;
			path = Application.dataPath;
		}
	}
}

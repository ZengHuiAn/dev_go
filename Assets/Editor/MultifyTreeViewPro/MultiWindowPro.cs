using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
namespace Snake
{
    public class MultiWindowPro : EditorWindow
    {
        //
        [MenuItem("TreeView/Windows")]
        public static MultiWindowPro GetWindowPro()
        {
            var icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Editor/MultifyTreeViewPro/texture/tree.png");
            var window = GetWindow<MultiWindowPro>();
            window.titleContent = new GUIContent("Tree", icon);
            window.Focus();
            window.Repaint();
            return window;
        }
        [OnOpenAsset]
        public static bool OnOpenAssets(int instanceID, int line)
        {
            var treeAsset = EditorUtility.InstanceIDToObject(instanceID);

            if (treeAsset != null)
            {
                return true;
            }
            return false;
        }
    }

}

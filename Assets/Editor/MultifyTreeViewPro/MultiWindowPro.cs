using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Editor.MultifyTreeViewPro
{
    public class MultiWindowPro : EditorWindow
    {
        [SerializeField] TreeViewState mTreeViewState; // Serialized in the window layout file so it survives assembly reloading
        [SerializeField] MultiColumnHeaderState mMultiColumnHeaderState;
        SearchField _mSearchField;
        TreeAsset _treeAsset;
    
        
        /*
         * 第一次初始化所有资源
         */
        private bool m_Initialized = true;
        
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
        public static bool OnOpenAssets(int instanceId, int line)
        {
            var treeAsset = EditorUtility.InstanceIDToObject(instanceId) as TreeAsset;
            //treeAsset.GetType();
            Debug.Log(treeAsset);
            
            if (treeAsset != null)
            {
                var window = GetWindowPro();
                window.SetTreeAsset( treeAsset);
                return true;
            }
            return false; 
        }

        void SetTreeAsset(TreeAsset myTreeAsset)
        {
            _treeAsset = myTreeAsset;
            m_Initialized = false;
        }
    }

}

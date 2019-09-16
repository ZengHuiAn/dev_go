using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Serialization;

public class AssetBundleGenerateWindow : EditorWindow
{
    [MenuItem("Window/AssetBundleGenerateWindow")]
    public static void init()
    {
        EditorWindow.CreateInstance<AssetBundleGenerateWindow>().Show();
    }

    #region 选择属性
    [SerializeField]
    public ShowType showType;
    #endregion

    #region ScrollView 属性
    [SerializeField]
    public TreeViewState m_TreeViewState;
    [SerializeField]
    public GenerateTreeview<SaveInfo> m_SimpleTreeView;

    [SerializeField]
    public SearchField m_searchField;
    #endregion
    #region 选中物体属性
    [SerializeField]
    public UnityEngine.Object scriptableObject = null;
    #endregion


    [SerializeField]
    public bool createScrollView = false;


    [FormerlySerializedAs("path")] [SerializeField]
    public string selectAssetPath = "";

    private void OnEnable()
    {
        if (m_TreeViewState == null)
            m_TreeViewState = new TreeViewState();
        m_SimpleTreeView = new GenerateTreeview<SaveInfo>("AssetBundle依赖查找工具", null, m_TreeViewState);
        SaveInfo si = new SaveInfo("111");
        var node = m_SimpleTreeView.AddForChild(m_SimpleTreeView.root, si.displayName, si);

        m_SimpleTreeView.Reload();

    }

    private void OnGUI()
    {
        #region 选择要查找的类型
        DrawShowType();
        #endregion

        DrawCurrentSelect();

        DrawGetDependencieBtn();

        if (scriptableObject == null)
        {
            createScrollView = false;
        }

        if (createScrollView == false || m_SimpleTreeView == null || m_searchField == null)
        {
            return;
        }
        EditorGUILayout.Space();
        DrawSearch();
        EditorGUILayout.Space();
        DrawScrollView();
    }

    void DrawCurrentSelect()
    {
        if (showType == ShowType.SingleAsset)
        {
            DrawSelectAsset();
        }
        else
        {
            DrawSelectPath();
        }

    }

    void DrawSelectAsset()
    {
        #region 选择资源器
        //ConsoleE.Api.ClearLog();
        EditorGUILayout.BeginVertical();
        scriptableObject = EditorGUILayout.ObjectField("要检查的资源", scriptableObject, typeof(UnityEngine.Object), false);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        #endregion
    }


    void DrawSelectPath()
    {
        #region 选择路径框
        GUILayout.BeginHorizontal();

        if (selectAssetPath.Equals(""))
        {
            selectAssetPath = Application.dataPath + "/AssetBundle";
        }

        selectAssetPath = GUILayout.TextArea(selectAssetPath, GUILayout.ExpandWidth(true), GUILayout.Height(20));
        if (GUILayout.Button("选择路径", GUILayout.ExpandWidth(true), GUILayout.Height(20)))
        {
            selectAssetPath = EditorUtility.OpenFolderPanel("选择要检查资源的路径", Application.dataPath, "");
        }
        GUILayout.EndHorizontal();
        #endregion
    }

    void DrawShowType()
    {
        #region 设置资源类型
        showType = (ShowType)EditorGUILayout.EnumPopup(showType, GUILayout.Width(200));
        #endregion
    }

    void DrawGetDependencieBtn()
    {

        if (GUILayout.Button("获取依赖", GUILayout.ExpandWidth(true), GUILayout.Height(20)))
        {
            ConsoleE.Api.ClearLog();
            if (m_TreeViewState == null)
                m_TreeViewState = new TreeViewState();
            m_SimpleTreeView = new GenerateTreeview<SaveInfo>("AssetBundle依赖查找工具", null, m_TreeViewState);

            m_searchField = new SearchField();

            m_searchField.downOrUpArrowKeyPressed += m_SimpleTreeView.SetFocusAndEnsureSelectedItem;
            GetDependencie();
        }
    }

    void GetDependencie()
    {
        if (!scriptableObject)
        {
            return;
        }
        SetTreeView();
    }

    public void SetTreeView()
    {
//        if (this.showType == ShowType.SingleAsset)
//        {
//            AssetDependencie.Instance.GetDependenciesForObject(m_SimpleTreeView, m_SimpleTreeView.root,scriptableObject);
//        }
//        else
//        {
//            AssetDependencie.Instance.GetDependenciesForPath(m_SimpleTreeView, m_SimpleTreeView.root,selectAssetPath);
//        }
//        
//        m_SimpleTreeView.Reload();
//        createScrollView = true;
    }

    void DrawSearch()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Space(100);
        GUILayout.FlexibleSpace();

        m_SimpleTreeView.searchString = m_searchField.OnToolbarGUI(m_SimpleTreeView.searchString);
        GUILayout.EndHorizontal();
    }

    void DrawScrollView()
    {
        #region 节点绘制
        GUILayout.BeginScrollView(Vector2.zero, GUILayout.Width(position.width), GUILayout.Height(position.height));
        m_SimpleTreeView.OnGUI(new Rect(0, 0, position.width, position.height));
        GUILayout.EndScrollView();
        #endregion
    }
}

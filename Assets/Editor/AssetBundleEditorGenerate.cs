using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System;

public class AssetBundleGenerate : SearchableEditorWindow
{
    [MenuItem("Window/Generate")]
    public static void init()
    {
        EditorWindow.CreateInstance<AssetBundleGenerate>().Show();

    }



    [SerializeField]
    public TreeViewState m_TreeViewState;
    [SerializeField]
    public GenerateTreeview<SaveInfo> m_SimpleTreeView;

    [SerializeField]
    public string obPath = "";
    [SerializeField]
    public UnityEngine.Object scriptableObject = null;
    [SerializeField]
    public SearchField m_searchField;
    [SerializeField]
    public string path = "";
    [SerializeField]
    public ShowType showType;
    [SerializeField]
    public bool createScrollView = false;
    private void OnGUI()
    {

        DrawShowType();
        if (showType == ShowType.SingleAsset)
        {
            DrawSelectAsset();
        }
        else
        {
            DrawSelectPath();
        }


        DrawGetDependencieBtn();

        if (scriptableObject == null)
        {
            createScrollView = false;
        }

        if (createScrollView == false)
        {
            return;
        }
        EditorGUILayout.Space();
        //DrawSearch();
        EditorGUILayout.Space();

        DrawScrollView();

    }

    void DrawShowType()
    {
        #region 设置资源类型
        showType = (ShowType)EditorGUILayout.EnumPopup(showType, GUILayout.Width(200));
        #endregion
        EditorGUILayout.Space();
    }

    void DrawSelectAsset()
    {
        #region 选择资源器
        //ConsoleE.Api.ClearLog();
        GUILayout.BeginVertical();


        if (scriptableObject != null)
        {
            obPath = AssetDatabase.GetAssetPath(scriptableObject);
            GUILayout.Label(obPath);
        }
        else
        {
            scriptableObject = AssetDatabase.LoadAssetAtPath(obPath, typeof(UnityEngine.Object));
            GUILayout.Label(obPath);
        }

        scriptableObject = EditorGUILayout.ObjectField("要检查的资源", scriptableObject, typeof(UnityEngine.Object), true);
        GUILayout.EndVertical();
        GUILayout.Space(10);
        #endregion
    }

    void DrawSelectPath()
    {
        #region 选择路径框
        GUILayout.BeginHorizontal();

        if (path.Equals(""))
        {
            path = Application.dataPath + "/AssetBundle";
        }

        path = GUILayout.TextArea(path, GUILayout.Width(position.width - 100), GUILayout.Height(20));
        if (GUILayout.Button("选择路径", GUILayout.Width(50), GUILayout.Height(20)))
        {
            path = EditorUtility.OpenFolderPanel("选择要检查资源的路径", Application.dataPath, "");
            Debug.Log(path);
        }
        GUILayout.EndHorizontal();
        #endregion
    }

    void DrawSearch()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Space(100);
        GUILayout.FlexibleSpace();

        m_SimpleTreeView.searchString = m_searchField.OnToolbarGUI(m_SimpleTreeView.searchString);
        GUILayout.EndHorizontal();
    }

    void DrawGetDependencieBtn()
    {

        if (GUILayout.Button("获取依赖", GUILayout.ExpandWidth(true), GUILayout.Height(20)))
        {
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


        AssetDependencie.Instance.GetDependenciesForObject(m_SimpleTreeView, scriptableObject);
        //GenerateTreeview.DefaultStyles.backgroundEven = GUIStyle

        m_SimpleTreeView.Reload();
        createScrollView = true;



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


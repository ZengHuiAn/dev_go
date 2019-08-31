using Snake;
using System;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class XLuaBehaviour : MonoBehaviour
{
    LuaManager lua { get { return LuaManager.Instance; } }
    public string luaFile;
    public object[] args = null;
    public LuaTable scriptTable { get; private set; }
  
    Action lua_Awake;
    Action lua_Start;
    Action lua_OnEnable;
    Action lua_OnDisable;
    Action lua_Update;
    Action lua_FixedUpdate;
    Action lua_LateUpdate;
    Action lua_OnDestroy;
    void Awake()
    {
        if (!string.IsNullOrEmpty(luaFile) && lua != null)
        {
            object[] obs = LuaManager.DoFile2(luaFile,null);
            if (obs.Length == 0) return;
           
            scriptTable = obs[0] as LuaTable;
            scriptTable.Set("self", this);
            scriptTable.Get("Awake", out lua_Awake);
            scriptTable.Get("Start", out lua_Start);
            scriptTable.Get("OnEnable", out lua_OnEnable);
            scriptTable.Get("OnDisable", out lua_OnDisable);
            scriptTable.Get("Update", out lua_Update);
            scriptTable.Get("FixedUpdate", out lua_FixedUpdate);
            scriptTable.Get("LateUpdate", out lua_LateUpdate);
            scriptTable.Get("OnDestroy", out lua_OnDestroy);

            args = new object[1];
            args[0] = gameObject;
        }
        if (lua_Awake != null)
            lua_Awake();
    }

   
    void Start()
    {
        if (lua_Start != null)
        {
            lua_Start();
        }
    }
    void OnEnable()
    {
        if (lua_OnEnable != null)
        {
            lua_OnEnable();
        }
    }
    void OnDisable()
    {
        if (lua_OnDisable != null)
        {
            lua_OnDisable();
        }
    }
    void Update()
    {
        if (lua_Update != null)
        {
            lua_Update();
        }
    }
    void FixedUpdate()
    {
        if (lua_FixedUpdate != null)
        {
            lua_FixedUpdate();
        }
    }
    void LateUpdate()
    {
        if (lua_LateUpdate != null)
        {
            lua_LateUpdate();
        }
    }
    void OnDestroy()
    {
        if (lua_OnDestroy != null)
        {
            lua_OnDestroy();
        }

        if (scriptTable!=null)
        {
            if (!lua.Disposed)
            {
                scriptTable.Dispose();
            }
            
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XLua;
namespace Snake
{

    [System.Serializable]
    public class Injection
    {
        public string name;
        public GameObject value;
    }

    [LuaCallCSharp]
    public class LuaBehaviour : MonoBehaviour
    {
        [CSharpCallLua]
        public delegate void LuaObjectAction();
        public string luaScript;
        public Injection[] injections;


        private LuaObjectAction luaStart;
        private LuaObjectAction luaUpdate;
        private LuaObjectAction luaOnDestroy;

        private LuaTable luaObject;
        
        void Awake()
        {
//            scriptEnv = LuaController.Instance.LuaEnv.NewTable();
    
            if (luaScript != "")
            {
                string path = "require '(" + luaScript + ")'";
                object[] obs = LuaController.Instance.LuaEnv.DoString(path, "LuaTestScript", luaObject);
                luaObject = obs[0] as  LuaTable;
            }
            if (luaObject == null)
            {
                Debug.Log("Lua loader error");
                return;
            }
            // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
            //LuaTable meta = LuaController.Instance.LuaEnv.NewTable();
            //meta.Set("__index", LuaController.Instance.LuaEnv.Global);
            //scriptEnv.SetMetaTable(meta);
            //meta.Dispose();

            luaObject.Set("gameObject", gameObject);
            foreach (var injection in injections)
            {
                luaObject.Set(injection.name, injection.value);
            }

            LuaObjectAction luaAwake = luaObject.Get<LuaObjectAction>("Awake");
            LuaObjectAction luaStart = luaObject.Get<LuaObjectAction>("Start");
            LuaObjectAction luaUpdate = luaObject.Get<LuaObjectAction>("Update");
            LuaObjectAction luaOnDestroy = luaObject.Get<LuaObjectAction>("OnDestroy");
            Debug.Log(luaStart);
            if (luaAwake != null)
            {
                luaAwake();
            }
        }

        // Use this for initialization
        void Start()
        {
            if (luaStart != null)
            {
                luaStart();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (luaUpdate != null)
            {
                luaUpdate();
            }
        }

        void OnDestroy()
        {
            if (luaOnDestroy != null)
            {
                luaOnDestroy();
            }

            luaOnDestroy = null;
            luaUpdate = null;
            luaStart = null;
            try
            {
                luaObject.Dispose();
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message + "\n" + e.StackTrace);
            }
        
            injections = null;
        }
        
        
        
    }
}
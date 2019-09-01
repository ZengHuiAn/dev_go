using Snake;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using XLua;

namespace SGK
{
    [System.Serializable]
    public class Injection
    {
        public string name;
        public GameObject value;
    }
    // [DisallowMultipleComponent]
    public class LuaBehaviour : MonoBehaviour
    {
        [CSharpCallLua]
        public delegate void LuaObjectAction(object lauObject, params object[] args);

        [CSharpCallLua]
        public delegate void LuaOnAction();

        LuaObjectAction l_Awake;
        LuaOnAction l_Start;
        LuaOnAction l_OnDestroy;
        LuaOnAction l_Update;
        LuaObjectAction l_onEvent;

        public string luaScriptFileName = "";
        public LuaTable luaObject = null;
        public Injection[] injections;
        public object[] args = null;
        bool lateLoad = false;
        bool scriptIsReady = false;

        void Awake()
        {
            //if (L != null && !string.IsNullOrEmpty(luaScriptFileName))
            //{
            //    luaObject = loadDelegate();
            //}
            args = new object[1];
            args[0] = gameObject;

            StartWithScript(luaScriptFileName, luaObject, args);
        }

        void Start()
        {
            if (l_Start != null) l_Start();
        }

        public void StartWithScript(string luaScriptFileName, LuaTable luaObject, params object[] args)
        {
            this.luaScriptFileName = luaScriptFileName;
            if (args != null)
            {
                this.args = args;
            }

            // release old object
            if (this.luaObject != null && this.luaObject != luaObject)
            {
                this.luaObject.Dispose();
                l_Awake = null;
                l_OnDestroy = null;
                l_Update = null;
                l_onEvent = null;
            }

            if (luaObject == null && !string.IsNullOrEmpty(luaScriptFileName))
            {
                luaObject = loadDelegate();
            }
            this.luaObject = luaObject;

            if (this.luaObject == null)
            {
                return;
            }

            luaObject.Set("gameObject", gameObject);
            foreach (var injection in injections)
            {
                luaObject.Set(injection.name, injection.value);
            }
            l_Awake = luaObject.Get<LuaObjectAction>("Awake");
            l_Start = luaObject.Get<LuaOnAction>("Start");


            l_OnDestroy = luaObject.Get<LuaOnAction>("OnDestroy");

            l_Update = luaObject.Get<LuaOnAction>("Update");
            l_onEvent = luaObject.Get<LuaObjectAction>("onEvent");

            luaAwake();
        }

        void luaAwake()
        {
            if (l_Awake != null)
            {
                if (args != null)
                {
                    l_Awake(luaObject, args);
                }
                else
                {
                    l_Awake(luaObject);
                }
            }
            LuaController.RegisterEventListener(luaObject);
            scriptIsReady = true;
        }

        void Update()
        {
            if (scriptIsReady && l_Update != null) l_Update();
        }

        void OnEnable()
        {
            if (scriptIsReady && luaObject != null)
            {
                LuaController.RegisterEventListener(luaObject);
            }
        }

        void OnDisable()
        {
            if (scriptIsReady && luaObject != null && L != null)
            {
                LuaController.RemoveEventListener(luaObject);
            }
        }

        void OnDestroy()
        {
            if (!scriptIsReady)
            {
                return;
            }
            scriptIsReady = false;

            if (l_OnDestroy != null && L != null) l_OnDestroy();

            l_Awake = null;
            l_Start = null;
            l_OnDestroy = null;
            l_Update = null;
            l_onEvent = null;

            if (L != null && luaObject != null)
            {
                luaObject.Dispose();

            }
            luaObject = null;
        }

        public void onEvent(string eventName)
        {
            if (l_onEvent != null) l_onEvent(luaObject, eventName);
        }

        public LuaTable loadDelegate()
        {
            if (!string.IsNullOrEmpty(luaScriptFileName) && L != null)
            {
                object[] objs = L.DoString(FileUtils.LoadStringFromFile(luaScriptFileName), luaScriptFileName);
                print(objs);
                if (objs != null && objs.Length > 0)
                {
                    return objs[0] as LuaTable;
                }
                Debug.LogError(string.Format("Do file '{0:s}' failed", luaScriptFileName));
            }
            return null;
        }

        LuaEnv L
        {
            get
            {
                return LuaController.GetLuaState();
            }
        }
    }
}

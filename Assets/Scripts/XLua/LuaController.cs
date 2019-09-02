using UnityEngine;

using XLua;

namespace SGK
{
    public class LuaController : MonoBehaviour
    {
        static LuaController instance;

        LuaEnv L = null;
        float lastGCTime = 0;

        System.Action<object> _RegisterEventListener;
        System.Action<object> _RemoveEventListener;

        [CSharpCallLua]
        public delegate void DispatchEventDelegate(string e, params object[] objs);
        DispatchEventDelegate _DispatchEvent;

        [CSharpCallLua]
        public delegate void StartLuaCoroutineDelegate(LuaFunction func, params object[] objs);
        StartLuaCoroutineDelegate _StartLuaCoroutineDelegate;

        System.Action MainAction;

        void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(this.gameObject);

            L = new LuaEnv();
            L.AddLoader(FileUtils.Load);

            //L.Global.Set<string, XLua.LuaDLL.lua_CSFunction>("WARNING_LOG", CustomSettings.PrintWarning);
            //L.Global.Set<string, XLua.LuaDLL.lua_CSFunction>("ERROR_LOG", CustomSettings.PrintError);
            //L.Global.Set<string, XLua.LuaDLL.lua_CSFunction>("loadstring", CustomSettings.LoadString);

            IService[] services = gameObject.GetComponents<IService>();
            for (int i = 0; i < services.Length; i++)
            {
                services[i].Register(L);
            }
            L.DoString("require 'main'");


            MainAction = L.Global.Get<System.Action>("main");
            if (MainAction != null)
            {
                MainAction();
            }
            //_RegisterEventListener = L.Global.Get<System.Action<object>>("RegisterEventListener");
            //_RemoveEventListener = L.Global.Get<System.Action<object>>("RemoveEventListener");
            //_DispatchEvent = L.Global.Get<DispatchEventDelegate>("DispatchEvent");
            //_StartLuaCoroutineDelegate = L.Global.Get<StartLuaCoroutineDelegate>("StartCoroutine");
        }

        void Update()
        {
            if (L != null && Time.realtimeSinceStartup - lastGCTime > 1.0f)
            {
                lastGCTime = Time.realtimeSinceStartup;
                //    L.GC();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DispatchEvent("KEYDOWN_ESCAPE_ONLY");
            }
        }

        void OnDestroy()
        {
            Dispose(false);
            if (instance == this)
            {
                instance = null;
            }
        }

        public void Dispose(bool destoryService = true)
        {
            if (L == null)
            {
                return;
            }

            if (_DispatchEvent != null) _DispatchEvent("UNITY_OnApplicationQuit");

            _RegisterEventListener = null;
            _RemoveEventListener = null;
            _DispatchEvent = null; ;
            _StartLuaCoroutineDelegate = null;

            //if (destoryService)
            //{
            //    IService[] services = gameObject.GetComponents<IService>();
            //    for (int i = 0; i < services.Length; i++)
            //    {
            //        DestroyImmediate(services[i] as MonoBehaviour);
            //    }
            //}

            L.GC();

            System.GC.Collect();

            L.Dispose();
            L = null;
        }

        public static void DisposeInstance()
        {
            if (instance != null)
            {
                Destroy(instance.gameObject);
            }
        }

        public static LuaEnv GetLuaState()
        {
            return instance ? instance.L : null;
        }

        public static void RegisterEventListener(object obj)
        {
            if (instance != null && instance._RegisterEventListener != null && obj != null)
            {
                instance._RegisterEventListener(obj);
            }
        }

        public static void RemoveEventListener(object obj)
        {
            if (instance != null && instance._RemoveEventListener != null && obj != null)
            {
                instance._RemoveEventListener(obj);
            }
        }

        public static void DispatchEvent(string e, params object[] objs)
        {
            if (instance != null && instance._DispatchEvent != null)
            {
                instance._DispatchEvent(e, objs);
            }
        }

        public static T LoadString<T>(string script)
        {
            if (instance != null)
            {
                LuaFunction func = instance.L.LoadString(script);
                return (func != null) ? func.Cast<T>() : default(T);
            }
            return default(T);
        }

        public static object[] DoString(string script, params object[] objs)
        {
            if (instance != null)
            {
                LuaFunction func = instance.L.LoadString(script);
                return func.Call(objs);
            }
            return null;
        }

        public static T GetLuaValue<T>(string name)
        {
            if (instance != null)
            {
                return instance.L.Global.Get<T>(name);
            }
            return default(T);
        }

        public static object[] DoStringInThread(string script, string chunkName, params object[] objs)
        {
            if (instance != null)
            {
                LuaFunction func = instance.L.LoadString(script, chunkName);
                if (func != null)
                {
                    instance._StartLuaCoroutineDelegate(func, objs);
                }
            }
            return null;
        }
    }
}

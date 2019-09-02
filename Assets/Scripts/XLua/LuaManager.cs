using UnityEngine;
using System.IO;
using System;
using System.Text;
using XLua;
using Snake;

public class LuaManager : MonoBehaviour
{

    //是否加密解密
    public bool encryption = false;
    public static LuaManager Instance;
    public bool Disposed = false;
    private LuaEnv lua;

    public LuaEnv GetState
    {

        get
        {
            return lua;
        }
    }

    public LuaTable Global { get { return lua == null ? null : lua.Global; } }
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        lua = new LuaEnv();
        InitThirdLibs();
        if (!encryption)
            InitPackagePath();
        else
            lua.AddLoader(Loader);
        //开启main.lua 入口
        StartUp();

    }
    void Update()
    {
        if (lua != null)
            lua.Tick();
    }
    void OnDestroy()
    {
        try
        {
            Disposed = true;
            lua.Dispose();
            lua = null;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            //
        }
        
    }
    #region 一些接口
    public void StartUp()
    {
        DoFile("main");
        Action mainFunc;
        Get("main", out mainFunc);
        mainFunc();
    }
    public object[] DoString(string script, string chunk = "chunk", LuaTable env = null)
    {
        return lua.DoString(script, chunk, env);
    }
    //因为之前遇到的一个 通过require加载代码 出现了 env变成了 "require xxx"所在环境，而不是require加载的代码的环境。
    //那么当我设置了一个env.this=this的时候，在本来加载进来的lua代码里就访问不到了。得到require层才能访问到
    //所以这里就不用require来加载了。
    public object[] DoFile(string filename, LuaTable env = null)
    {
        byte[] buffer = Loader(ref filename);
        string script = Encoding.Default.GetString(buffer);
        return lua.DoString(script, filename, env);
    }
    public static object[] DoString(byte[] script, params object[] objs)
    {
        if (LuaManager.Instance != null)
        {
            LuaFunction func = LuaManager.Instance.GetState.LoadString<LuaFunction>(script);
            return func.Call(objs);
        }
        return null;
    }

    public static object[] DoFile2(string fileName, params object[] objs)
    {
        if (LuaManager.Instance != null)
        {
            LuaFunction func = Load(fileName, fileName);
            return func.Call(objs);
        }
        return null;
    }

    public static LuaFunction Load(string file, string chunkName = "chunk", LuaTable env = null)
    {
        if (LuaManager.Instance == null)
        {
            return null;
        }

        var bbs = System.Text.Encoding.UTF8.GetBytes(FileUtils.ReadFileContent(file));
        if (bbs == null)
        {
            return null;
        }
        return LuaManager.Instance.GetState.LoadString<LuaFunction>(bbs, chunkName, env);
    }


    public object[] CallFunction(string funcName, params object[] args)
    {
        LuaFunction func = lua.Global.Get<LuaFunction>(funcName);
        var result = func.Call(args);
        func.Dispose();
        func = null;
        return result;
    }
    public void Get<K, V>(K key, out V val)
    {
        lua.Global.Get(key, out val);
    }

    public LuaTable NewTable(bool setGlobalEnv = false)
    {
        LuaTable table = lua.NewTable();
        LuaTable meta = lua.NewTable();
        meta.Set("__index", Global);
        table.SetMetaTable(meta);
        meta.Dispose();
        meta = null;
        return table;
    }

    public void Close()
    {
        lua.Dispose();
        lua = null;
        Disposed = true;
    }
    #endregion
    #region 环境设置
    //初始化一些第三方库
    void InitThirdLibs()
    {
    }
    void InitPackagePath()
    {
        lua.DoString("package.path=package.path..';" + FileUtils.envPath + "/?.lua" + "'");
    }
    byte[] Loader(ref string fileName)
    {
        //查找lua文件
        fileName = fileName.EndsWith(".lua") ? fileName.Substring(0, fileName.Length - 4) : fileName;
        fileName = fileName.Replace('.', '/');
        fileName = fileName + ".lua";
        DirectoryInfo dire = new DirectoryInfo(FileUtils.envPath);
        FileInfo fi = SeachFile(dire, fileName);
        if (fi == null)
        {
            Debug.LogError("Lua File: " + fileName + " Not Found!");
            return null;
        }
        fileName = fi.FullName;
        FileStream stream = fi.OpenRead();
        byte[] buffers = new byte[stream.Length];
        stream.Read(buffers, 0, buffers.Length);
        stream.Close();
        stream.Dispose();
        if (encryption)
        {
            //解密
        }
        return buffers;
    }
    FileInfo SeachFile(DirectoryInfo dire, string fileName)
    {
        FileInfo result = null;
        foreach (FileSystemInfo fs in dire.GetFileSystemInfos())
        {
            FileInfo fi = fs as FileInfo;
            if (fi == null)//directory
            {
                result = SeachFile(fs as DirectoryInfo, fileName);
            }
            else
            {
                string path = fi.FullName;
                if (path.IndexOf(fileName) >= 0)
                {
                    result = fi;
                    break;
                }
            }
        }
        return result;
    }
    #endregion
}
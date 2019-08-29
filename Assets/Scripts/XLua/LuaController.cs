using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Snake
{
    public class LuaController
    {
        //
        static LuaController luaController;
        public XLua.LuaEnv LuaEnv;
        public static LuaController Instance
        {
            get
            {
                if (luaController == null)
                {
                    luaController = new LuaController();

                }

                return LuaController.luaController;
            }
        }

        public LuaController()
        {
            this.init();

        }
        private void init()
        {
            LuaEnv = new XLua.LuaEnv();


            LuaEnv.AddLoader(FileUtils.LuaLoader);
        }

        public void DoString(string content)
        {
            LuaEnv.DoString(content);
        }


        internal static float lastGCTime = 0;
        internal const float GCInterval = 1;//1 second 

        private void Update()
        {

            if (LuaEnv != null)
            {
                LuaEnv.Tick();
                if (Time.time - LuaController.lastGCTime > GCInterval)
                {
                    if (LuaEnv.Memroy > 50)
                    {
                        LuaEnv.GC();
                    }

                    LuaController.lastGCTime = Time.time;
                }
            }
        }

        ~LuaController()
        {
            if (LuaEnv != null)
            {
                LuaEnv.Dispose();
            }
        }
    }
}
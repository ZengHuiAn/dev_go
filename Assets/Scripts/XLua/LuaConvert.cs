using System;
using XLua;
using System.Collections;
using UnityEngine;
using System.IO;
using Utils;

namespace Snake
{
    public static class LuaConvert
    {
        public static void Test(LuaTable tb)
        {
            ClientHeader clientHeader = new ClientHeader() { Length = 12 ,MessageID = 101,SN = 2};
            using (MemoryStream stream = new MemoryStream())
            {
                amf.Encode(stream, clientHeader);
                Debug.Log(stream.ToArray());
                Utils.utils.Log("Encode:", stream.ToArray());
            }
        }

        public static object[] ConvertObjects(LuaTable tb)
        {
            if (tb.Length == 0)
            {
                return null;
            }

            object[] obs = new object[tb.Length];

            for (int i = 0; i < tb.Length; i++)
            {
                object value = tb.Get<object, object>(i + 1);

                object result;
                if (value.GetType() == typeof(LuaTable))
                {
                    LuaTable luaTable = value as LuaTable;
                    result = ConvertObjects(luaTable);
                }
                else
                {
                    result = value;
                }

                obs[i] = result;
            }
            return obs;
        }

        public static LuaTable TestConvertTable()
        {
            //            LuaTable tb = LuaController.Instance.LuaEnv.NewTable();

            object ob = new object[]
            {
                new ClientHeader() {Length = 0, SN = 0, MessageID = 1},
                new int[] {1,9,9 }
            };

            utils.Log(ob);
            LuaTable tb = ConvertLuaTable(ob);

            return tb;

        }

        public static LuaTable ConvertLuaTable(object body, LuaTable luaTable = null)
        {
            
            AMF_TYPE tp = utils.GetObjectType(body);
            switch (tp)
            {
                case AMF_TYPE.Bool:
                case AMF_TYPE.Int:
                case AMF_TYPE.Float64:
                case AMF_TYPE.Uint:
                case AMF_TYPE.String:
                    if (luaTable == null)
                    {
                        luaTable = SGK.LuaController.GetLuaState()?.NewTable();
                    }

                    luaTable.Set<int, object>(luaTable.Length + 1, body);
                    break;

                case AMF_TYPE.Array:
                case AMF_TYPE.Struct:
                    object[] obs;
                    if (tp == AMF_TYPE.Array)
                    {
                        obs = Unpack((Array)body);
                    }
                    else
                    {
                        obs = utils.ConvertStruct(body);
                    }


                    LuaTable tb = SGK.LuaController.GetLuaState()?.NewTable();
                    for (int i = 0; i < obs.Length; i++)
                    {
                        ConvertLuaTable(obs[i], tb);
                    }

                    if (luaTable == null)
                    {
                        return tb;
                    }
                    luaTable.Set<int, LuaTable>(luaTable.Length + 1, tb);
                    break;
            }

            return luaTable;
        }

        public static object[] Unpack(Array sourceArray)
        {
            //
            object[] newOBs = new object[sourceArray.Length];
            for (int i = 0; i < sourceArray.Length; i++)
            {
                newOBs[i] = sourceArray.GetValue(i);
            }

            return newOBs;

        }

        public static object[] Unpack(ClientHeader sourceArray)
        {
            object[] clientHeader = new object[3];
            clientHeader[0] = sourceArray.Length;
            clientHeader[1] = sourceArray.MessageID;
            clientHeader[2] = sourceArray.SN;
            return clientHeader;
        }


    }
}
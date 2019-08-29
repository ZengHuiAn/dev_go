using UnityEngine;
using System.Collections;
using System;
using XLua;
using System.Collections.Generic;

namespace Tutorial
{
    [LuaCallCSharp]
    public class BaseClass
    {
        public static void BSFunc()
        {
            Debug.Log("Derived Static Func, BSF = " + BSF);
        }

        public static int BSF = 1;

        public void BMFunc()
        {
            Debug.Log("Derived Member Func, BMF = " + BMF);
        }

        public int BMF { get; set; }
    }

    public struct Param1
    {
        public int x;
        public string y;
    }

    [LuaCallCSharp]
    public enum TestEnum
    {
        E1,
        E2
    }


    [LuaCallCSharp]
    public class DerivedClass : BaseClass
    {
        [LuaCallCSharp]
        public enum TestEnumInner
        {
            E3,
            E4
        }

        public void DMFunc()
        {
            Debug.Log("Derived Member Func, DMF = " + DMF);
        }

        public int DMF { get; set; }

        public double ComplexFunc(Param1 p1, ref int p2, out string p3, Action luafunc, out Action csfunc)
        {
            Debug.Log("P1 = {x=" + p1.x + ",y=" + p1.y + "},p2 = " + p2);
            luafunc();
            p2 = p2 * p1.x;
            p3 = "hello " + p1.y;
            csfunc = () =>
            {
                Debug.Log("csharp callback invoked!");
            };
            return 1.23;
        }

        public void TestFunc(int i)
        {
            Debug.Log("TestFunc(int i)");
        }

        public void TestFunc(string i)
        {
            Debug.Log("TestFunc(string i)");
        }

        public static DerivedClass operator +(DerivedClass a, DerivedClass b)
        {
            DerivedClass ret = new DerivedClass();
            ret.DMF = a.DMF + b.DMF;
            return ret;
        }

        public void DefaultValueFunc(int a = 100, string b = "cccc", string c = null)
        {
            UnityEngine.Debug.Log("DefaultValueFunc: a=" + a + ",b=" + b + ",c=" + c);
        }

        public void VariableParamsFunc(int a, params string[] strs)
        {
            UnityEngine.Debug.Log("VariableParamsFunc: a =" + a);
            foreach (var str in strs)
            {
                UnityEngine.Debug.Log("str:" + str);
            }
        }

        public TestEnum EnumTestFunc(TestEnum e)
        {
            Debug.Log("EnumTestFunc: e=" + e);
            return TestEnum.E2;
        }

        public Action<string> TestDelegate = (param) =>
        {
            Debug.Log("TestDelegate in c#:" + param);
        };

        public event Action TestEvent;

        public void CallEvent()
        {
            TestEvent();
        }

        public ulong TestLong(long n)
        {
            return (ulong)(n + 1);
        }

        class InnerCalc : ICalc
        {
            public int add(int a, int b)
            {
                return a + b;
            }

            public int id = 100;
        }

        public ICalc GetCalc()
        {
            return new InnerCalc();
        }

        public void GenericMethod<T>()
        {
            Debug.Log("GenericMethod<" + typeof(T) + ">");
        }
    }

    [LuaCallCSharp]
    public interface ICalc
    {
        int add(int a, int b);
    }

    [LuaCallCSharp]
    public static class DerivedClassExtensions
    {
        public static int GetSomeData(this DerivedClass obj)
        {
            Debug.Log("GetSomeData ret = " + obj.DMF);
            return obj.DMF;
        }

        public static int GetSomeBaseData(this BaseClass obj)
        {
            Debug.Log("GetSomeBaseData ret = " + obj.BMF);
            return obj.BMF;
        }

        public static void GenericMethodOfString(this DerivedClass obj)
        {
            obj.GenericMethod<string>();
        }
    }
}
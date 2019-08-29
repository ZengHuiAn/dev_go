using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snake
{
    public class StartGame : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            LuaController.Instance.DoString("require ('start')");
        }

    }
}
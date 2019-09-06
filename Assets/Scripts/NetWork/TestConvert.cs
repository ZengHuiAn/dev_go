using Snake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestConvert : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        uint con = 19;
        Utils.utils.Log(bitConvert.GetBytes(con));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

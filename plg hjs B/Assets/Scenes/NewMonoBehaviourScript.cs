using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        int name = 3;


        int[] arraayName = new int[10];


        arraayName[0] = 10;
        arraayName[5] = 20;



        List<int> testList = new List<int>();
        testList.Add(5);
        testList.Add(10);
        testList.Add(15);

        testList[1] = 30;

        for (int i = 0; i < testList.Count; i++)
        {
            Debug.Log(testList[i]);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

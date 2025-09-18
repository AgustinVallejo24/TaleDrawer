using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class SpawnableManager : MonoBehaviour
{
    public SpawnableObjectDirectoryDictionary spawnableObjectDictionary;
    public Transform spawningPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}

[System.Serializable]


public enum SpawnableObjectType
{
    None,
    Caja,
    Soga,
    Character
}

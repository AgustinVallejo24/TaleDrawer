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
[Flags]
public enum SpawnableObjectType
{
    None = 0,
    Caja = 1 << 0,
    Soga = 1 << 1,
    Character = 1 << 2,
    All = ~0
}

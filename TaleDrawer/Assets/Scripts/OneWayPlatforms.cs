using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class OneWayPlatforms : MonoBehaviour
{    
    [SerializeField] PlatformEffector2D _myEffector;
    int _characterLayerIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _characterLayerIndex = LayerMask.NameToLayer("Character");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnDesable()
    {
        StartCoroutine(Desable());
    }

    IEnumerator Desable()
    {
        _myEffector.colliderMask &= ~(1 << _characterLayerIndex);
        yield return new WaitForSeconds(0.5f);
        _myEffector.colliderMask |= (1 << _characterLayerIndex);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent( out Character charac))
        {
            InputManager.instance.onGoingDown += OnDesable;            
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Character charac))
        {
            InputManager.instance.onGoingDown -= OnDesable;            
        }
    }
}

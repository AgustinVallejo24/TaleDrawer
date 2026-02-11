using System;
using UnityEngine;
using System.Linq;
using Unity.Cinemachine;
using System.Collections;
public class MonkeyActivatorManager : ActivatorManager
{
    [SerializeField] Rubies[] _rubies;
    [SerializeField] GameObject[] _monkeySprites;
    [SerializeField] CinemachineCamera _eventCamera;
    [SerializeField] CinemachineCamera _playerCamera;
    [SerializeField] SpecialFloor _floor;
    public override void OnActivation()
    {
        if (_rubies.Any())
        {

            _rubies[currentActivatorsOn - 1].shiningRuby.SetActive(true);
            _rubies[currentActivatorsOn - 1].grayRuby.SetActive(false);

        }

        if(currentActivatorsOn == activators.Length)
        {
            _monkeySprites[0].SetActive(false);
            _monkeySprites[1].SetActive(true);
        }
        
    }

    public void MonkeyHeadEvent()
    {
        StartCoroutine(CameraMovement());
    }
    public IEnumerator CameraMovement()
    {
        _playerCamera.enabled = false;
        _eventCamera.enabled = true;
        yield return new WaitForSeconds(2f);
        _floor.Activate();
        yield return new WaitForSeconds(2f);
        _playerCamera.enabled = true;
        _eventCamera.enabled = false;
        //yield return new WaitForSeconds(4f);
    }
}

[Serializable]

public struct Rubies
{
    [SerializeField] GameObject _obj;
    public GameObject shiningRuby;
    public GameObject grayRuby;
}

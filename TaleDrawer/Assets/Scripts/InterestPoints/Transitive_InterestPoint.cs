using Unity.Cinemachine;
using UnityEngine;
using System;
using System.Collections;

public class Transitive_InterestPoint : InterestPoint
{
    [SerializeField] Transform _target;
    [SerializeField] CinemachineTargetGroup _cameraTarget;
    [SerializeField] CinemachineFollow _playerCamera;
    [SerializeField] GameObject _currentZone;
    [SerializeField] GameObject _nextZone;
    [SerializeField] SpawnableManager _spawnableManager;
    [SerializeField] GameObject _pencil; 
    

    protected override void Start()
    {
        base.Start();
        pointEvent.AddListener(Transition);
    }

    void Transition()
    {
        _pencil.SetActive(false);
        _nextZone.SetActive(true);
        _cameraTarget.Targets[0].Object = _target;
        _spawnableManager.spawningPos = _target;
        StartCoroutine(PostTransition());
        
    }

    IEnumerator PostTransition()
    {
        yield return new WaitForSeconds(_playerCamera.GetMaxDampTime());
        _pencil.SetActive(true);
        Character.instance.SendInputToFSM(CharacterStates.Moving);
        yield return new WaitForSecondsRealtime(1f);
        _currentZone.SetActive(false);
    } 
}

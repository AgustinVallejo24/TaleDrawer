using Unity.Cinemachine;
using UnityEngine;

public class TransitiveInterestPoint : InterestPoint
{
    [SerializeField] Transform _target;
    [SerializeField] CinemachineTargetGroup _cameraTarget;
    [SerializeField] GameObject _currentZone;
    [SerializeField] GameObject _nextZone;
    [SerializeField] SpawnableManager _spawnableManager;
    

    protected override void Start()
    {
        base.Start();
        pointEvent.AddListener(Transition);
    }

    void Transition()
    {
        _nextZone.SetActive(true);
        _cameraTarget.Targets[0].Object = _target;
        _spawnableManager.spawningPos = _target;
        Character.instance.SendInputToFSM(CharacterStates.Moving);
        _currentZone.SetActive(false);
    }
}

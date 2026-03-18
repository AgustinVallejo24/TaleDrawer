using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
public class InitialCameraAnimation : MonoBehaviour
{
    [SerializeField] CinemachineBrain _cinemachineBrain;
    [SerializeField] CinemachineCamera[] _cameraSequence;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CameraSequence()
    {
        StartCoroutine(CameraSequenceCoroutine());
    }
    public IEnumerator CameraSequenceCoroutine()
    {
        _cameraSequence[0].enabled = true;
        for (int i = 1; i < _cameraSequence.Length; i++)
        {
            yield return new WaitForSeconds(2f);
            _cameraSequence[i].enabled = true;
            _cameraSequence[i-1].enabled = true;
        }
        yield return new WaitForSeconds(2f);
        Character.instance.SendInputToFSM(CharacterStates.Idle);
    }
}

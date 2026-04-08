using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.Events;
public class Activator : Puzzle
{
    public bool shouldLookAction;
    public bool shouldMakeCameraTransition;

    public CinemachineCamera lookCamera;

    [SerializeField] protected UnityEvent _activatorEvent;
    public virtual void Activation()
    {

    }

    public void LookAction()
    {
        GameManager.instance.StopAll();

        if (shouldMakeCameraTransition)
        {
            StartCoroutine(CameraMovement());
        }
        else
        {
            StartCoroutine(CameraChange());
        }
    }

    public IEnumerator CameraMovement()
    {
        lookCamera.enabled = true;
        GameManager.instance._playerCamera.enabled = false;
        yield return new WaitForSeconds(2f);
        _activatorEvent.Invoke();
        yield return new WaitForSeconds(1f);
        lookCamera.enabled = false;
        GameManager.instance._playerCamera.enabled = true;
        _activatorEvent.Invoke();
    }

    public IEnumerator CameraChange()
    {
        yield return null;
    }
}

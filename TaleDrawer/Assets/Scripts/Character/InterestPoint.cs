using UnityEngine;
using UnityEngine.Events;
public class InterestPoint : MonoBehaviour
{
    public UnityEvent pointEvent;
    public IEventObject eventObject;
    public bool changeDirection;
 
    protected virtual void Start()
    {
        if (eventObject != null)
            pointEvent.AddListener(eventObject.Event);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

using UnityEngine;

public class TriggerDialogue : MonoBehaviour
{
    public Dialogue dialogue;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Character character))
        {
            DialogManager.instance.StartDialogue(dialogue);
            Destroy(gameObject);
        }
    }
}

using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
       if(collision.TryGetComponent(out Character character))
        {
            SaveManager.instance.Save(transform);
            gameObject.SetActive(false);
        }
     
    }
}

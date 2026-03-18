using UnityEngine;

public class HintTrigger : MonoBehaviour
{
    [SerializeField] Hints hint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Character chara))
        {
            GameManager.instance.SaveHint(hint);
            Destroy(gameObject);
        }
    }
}

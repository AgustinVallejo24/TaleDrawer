using UnityEngine;
using UnityEngine.SceneManagement;
public class Projectile : MonoBehaviour
{

    public int sign;
    public float speed;
    void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        transform.position += Vector3.right * sign * Time.deltaTime * speed;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Character character))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (!collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}

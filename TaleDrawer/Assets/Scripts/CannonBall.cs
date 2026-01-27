using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public float speed;
    void Start()
    {
        GetComponent<Rigidbody2D>().AddForce(transform.right * speed, ForceMode2D.Impulse);
        Destroy(gameObject,4f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "20")
        {
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }
}

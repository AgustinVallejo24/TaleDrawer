using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
public class MetaBallSpawner : MonoBehaviour
{

    public GameObject ballPrefab;
    public Transform spawnPoint;
    public MetaballManager manager;

    [Header("Spawn Settings")]
    public float spawnRate = 0.02f;
    public int maxBalls = 150;
    public float initialForce = 2f;

    [Header("Lifetime")]
    public float maxDistance = 15f;

    float timer;

    List<Rigidbody2D> activeBalls = new List<Rigidbody2D>();
    Queue<Rigidbody2D> pool = new Queue<Rigidbody2D>();

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            SpawnBall();
            timer = 0f;
        }

        CleanupBalls();
    }

    void SpawnBall()
    {
        Rigidbody2D rb;

        if (pool.Count > 0)
        {
            rb = pool.Dequeue();
            rb.gameObject.SetActive(true);
        }
        else
        {
            GameObject obj = Instantiate(ballPrefab, spawnPoint);
            rb = obj.GetComponent<Rigidbody2D>();
        }

        rb.position = spawnPoint.position +(Vector3)Random.insideUnitCircle;
        rb.linearVelocity = Vector2.zero;

        // Empujoncito inicial (como manguera)
        rb.AddForce(spawnPoint.right * initialForce, ForceMode2D.Impulse);

        activeBalls.Add(rb);

        // Limitar cantidad
        if (activeBalls.Count > maxBalls)
        {
            ReturnBall(activeBalls[0]);
            activeBalls.RemoveAt(0);
        }

        UpdateManager();
    }

    void CleanupBalls()
    {
        for (int i = activeBalls.Count - 1; i >= 0; i--)
        {
            if (Vector2.Distance(activeBalls[i].position, spawnPoint.position) > maxDistance)
            {
                ReturnBall(activeBalls[i]);
                activeBalls.RemoveAt(i);
            }
        }

        UpdateManager();
    }

    void ReturnBall(Rigidbody2D rb)
    {
        rb.gameObject.SetActive(false);
        pool.Enqueue(rb);
    }

    void UpdateManager()
    {
        manager.balls = new GameObject[activeBalls.Count];

        for (int i = 0; i < activeBalls.Count; i++)
        {
            manager.balls[i] = activeBalls[i].gameObject;
        }
    }
}

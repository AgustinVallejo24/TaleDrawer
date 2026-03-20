using UnityEngine;
using System.Linq;
using System.Collections.Generic;
public class MetaballManager : MonoBehaviour
{
    public List<GameObject> balls;
    public Material material;
    public SpriteRenderer spriteRenderer;
    Vector4[] data;
    public float pullForce = 0.5f; // Reduced, as gravity will handle most downward pull; set to 0 if not needed
    public float pressureRadius = 0.6f;
    public float pressureForce = 4f; // Slightly increased for better anti-overlap
    public float cohesionRadius = 1.0f; // New: Radius for attraction (should be > pressureRadius)
    public float cohesionForce = 2f; // New: Strength of attraction to keep fluid cohesive
    List<Rigidbody2D>  bodies;
    Vector2 centerOfMass; // New: Dynamically computed each frame

    public float damping;

    public float radialOffset;
    public float radialSize;

    void Start()
    {
        data = new Vector4[balls.Count];
        bodies = new List<Rigidbody2D>();
        for (int i = 0; i < balls.Count; i++)
        {
            bodies.Add(balls[i].GetComponent<Rigidbody2D>());
            bodies[i].gravityScale = 1.3f; // Ensure gravity is enabled (default is 1, but explicit for safety)
            bodies[i].linearDamping = damping; // Add some drag to dampen oscillations and improve stability
        }
    }

    public List<Rigidbody2D> GetCloseDrops(int quantity, Vector2 pos)
    {
        return bodies.OrderBy(x => Vector2.Distance(x.position, pos)).Take(quantity).ToList();
    } 
    void FixedUpdate()
    {
        // New: Compute center of mass dynamically for cohesion
        centerOfMass = Vector2.zero;
        for (int i = 0; i < bodies.Count; i++)
        {
            centerOfMass += bodies[i].position;
        }
        centerOfMass /= bodies.Count;

        float pressureRadiusSqr = pressureRadius * pressureRadius;
        float cohesionRadiusSqr = cohesionRadius * cohesionRadius;

        for (int i = 0; i < bodies.Count; i++)
        {
            Rigidbody2D a = bodies[i];
            float customCohesionForce = 0;
            if(a.linearVelocity.magnitude > 1)
            {
                customCohesionForce = cohesionForce;
            }
            else
            {
                customCohesionForce = 0;
            }
            // Cohesion toward center of mass (gentle pull to keep fluid together)
            Vector2 dirCenter = centerOfMass - a.position;
            a.AddForce(dirCenter.normalized * pullForce); // Normalized for consistent strength

            for (int j = i + 1; j < bodies.Count; j++)
            {
                Rigidbody2D b = bodies[j];
                Vector2 dir = b.position - a.position;
                float distSqr = dir.sqrMagnitude;
                if (distSqr > cohesionRadiusSqr)
                    continue; // Too far, no interaction

                float dist = Mathf.Sqrt(distSqr);
                float strength = 0f;

                if (dist < pressureRadius)
                {
                    // Repulsion (pressure) when too close
                    strength = 1f - (dist / pressureRadius);
                    Vector2 force = dir / dist * strength * pressureForce;
                    a.AddForce(-force);
                    b.AddForce(force);
                }
                else if (dist < cohesionRadius)
                {
                    // New: Attraction (cohesion) at medium distance
                    strength = (dist - pressureRadius) / (cohesionRadius - pressureRadius);
                    Vector2 force = dir / dist * strength * customCohesionForce;
                    a.AddForce(force);
                    b.AddForce(-force);
                }
            }
        }
    }

    public void DestroyAllParticles()
    {
        var newList = new List<GameObject>(balls);
        foreach (var item in newList)
        {
            DestroyParticle(item);
        }
      
    }
    public void DestroyParticle(GameObject ball)
    {
        balls.Remove(ball);
        bodies.Remove(ball.GetComponent<Rigidbody2D>());
        Destroy(ball);
    }
    void Update()
    {
        float minY = float.MaxValue;
        float maxY = float.MinValue;
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        for (int i = 0; i < balls.Count; i++)
        {
            Vector3 pos = balls[i].transform.position;

            data[i] = new Vector4(pos.x, pos.y, 2f, 0);

            // -------- calcular limites reales --------
            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
        }

        Vector2 centerTop = new Vector2( (minX + maxX) * 0.5f,maxY +radialOffset);

        float radius = radialSize;

        spriteRenderer.material.SetInt("_BallCount", balls.Count);
        spriteRenderer.material.SetVectorArray("_Balls", data);

        // -------- NUEVO: enviar al shader --------
        spriteRenderer.material.SetFloat("_LiquidMinY", minY -.2f);
        spriteRenderer.material.SetFloat("_LiquidMaxY", maxY + 1f);

        //spriteRenderer.material.SetVector("_RadialCenter", centerTop);
        //spriteRenderer.material.SetFloat("_RadialRadius", radius);


    }
}
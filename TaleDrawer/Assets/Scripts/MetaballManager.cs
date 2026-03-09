using UnityEngine;

public class MetaballManager : MonoBehaviour
{
    public GameObject[] balls;
    public Material material;
    Vector4[] data;
    public float pullForce = 0.5f; // Reduced, as gravity will handle most downward pull; set to 0 if not needed
    public float pressureRadius = 0.6f;
    public float pressureForce = 4f; // Slightly increased for better anti-overlap
    public float cohesionRadius = 1.0f; // New: Radius for attraction (should be > pressureRadius)
    public float cohesionForce = 2f; // New: Strength of attraction to keep fluid cohesive
    Rigidbody2D[] bodies;
    Vector2 centerOfMass; // New: Dynamically computed each frame

    public float damping;

    void Start()
    {
        data = new Vector4[balls.Length];
        bodies = new Rigidbody2D[balls.Length];
        for (int i = 0; i < balls.Length; i++)
        {
            bodies[i] = balls[i].GetComponent<Rigidbody2D>();
            bodies[i].gravityScale = 1.3f; // Ensure gravity is enabled (default is 1, but explicit for safety)
            bodies[i].linearDamping = damping; // Add some drag to dampen oscillations and improve stability
        }
    }

    void FixedUpdate()
    {
        // New: Compute center of mass dynamically for cohesion
        centerOfMass = Vector2.zero;
        for (int i = 0; i < bodies.Length; i++)
        {
            centerOfMass += bodies[i].position;
        }
        centerOfMass /= bodies.Length;

        float pressureRadiusSqr = pressureRadius * pressureRadius;
        float cohesionRadiusSqr = cohesionRadius * cohesionRadius;

        for (int i = 0; i < bodies.Length; i++)
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

            for (int j = i + 1; j < bodies.Length; j++)
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

    void Update()
    {
        for (int i = 0; i < balls.Length; i++)
        {
            Vector3 pos = balls[i].transform.position;
            data[i] = new Vector4(pos.x, pos.y, 2f, 0); // Assuming radius is fixed at 2f; adjust if needed
        }
        material.SetInt("_BallCount", balls.Length);
        material.SetVectorArray("_Balls", data);
    }
}
using UnityEngine;

public class LightSource : MonoBehaviour
{
    public float range = 5f;
    public float intensity = 1f;
    public LayerMask obstacleMask;

    public float GetLight(Vector2 pos)
    {
        float dist = Vector2.Distance(pos, transform.position);
        if (dist > range) return 0f;

        RaycastHit2D hit = Physics2D.Raycast(
            pos,
            (transform.position - (Vector3)pos).normalized,
            dist,
            obstacleMask
        );

        if (hit.collider != null) return 0f;

        float atten = 1f - (dist / range);
        Debug.LogError(intensity * atten);
        return intensity * atten;
        
    }

}

using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
public class Wind : MonoBehaviour
{
    public GameObject windPrefab;
    public List<Transform> turrentSpots;
    Vector3[] _windpath; 
    Vector3[] _path;
    public LineRenderer lineRenderer;
    public Entity currentEntity;
    public bool movingEntity;

    public int loopPoints = 20;       
    public float loopRadius = 1f;
    public float loopTurns = 1f;
    void Start()
    {
        GeneratePath();
        _windpath = new Vector3[lineRenderer.positionCount];

        lineRenderer.GetPositions(_windpath);
        _path = new Vector3[turrentSpots.Count];
        for (int i = 0; i < _path.Length; i++)
        {
            _path[i] = turrentSpots[i].position;
        }
        StartCoroutine(SpawnWind());
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void GeneratePath()
    {
        LineRenderer lr = GetComponent<LineRenderer>();

        int baseCount = lr.positionCount;
        if (baseCount < 2) return;

        Vector2 last = lr.GetPosition(baseCount - 1);
        Vector2 prev = lr.GetPosition(baseCount - 2);

        // Dirección final
        Vector2 dir = (last - prev).normalized;

        // Ángulo de la dirección final
        float baseAngle = Mathf.Atan2(dir.y, dir.x);

        lr.positionCount = baseCount + loopPoints;

        for (int i = 0; i < loopPoints; i++)
        {
            float t = (float)i / (loopPoints - 1);
            float angle = t * Mathf.PI * 2f * loopTurns;

            // Círculo LOCAL (no depende del recorrido)
            Vector2 local =
                new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * loopRadius;

            // Rotar TODO el círculo una sola vez
            float cos = Mathf.Cos(baseAngle);
            float sin = Mathf.Sin(baseAngle);

            Vector2 rotated = new Vector2(
                local.x * cos - local.y * sin,
                local.x * sin + local.y * cos
            );

            lr.SetPosition(baseCount + i, last + rotated);
        }
    
}
    IEnumerator SpawnWind()
    {
        while (true)
        {
            var wind = Instantiate(windPrefab,transform);
            wind.transform.localPosition = _windpath[0];


            wind.transform.DOLocalPath(_windpath, 2f, PathType.CatmullRom);
            yield return new WaitForSeconds(.2f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Balloon balloon) && balloon.HasEntity() && !movingEntity)
        {
            Entity entity = balloon.GetCurrentEntity();
            movingEntity = true;
            entity.inWind = true;
            if(balloon.floatCoroutine!=null)
            StopCoroutine(balloon.floatCoroutine);
            entity.transform.DOPath(_path, 5f, PathType.CatmullRom).OnComplete(() =>
            {
                Debug.LogError("Me subo al viento");
                balloon.ActivateFloat();
                movingEntity = false;
            });
        }
        else if(collision.TryGetComponent(out Entity entity1))
        {
            entity1.inWind = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Entity entity1))
        {
            entity1.inWind = false;
        }
    }
}

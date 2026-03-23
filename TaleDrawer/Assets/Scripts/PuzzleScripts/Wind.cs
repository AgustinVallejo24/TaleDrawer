using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
public class Wind : MonoBehaviour
{
    public GameObject windPrefab;
    public List<Transform> turrentSpots;
    Vector3[] _windpath; 
    Vector3[] _path;
    public LineRenderer lineRenderer;
    public LineRenderer[] windPathLineRenderers;
    public Entity currentEntity;
    public bool movingEntity;

    public int loopPoints = 20;       
    public float loopRadius = 1f;
    public float loopTurns = 1f;

    public float pathDuration;

    public List<WindPath> windPaths;

    public bool playOnAwake = true;
    void Start()
    {
        if (playOnAwake)
        {
            ChangeWindPath(0);
            StartCoroutine(SpawnWind());
        }

       
    }

    public void ActivateWind()
    {
        ChangeWindPath(0);
        StartCoroutine(SpawnWind());
    }
    public void ChangeWindPath(int index)
    {
        GeneratePath(index);
        _windpath = new Vector3[windPaths[index].windLineRenderer.positionCount];

        windPaths[index].windLineRenderer.GetPositions(_windpath);
        _path = new Vector3[windPaths[index].turrentSpots.Count];
        for (int i = 0; i < _path.Length; i++)
        {
            _path[i] = windPaths[index].turrentSpots[i].position;
        }
        for (int i = 0; i < windPaths.Count; i++)
        {
            if (i != index)
            {
                windPaths[i].collider.enabled = false;
            }
            else
            {
                windPaths[i].collider.enabled = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            ChangeWindPath(1);
        }
    }
    void GeneratePath(int index)
    {
        LineRenderer lr = windPaths[index].windLineRenderer;

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
        if(collision.TryGetComponent(out Balloon balloon))
        {
            Debug.LogError("LaPutaMadre");
            if (balloon.HasEntity() && !movingEntity)
            {
                Entity entity = balloon.GetCurrentEntity();
                movingEntity = true;
                entity.inWind = true;
                if (balloon.floatCoroutine != null)
                    StopCoroutine(balloon.floatCoroutine);
                var orderPath = _path.OrderBy(x => Vector2.Distance((Vector2)x, (Vector2)Character.instance.transform.position)).ToList();
                var newPath = _path.ToList();
                for (int i = Mathf.Min(_path.ToList().IndexOf(orderPath[0]), _path.ToList().IndexOf(orderPath[1])); i > 0; i--)
                {
                    newPath.RemoveAt(i);
                }
                entity.transform.DOPath(newPath.ToArray(), newPath.Count / pathDuration).SetEase(Ease.Linear).OnComplete(() =>
                {
                    Debug.LogError("Me subo al viento");
                    balloon.ActivateFloat();
                    movingEntity = false;
                });
            }
            else
            {
                balloon.onWind = true;
                balloon.OnWind();
                var orderPath = _path.OrderBy(x => Vector2.Distance((Vector2)x, (Vector2)balloon.transform.position)).ToList();
                var newPath = _path.ToList();
                for (int i = Mathf.Min(_path.ToList().IndexOf(orderPath[0]), _path.ToList().IndexOf(orderPath[1])); i > 0; i--)
                {
                    newPath.RemoveAt(i);
                }
                balloon.transform.DOPath(newPath.ToArray(), newPath.Count / pathDuration).SetEase(Ease.Linear).OnComplete(() => balloon.onWind = false);
            }
            
        }
        if (collision.TryGetComponent(out Umbrella umbrella) )
        {
       
            Entity entity = umbrella.GetCurrentEntity();
            entity.entityRigidbody.linearVelocity = Vector2.zero;
            movingEntity = true;
            entity.inWind = true;
            var orderPath = _path.OrderBy(x => Vector2.Distance((Vector2)x, (Vector2)Character.instance.transform.position)).ToList();
            var newPath = _path.ToList();
            for (int i = Mathf.Min(_path.ToList().IndexOf(orderPath[0]), _path.ToList().IndexOf(orderPath[1])); i >= 0; i--)
            {
                newPath.RemoveAt(i);
            }

            entity.transform.DOPath(newPath.ToArray(), newPath.Count/pathDuration, PathType.CatmullRom).SetEase(Ease.Linear).OnComplete(() =>
            {
                Debug.LogError("Me subo al viento");
                Character.instance.AddGlideVelocity((_path[_path.Length - 1] - _path[_path.Length - 2]).normalized * 10f);
                 movingEntity = false;


            }).OnKill(() => movingEntity = false);
        }
        if (collision.TryGetComponent(out Entity entity1))
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
[System.Serializable]
public struct WindPath
{
    public LineRenderer windLineRenderer;

    public List<Transform> turrentSpots;

    public Collider2D collider;
}
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
    void Start()
    {
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

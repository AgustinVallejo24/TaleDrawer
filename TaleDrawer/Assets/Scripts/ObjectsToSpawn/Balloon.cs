using UnityEngine;
using System.Collections;
public class Balloon : SpawningObject
{
    [Header("Subida inicial")]
    public float riseHeight = 0.4f;
    public float riseDuration = 0.6f;

    [Header("Vaivén")]
    public float amplitude = 0.25f;
    public float speed = 2f;

    private Vector3 basePosition;
    public bool floating = false;
    public bool onWind;
    public Coroutine floatCoroutine;
    public bool HasEntity()
    {
        return _currentEntity != null;
    }

    public Entity GetCurrentEntity()
    {
        return _currentEntity;
    }
    // Llamá a esto cuando el jugador interactúe
    public void ActivateFloat()
    {
        floatCoroutine = StartCoroutine(RiseAndFloat());
    }

    public override void Paint()
    {
        StartCoroutine(GetComponentInChildren<Paint>().PaintSprite());
    }
    public override void Delete()
    {
        base.Delete();
        _currentEntity.ReleaseFromBalloon();
        Destroy(gameObject);
    }
    IEnumerator RiseAndFloat()
    {
        Vector3 startPos = _currentEntity.transform.position;
        Vector3 endPos = startPos + Vector3.up * riseHeight;

        float elapsed = 0f;
        // Subida suave

        while (elapsed < riseDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / riseDuration;

            _currentEntity.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        // Guardamos la nueva base
        basePosition = _currentEntity.transform.position;
        floating = true;
    }

    protected virtual void Update()
    {
        if (!floating) return;

        float yOffset = Mathf.Sin(Time.time * speed) * amplitude;
        _currentEntity.transform.position = basePosition + new Vector3(0f, yOffset, 0f);
    }

    public virtual void OnWind()
    {

    }

    public override void InteractionWithEntity()
    {
        _currentEntity.LiftEntity();
        _currentEntity.currentBalloon = this;
        transform.position = _currentEntity.balloonPosition.position;
        _mySpriteRenderer.sortingOrder = -1;
        transform.parent = _currentEntity.transform;
        if(!_currentEntity.inWind)
        ActivateFloat();
    }


    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
       // base.OnTriggerEnter2D(collision);
        if (floating && collision.gameObject.tag == "Spikes")
        {
            _currentEntity.ReleaseFromBalloon();
            Delete();
        }
    }
}

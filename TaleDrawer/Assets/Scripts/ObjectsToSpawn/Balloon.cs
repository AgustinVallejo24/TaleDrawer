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
    private bool floating = false;



    // Llamá a esto cuando el jugador interactúe
    public void ActivateFloat()
    {
        StartCoroutine(RiseAndFloat());
    }

    public override void Delete()
    {
        base.Delete();
        _myCharacter.characterRigidbody.gravityScale = 1;
        _myCharacter.SendInputToFSM(CharacterStates.Idle);
        Destroy(gameObject);
    }
    IEnumerator RiseAndFloat()
    {
        Vector3 startPos = _myCharacter.transform.position;
        Vector3 endPos = startPos + Vector3.up * riseHeight;

        float elapsed = 0f;
        // Subida suave

        while (elapsed < riseDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / riseDuration;

            _myCharacter.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        // Guardamos la nueva base
        basePosition = _myCharacter.transform.position;
        floating = true;
    }

    void Update()
    {
        if (!floating) return;

        float yOffset = Mathf.Sin(Time.time * speed) * amplitude;
        _myCharacter.transform.position = basePosition + new Vector3(0f, yOffset, 0f);
    }


    public override void InteractionWithPlayer()
    {
        _myCharacter = Character.instance;
        _myCharacter.SendInputToFSM(CharacterStates.Stop);
        _myCharacter.characterRigidbody.gravityScale = 0;
        transform.position = _myCharacter.balloonPosition.position;
        transform.parent = _myCharacter.transform;
        ActivateFloat();
    }

}

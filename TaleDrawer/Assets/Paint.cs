using UnityEngine;
using System.Collections;
using DG.Tweening;
public class Paint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    SpriteRenderer _myRenderer;
    public GameObject cloudParticleSystem;

    public float firstStripeInitialValue;
    void Start()
    {
        //_myRenderer = GetComponent<SpriteRenderer>();
        //Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        //Texture2D tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        //_myRenderer.material.SetTexture("_MainTex", tex);
        //StartCoroutine(PaintSprite());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator PaintSprite()
    {
        _myRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = _myRenderer.sprite;

        Texture2D tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        _myRenderer.material.SetTexture("_MainTex", tex);

        //_myRenderer.material.DOFloat(.6f, "_FirstStripe", 1.3f).SetUpdate(true);
        //yield return new WaitForSecondsRealtime(1f);

        //_myRenderer.material.DOFloat(-.5f, "_SecondStripe", 1.3f).SetUpdate(true);
        //yield return new WaitForSecondsRealtime(1.3f);

        //_myRenderer.material.DOFloat(0f, "_ThirdStripe", 1.3f).SetUpdate(true);
        //yield return new WaitForSecondsRealtime(1.3f);
        _myRenderer.material.DOFloat(1.4f, "_MaskValue", .7f).SetUpdate(true);
        yield return new WaitForSecondsRealtime(.7f);
        GameManager.instance.StateChanger(SceneStates.Dragging);

        transform.gameObject.layer = 7;

    }
}

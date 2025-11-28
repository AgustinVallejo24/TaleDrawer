using UnityEngine;
using System.Collections;
using DG.Tweening;
public class Paint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    SpriteRenderer _myRenderer;
    void Start()
    {
        _myRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(PaintSprite());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator PaintSprite()
    {

        _myRenderer.material.DOFloat(.6f, "_FirstStripe", 1.3f);
        yield return new WaitForSeconds(1f);
        _myRenderer.material.DOFloat(-.5f, "_SecondStripe", 1.3f);
        yield return new WaitForSeconds(1.3f);
        _myRenderer.material.DOFloat(0, "_ThirdStripe", 1.3f);
    }
}

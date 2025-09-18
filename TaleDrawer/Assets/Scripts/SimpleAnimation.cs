

using System.Collections;
using UnityEngine;

public class SimpleAnimation : MonoBehaviour
{
    [SerializeField] SpriteRenderer _sprite;
    [SerializeField] Sprite[] _textures;
    public bool isActive = true;
    [SerializeField] float _delay;
    void Start()
    {
        StartCoroutine(SwitchImages());
    }

    IEnumerator SwitchImages()
    {
        int imageIndex = 0;
        while (isActive)
        {
            yield return new WaitForSeconds(.2f);
            imageIndex++;
            if(imageIndex >= _textures.Length)
            {
                _sprite.sprite = _textures[0];
                imageIndex = 0;
            }
            else
            {
                _sprite.sprite = _textures[imageIndex];
            }
        }
    }
}

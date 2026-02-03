using UnityEngine;
using UnityEngine.UI;
public class BookPage : MonoBehaviour
{
    [SerializeField] Image[] _images;
    public int index;


    public void FillImages(Sprite sprite)
    {
        _images[index].sprite = sprite;
        _images[index].color = Color.white;
        index++;
    }
    public bool IsFull()
    {
        return index == _images.Length;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}

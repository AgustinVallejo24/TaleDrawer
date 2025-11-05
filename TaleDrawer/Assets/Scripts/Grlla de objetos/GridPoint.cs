using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
public class GridPoint : MonoBehaviour
{
    public SpriteRenderer _myRend;
    [SerializeField] Color _originalColor;
    [SerializeField] Color _selectedColor;
    [SerializeField] Color _blockedColor;
    [SerializeField] int _originalOrderInLayer;
    [SerializeField] int _visibleOrderInLayer;

    //[SerializeField] bool _blocked;
    

    private void Start()
    {                
        _originalOrderInLayer = _myRend.sortingOrder;
        //_blocked = false;
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SelectAndDeselectPoint(bool paint)
    {
        if (paint)
        {
            _myRend.color = _selectedColor;
        }
        else
        {
            _myRend.color= _originalColor;
        }
    }

    public void SetAvailabilityColor(bool available)
    {
        if (!available)
        {
            _myRend.color = _blockedColor;
            //_blocked = true;
        }
        else
        {
            _myRend.color =_originalColor;
            //_blocked = false;
        }
    }

    public void SetVisibility(bool visible)
    {
        if (visible)
        {
            _myRend.sortingOrder = _visibleOrderInLayer;
        }
        else
        {
            _myRend.sortingOrder = _originalOrderInLayer;
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
public class TemplateBook : MonoBehaviour
{
    [SerializeField] List<RectTransform> _pages;
    [SerializeField] RectTransform _pageParent;
    [SerializeField] RectTransform _coverPage;
    [SerializeField] ZernikeManager _manager;
    [SerializeField] BookPage _pagePrefab;
    BookPage _currentPage;
    [SerializeField] int index = -1;
    [SerializeField] NewSerializableDictionary<SpawnableObjectType, Sprite> _typesAndImages;

    private void Start()
    {

    }
    public void OnActivated()
    {
        foreach (var item in _manager.referenceSymbolsList)
        {
            if (_currentPage == null || _currentPage.IsFull())
            {
                _currentPage = Instantiate(_pagePrefab, _pageParent);
                _pages.Add(_currentPage.GetComponent<RectTransform>());
                _currentPage.transform.SetAsFirstSibling();
      
            }
            _currentPage.FillImages(_typesAndImages[item.objectType]);

        }
        _coverPage.transform.SetAsLastSibling();
    }

    public void OnDeactivated()
    {
        int count = _pages.Count;
        for (int i = _pages.Count -1 ; i > 0; i--)
        {
            var page = _pages[i];
            _pages.Remove(page);
            Destroy(page.gameObject);
        }
        index = -1;
        _coverPage.localEulerAngles = Vector3.zero;
    }
    public void RotateForward()
    {
        if (index == -1) return;
        else
        {
            _pages[index].SetAsLastSibling();
            _pages[index].DOLocalRotate(new Vector3(0, 0, 0), .6f).SetEase(Ease.InQuad);
            index--;
        }
    }
    public void RotateBackwards()
    {
        if (index == _pages.Count - 1) return;
        else
        {
           
            index++;
            _pages[index].SetAsLastSibling();
            _pages[index].DOLocalRotate(new Vector3(0, 180, 0), .6f).SetEase(Ease.InQuad);
        }

    }
}

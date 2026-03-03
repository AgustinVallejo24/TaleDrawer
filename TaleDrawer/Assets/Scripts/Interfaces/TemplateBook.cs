using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using System.Linq;
public class TemplateBook : MonoBehaviour
{
    [SerializeField] List<RectTransform> _pages;
    [SerializeField] RectTransform _pageParent;
    [SerializeField] RectTransform _coverPage;
    [SerializeField] RectTransform _hintsCoverPage;
    [SerializeField] ZernikeManager _manager;
    [SerializeField] BookPage _pagePrefab;
    BookPage _currentSymbolPage;
    BookPage _currentHintPage;
    [SerializeField] int index = -1;
    [SerializeField] NewSerializableDictionary<SpawnableObjectType, Sprite> _typesAndImages;
    [SerializeField] NewSerializableDictionary<Hints, Sprite> _hintsAndImages;

    public void OnActivated()
    {
        foreach (var item in _manager.referenceSymbolsList)
        {
            if (_currentSymbolPage == null || _currentSymbolPage.IsFull())
            {
                _currentSymbolPage = Instantiate(_pagePrefab, _pageParent);
                _pages.Add(_currentSymbolPage.GetComponent<RectTransform>());
                _currentSymbolPage.transform.SetAsFirstSibling();
      
            }
            _currentSymbolPage.FillImages(_typesAndImages[item.objectType]);


        }
            _pages.Add(_hintsCoverPage.GetComponent<RectTransform>());
            _hintsCoverPage.transform.SetAsFirstSibling();
            var hints = SaveSystem.Load().hubData.hints.Where(x => x.unlocked);
            foreach (var item in hints)
            {
                if (_currentHintPage == null || _currentHintPage.IsFull())
                {
                    _currentHintPage = Instantiate(_pagePrefab, _pageParent);
                    _pages.Add(_currentHintPage.GetComponent<RectTransform>());
                    _currentHintPage.transform.SetAsFirstSibling();

                }
                _currentHintPage.FillImages(_hintsAndImages[item.hint]);

            }
        
        _coverPage.transform.SetAsLastSibling();
    }

    public void OnDeactivated()
    {
        int count = _pages.Count;
        for (int i = _pages.Count -1 ; i > 0; i--)
        {
            if (_pages[i] == _hintsCoverPage) continue; 
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

    public void GoToHints()
    {
        int hintsIndex = _pages.IndexOf(_hintsCoverPage.GetComponent<RectTransform>());
        if (index > hintsIndex)
        {
            StartCoroutine(MovePages(index, hintsIndex, -1));
        }
        else
        {
            StartCoroutine(MovePages(index, hintsIndex, 1));
        }
    }
    public void GoToSymbols()
    {
        int hintsIndex = _pages.IndexOf(_coverPage.GetComponent<RectTransform>());
        if (index > hintsIndex)
        {
            StartCoroutine(MovePages(index, hintsIndex, -1));
        }
        else
        {
            StartCoroutine(MovePages(index, hintsIndex, 1));
        }
    }

    public IEnumerator MovePages(int startValue, int endValue, int sign)
    {
        if (sign < 0)
        {
            for (int i = startValue; i >= endValue; i--)
            {
                index = i;
                _pages[i].SetAsLastSibling();
                _pages[i].DOLocalRotate(new Vector3(0, 0, 0), .3f).SetEase(Ease.InQuad);
                yield return new WaitForSeconds(.3f);
            }
            index--;
        }
        else
        {
            int newStartValue = startValue + 1;
            for (int i = newStartValue; i < endValue; i++)
            {
                index = i;
                _pages[i].SetAsLastSibling();
                _pages[i].DOLocalRotate(new Vector3(0, 180, 0), .3f).SetEase(Ease.InQuad);
                yield return new WaitForSeconds(.3f);
            }
        }

    }
}
public enum Hints
{
    Umbrella,
    Bait,
    Boleadora,

}
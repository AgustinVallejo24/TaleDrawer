using System;
using UnityEngine;
using System.Linq;

public class MonkeyActivatorManager : ActivatorManager
{
    [SerializeField] Rubies[] _rubies;
    [SerializeField] GameObject[] _monkeySprites;

    public override void OnActivation()
    {
        if (_rubies.Any())
        {

            _rubies[currentActivatorsOn - 1].shiningRuby.SetActive(true);
            _rubies[currentActivatorsOn - 1].grayRuby.SetActive(false);

        }

        if(currentActivatorsOn == activators.Length)
        {
            _monkeySprites[0].SetActive(false);
            _monkeySprites[1].SetActive(true);
        }
        
    }
}

[Serializable]

public struct Rubies
{
    [SerializeField] GameObject _obj;
    public GameObject shiningRuby;
    public GameObject grayRuby;
}

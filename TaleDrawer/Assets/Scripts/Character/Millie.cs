using UnityEngine;
using UnityEngine.UI;
public class Millie : Character
{
    [SerializeField] Image _fearProgressBar;
    [SerializeField] LightSource[] lights;

    public float ambientLight = 0.2f;
    [SerializeField] float totalFear;
    [SerializeField] float fear;
    [SerializeField] float fearFactor;
    [SerializeField] float safeLightValue;
    [SerializeField] float midLightValue;
    [SerializeField] float dangerLightValue;
    protected override void Awake()
    {
        characterModel = new CharacterModel(this, characterRigidbody, floorLayerMask);
        characterView = new CharacterView(this, _animator, _characterSprite);
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }
    public override void Update()
    {
        base.Update();
        Fear();
    }
    public void Fear()
    {
        float directLight = 0f;

        foreach (var l in lights)
            directLight += l.GetLight(transform.position);

        float exposure = ambientLight + directLight;
        float targetFear = 1f - Mathf.Clamp01(directLight);
        if (directLight >= safeLightValue)
        {
            fearFactor = -.25f*Mathf.Clamp(directLight, 0.2f, 1);
        }
        else
        {
            fearFactor = 0.05f / Mathf.Clamp(directLight, 0.2f, 1);
        }

        fear +=  Time.deltaTime  * fearFactor;
        fear = Mathf.Clamp01(fear);
        UpdateFearUI();

        if(fear == 1)
        {
            Death();
        }
    }

    public void UpdateFearUI()
    {
        _fearProgressBar.fillAmount = fear;
    }
}

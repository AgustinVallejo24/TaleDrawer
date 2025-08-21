using UnityEngine;

public class Robin : Character
{
    protected override void Awake()
    {
        _characterModel = new CharacterModel(this, _characterRigidbody);
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();

    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}

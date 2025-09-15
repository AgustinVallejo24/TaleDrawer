using UnityEngine;

public class Robin : Character
{

    
    protected override void Awake()
    {
        characterModel = new CharacterModel(this, _characterRigidbody);
        characterView = new CharacterView(this, _animator, _characterSprite);
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

using UnityEngine;

public class CharacterView
{
    Character _character;
    Animator _anim;
    SpriteRenderer _characterSprite;

    [Header("Anim References")]
    private string _runTrigger = "Run";
    private string _idleTrigger = "Idle";
    private string _jumpTrigger = "Jump";
    public CharacterView(Character character, Animator anim, SpriteRenderer characterSprite)
    {
        _character = character;
        _anim = anim;
        _characterSprite = characterSprite;        
    }

    public void OnMove()
    {
        _anim.SetInteger("MovementState", 1);
        _anim.SetTrigger(_runTrigger);
    }

    public void OnIdle()
    {
        _anim.SetInteger("MovementState", 0);
        _anim.SetTrigger(_idleTrigger);
    }

    public void FlipCharacter()
    {
        _characterSprite.flipX = !_characterSprite.flipX;
    }

    public void OnJump()
    {
        _anim.SetTrigger(_jumpTrigger);
    }
}

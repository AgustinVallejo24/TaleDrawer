using UnityEngine;
using System.Collections;
public class CharacterView
{
    Character _character;
    Animator _anim;
    SpriteRenderer _characterSprite;

    [Header("Anim References")]
    private string _runTrigger = "Run";
    private string _idleTrigger = "Idle";
    private string _jumpTrigger = "Jump";
    private string _landTrigger = "Land";
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
        _character.StartCoroutine(CustomResetTrigger(_runTrigger));
    }

    public void OnIdle()
    {
        _anim.SetInteger("MovementState", 0);
        _anim.SetTrigger(_idleTrigger);
        _character.StartCoroutine(CustomResetTrigger(_idleTrigger));
    }

    public void OnLand()
    {
        _anim.SetTrigger(_landTrigger);
        _character.StartCoroutine(CustomResetTrigger(_landTrigger));
    }

    public void OnClimb()
    {
        _anim.SetTrigger("Climb");
        _character.StartCoroutine(CustomResetTrigger("Climb"));
    }
    public void FlipCharacter(int movementSign)
    {
        if(movementSign > 0)
        {
            _characterSprite.flipX = false;
        }
        else
        {
            _characterSprite.flipX = true;
        }
         
    }

    public void OnJump()
    {
        _anim.SetTrigger(_jumpTrigger); 
        _character.StartCoroutine(CustomResetTrigger(_jumpTrigger));
        
    }

    public void OnJumpingToRope()
    {

    }

    public void OnRopeClimbing()
    {

    }

    public void OnWaitingForRopeMovement()
    {

    }

    public void OnExitingRope()
    {

    }

    public void OnEquippingHelmet()
    {

    }

    public IEnumerator CustomResetTrigger(string triggerName)
    {
        yield return new WaitForSeconds(.4f);
        _anim.ResetTrigger(triggerName);
    }

}

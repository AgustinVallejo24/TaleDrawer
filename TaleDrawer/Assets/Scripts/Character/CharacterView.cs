using UnityEngine;
using System.Collections;
using DG.Tweening;
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
    private string _idleClimbTrigger = "IdleClimb";
    private string _verticalRopeEventTrigger = "VerticalRopeEventMovement";
    private string _horizontalRopeEventTrigger = "HorizontalRopeEventMovement";
    private string _jumpingToRopeTrigger = "JumpingToRope";
    private string _exitingHorizontalRopeTrigger = "ExitingHorizontalRope";    
    private string _enteringLadder = "Ladder";
    private string _eventMovement = "EventMovement";
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
    public void OnDeath()
    {
        _anim.SetTrigger("Death");
        _character.StartCoroutine(CustomResetTrigger("Death"));
    }
    public void OnIdle()
    {
        _anim.SetInteger("MovementState", 0);
        _anim.SetTrigger(_idleTrigger);
        _character.StartCoroutine(CustomResetTrigger(_idleTrigger));
    }

    public void OnIdleClimb()
    {
        _anim.SetTrigger(_idleClimbTrigger);
        //_character.StartCoroutine(CustomResetTrigger(_idleClimbTrigger));
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

    public void BallonAnimation()
    {
        _anim.SetTrigger("Balloon");
    }
    public void FlipCharacter(int movementSign)
    {

        _character.flipSign = movementSign;
        if(movementSign > 0)
        {
            Vector3 previousOffset = _character.cameraFollow.FollowOffset;
            previousOffset.x = 2;
            DOTween.To(
            () => _character.cameraFollow.FollowOffset,
            x => _character.cameraFollow.FollowOffset = x,
            previousOffset,
            1.5f
        ).SetEase(Ease.OutCubic);
            _characterSprite.flipX = false;
        }
        else
        {
            Vector3 previousOffset = _character.cameraFollow.FollowOffset;
            previousOffset.x = -2;
            DOTween.To(
            () => _character.cameraFollow.FollowOffset,
            x => _character.cameraFollow.FollowOffset = x,
            previousOffset,
            1.5f
        ).SetEase(Ease.OutCubic);
            _characterSprite.flipX = true;            
        }
         
    }

    public void OnJump()
    {
        _anim.SetTrigger(_jumpTrigger); 
        _character.StartCoroutine(CustomResetTrigger(_jumpTrigger));
        
    }    

    public void OnVerticalRopeEventMovement()
    {
        _anim.SetTrigger(_verticalRopeEventTrigger);
    } 
    
    public void OnHorizontalRopeMovement()
    {
        _anim.SetTrigger(_horizontalRopeEventTrigger);
    }

    public void OnJumpingToRope()
    {
        _anim.SetTrigger(_jumpingToRopeTrigger);
    }

    public void OnExitingHorizontalRope()
    {
        _anim.SetTrigger(_exitingHorizontalRopeTrigger);
    }

    public void OnEquippingHelmet()
    {

    }
    public void OnEnteringLadder()
    {
        _anim.SetTrigger(_enteringLadder);        
    }

    public void OnEventMovement()
    {
        _anim.SetTrigger(_eventMovement);
        _character.StartCoroutine(CustomResetTrigger(_eventMovement));
    }

    public IEnumerator CustomResetTrigger(string triggerName)
    {
        yield return new WaitForSeconds(.4f);
        _anim.ResetTrigger(triggerName);
    }

}

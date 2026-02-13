using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Ladders : MonoBehaviour, IInteractableP
{
    [SerializeField] Transform _upperPoint;
    [SerializeField] Transform _lowerPoint;
    [SerializeField] float _movementDuration;
    [SerializeField] CustomNode _upperNode;
    [SerializeField] CustomNode _lowerNode;
    [SerializeField] Character _character;
    [SerializeField] bool _fromAbove;
    [SerializeField] InteractableType _interactableType;
    [SerializeField] Transform[] _accesPoints;
    [SerializeField] Transform[] _bodyPieces;
    [SerializeField] List<float> _bodyPos;
    [SerializeField] bool _rolledUp;
    [SerializeField] Collider2D _lowerCollider;
    bool first = true;
    [SerializeField] GameObject _sKey;
    [SerializeField] GameObject _eKey;
    [SerializeField] GameObject _wKey;
    public bool hasCharacter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_rolledUp)
        {
            _bodyPieces = _bodyPieces.OrderByDescending(x => x.position.y).ToArray();

            _bodyPieces.Last().parent = _bodyPieces.SkipLast(1).Last();

            Transform previous = null;

            foreach (var item in _bodyPieces)
            {
                if (item == _bodyPieces.Last()) break;

                if (previous != null)
                {
                    item.parent = previous;
                }

                _bodyPos.Add(item.position.y);
                item.position = _bodyPieces[0].position;
                
                previous = item;
            }

            _lowerCollider.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Interact()
    {
        if (_rolledUp && first && _bodyPieces.Any())
        {
            first = false;
            Sequence sequence = DOTween.Sequence();

            foreach (var item in _bodyPieces)
            {
                if (item != _bodyPieces.First() && item != _bodyPieces.Last())
                {
                    sequence.Append(item.DOMoveY(_bodyPos[Array.IndexOf(_bodyPieces, item)], 0.07f));
                }

            }

            sequence.Play().OnComplete(() => { _rolledUp = false; _lowerCollider.enabled = true; });
        }
        else if (!_rolledUp)
        {
            Character.instance.HideKeyUI();
            hasCharacter = true;
            Character.instance.currentInteraction = this;
            Character.instance.climbingSpeedMultiplier = 1;
            Character.instance.maxClimbingPos = _upperPoint.position;
            Character.instance.minClimbingPos = _lowerPoint.position;
            Transform nearestPoint = _accesPoints.OrderBy(x => Vector2.Distance(CustomTools.ToVector2(Character.instance.transform.position), CustomTools.ToVector2(x.position))).First();
            Character.instance.SendInputToFSM(CharacterStates.Wait);
            Character.instance.characterModel.Flip(nearestPoint.position);
            Character.instance.characterView.OnEventMovement();


            Character.instance.transform.DOMoveX(nearestPoint.position.x, 0.2f).OnComplete(() => { Character.instance.SendInputToFSM(CharacterStates.OnLadder); Character.instance.characterView.OnEnteringLadder(); });

        }
    }

   
    public void StartLadderMovement()
    {

        StartCoroutine(IGetOnLadder());
        
    }

    public IEnumerator IGetOnLadder()
    {

        _character.SendInputToFSM(CharacterStates.Wait);
        _character.SetAnimatorTrigger("Ladder");
        yield return null;

        LadderMovement();
    }

    private void LadderMovement()
    {
        _character.SendInputToFSM(CharacterStates.OnLadder);
        _character.characterRigidbody.gravityScale = 0;

    }

    public InteractableType MyInteractableType()
    {
        return _interactableType;
    }

    
    public void OnLeavingInteraction()
    {
        Character.instance.currentInteraction = null;
        hasCharacter = false;
    }

    public KeyCode InteractionKey()
    {
        if (hasCharacter) return KeyCode.None;

        if (Vector2.Distance(Character.instance.transform.position, _lowerPoint.position) <= Vector2.Distance(Character.instance.transform.position, _upperPoint.position))
        {
            return KeyCode.W;
        }
        else
        {
            if (!_rolledUp) return KeyCode.S;
            else return KeyCode.E;
        }
    }
}

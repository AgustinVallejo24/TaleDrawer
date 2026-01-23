using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Ladders : MonoBehaviour, IInteractable
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

    public void InsideInteraction()
    {
        
    }

    public void Interact(SpawnableObjectType objectType, GameObject interactor)
    {
        
    }

    public void Interact(SpawningObject spawningObject)
    {
        
    }

    public void Interact(GameObject interactor)
    {
        
    }

    public void InteractWithPlayer()
    {
        if (_rolledUp && first)
        {
            first = false;
            Sequence sequence = DOTween.Sequence();

            foreach (var item in _bodyPieces)
            {
                if (item != _bodyPieces.First() && item != _bodyPieces.Last())
                {
                    sequence.Append(item.DOMoveY(_bodyPos[Array.IndexOf(_bodyPieces, item)], 0.2f));
                }

            }

            sequence.Play().OnComplete(() => { _rolledUp = false; _lowerCollider.enabled = true; });
        }
        else if(!_rolledUp)
        {
            Character.instance.climbingSpeedMultiplier = 1;
            Character.instance.maxClimbingPos = _upperPoint.position;
            Character.instance.minClimbingPos = _lowerPoint.position;
            Transform nearestPoint = _accesPoints.OrderBy(x => Vector2.Distance(CustomTools.ToVector2(Character.instance.transform.position), CustomTools.ToVector2(x.position))).First();
            Character.instance.SendInputToFSM(CharacterStates.Wait);
            Character.instance.characterModel.Flip(nearestPoint.position);
            Character.instance.characterView.OnEventMovement();


            Character.instance.transform.DOMoveX(nearestPoint.position.x, 0.2f).OnComplete(() => { Character.instance.SendInputToFSM(CharacterStates.OnLadder); Character.instance.characterView.OnEnteringLadder(); });
            
        }
        

        //if (_character.GetPathList(_upperNode).Count >= _character.GetPathList(_lowerNode).Count)
        //{
        //    Debug.LogError("A");
        //    Vector3 pos = (new Vector3(_character.transform.position.x, _upperNode.transform.position.y, 0) - _upperNode.transform.transform.position).normalized;
        //    _fromAbove = false;
        //    _character.GetPath(_upperNode);
        //    _character.SendInputToFSM(CharacterStates.Moving);
        //}
        //else
        //{
        //    Debug.LogError("B");
        //    Vector3 pos = (new Vector3(_character.transform.position.x, _lowerNode.transform.position.y, 0) - _lowerNode.transform.transform.position).normalized;
        //    _fromAbove = true;
        //    _character.GetPath(_lowerNode);
        //    _character.SendInputToFSM(CharacterStates.Moving);
        //}
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

        /*if (_fromAbove)
        {
            _character.characterView.OnEnteringLadder(1);
            _character.transform.DOMoveY(_lowerPoint.position.y, _movementDuration)
                .OnComplete(() => { _character.transform.position = _lowerNode.transform.position; _character.characterRigidbody.gravityScale = 3; 
                    StartCoroutine(_character.SendInputToFSM(CharacterStates.Moving, 0.2f)); _character.SetAnimatorTrigger("Idle"); });
        }
        else
        {
            _character.characterView.OnEnteringLadder(0);
            _character.transform.DOMoveY(_upperPoint.position.y, _movementDuration)
                .OnComplete(() => { _character.transform.position = _upperNode.transform.position; _character.characterRigidbody.gravityScale = 3; 
                    StartCoroutine(_character.SendInputToFSM(CharacterStates.Moving, 0.2f)); _character.SetAnimatorTrigger("Idle");
                });
        }*/
    }

    public InteractableType MyInteractableType()
    {
        return _interactableType;
    }
}

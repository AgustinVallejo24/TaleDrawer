using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Ladders : MonoBehaviour, IInteractable
{
    [SerializeField] Transform _upperPoint;
    [SerializeField] Transform _lowerPoint;
    [SerializeField] float _movementDuration;
    [SerializeField] CustomNode _upperNode;
    [SerializeField] CustomNode _lowerNode;
    [SerializeField] Character _character;
    [SerializeField] bool _fromAbove;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
        if (_character.GetPathList(_upperNode).Count >= _character.GetPathList(_lowerNode).Count)
        {
            _fromAbove = false;
            _character.GetPath(_upperNode);
        }
        else
        {
            _fromAbove = true;
            _character.GetPath(_lowerNode);
        }
    }

    public void StartLadderMovement()
    {

        StartCoroutine(IGetOnLadder());
        
    }

    public IEnumerator IGetOnLadder()
    {
        _character.SendInputToFSM(CharacterStates.Wait);

        yield return null;

        LadderMovement();
    }

    private void LadderMovement()
    {
        _character.SendInputToFSM(CharacterStates.OnLadder);
        _character.characterRigidbody.gravityScale = 0;

        if (_fromAbove)
        {
            _character.characterView.OnEnteringLadder(1);
            _character.transform.DOMoveY(_lowerPoint.position.y, _movementDuration)
                .OnComplete(() => { _character.transform.position = _lowerNode.transform.position; _character.characterRigidbody.gravityScale = 1; 
                    StartCoroutine(_character.SendInputToFSM(CharacterStates.Moving, 0.2f)); });
        }
        else
        {
            _character.characterView.OnEnteringLadder(0);
            _character.transform.DOMoveY(_upperPoint.position.y, _movementDuration)
                .OnComplete(() => { _character.transform.position = _upperNode.transform.position; _character.characterRigidbody.gravityScale = 1; 
                    StartCoroutine(_character.SendInputToFSM(CharacterStates.Moving, 0.2f)); });
        }
    }
}

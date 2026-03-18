using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class LevelTransition : MonoBehaviour
{
    [SerializeField] Transform _playerPos;
    [SerializeField] string _level;
    bool active;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Character character) && !active)
        {
            active = true;
            character.SendInputToFSM(CharacterStates.DoingEvent);
            character.characterView.OnEventMovement();
            GameManager.instance.FadeOut();
            character.transform.DOMoveX(_playerPos.position.x, 2f).OnComplete(() =>
            {
                character.characterView.OnIdle();
                SceneManager.LoadScene(_level);

            });
        }
    }
}

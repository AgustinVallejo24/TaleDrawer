using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] Tutorials _myTutorial;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Character>(out Character cha))
        {
            Tutorial.instance.PlayTutorial(_myTutorial, 6);
            Destroy(gameObject);
        }

    }
}

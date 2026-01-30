using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class MainMenu : MonoBehaviour
{

    public bool victoryScene;
    public void Start()
    {
        if (victoryScene)
        {
            StartCoroutine(GoToMenu());
        }
    }

    public IEnumerator GoToMenu()
    {
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("Menu Principal");
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("Level 1-1");
    }

    public void Quit()
    {
        Application.Quit();
    }
}

using UnityEngine;
using System.Collections;
using TMPro;
public class Tutorial : MonoBehaviour
{
    public TMP_Text tutorialText;

    public NewSerializableDictionary<Tutorials, string> tutorialTexts;

    public static Tutorial instance;
    private void Start()
    {
        instance = this;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            PlayerPrefs.DeleteAll();
        }
    }
    public void PlayTutorial(Tutorials tutorialName)
    {
        if (PlayerPrefs.HasKey(tutorialName.ToString())) return;
        if(!tutorialTexts.ContainsKey(tutorialName)) return;
        tutorialText.text = tutorialTexts[tutorialName];
        PlayerPrefs.SetString(tutorialName.ToString(), "");
    }

    public void PlayTutorial(Tutorials tutorialName, float duration)
    {
        if (PlayerPrefs.HasKey(tutorialName.ToString())) return;
        if (!tutorialTexts.ContainsKey(tutorialName)) return;
        tutorialText.text = tutorialTexts[tutorialName];
        PlayerPrefs.SetString(tutorialName.ToString(), "");
        StartCoroutine(DeactivateTutorial(duration));
    }


    public IEnumerator DeactivateTutorial(float time)
    {
        yield return new WaitForSeconds(time);
        tutorialText.text = "";
    }
}

public enum Tutorials
{
    Movement,
    Drawing,
    Dragging, 
    Interact,
    MultiStokes,
    Rubber,
}

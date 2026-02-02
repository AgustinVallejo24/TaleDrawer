using UnityEngine;
using Ink.Runtime;
using TMPro;
using System;
using System.Collections;
using UnityEngine.UI;
public class DialogManager : MonoBehaviour
{
    public GameObject dialogueUI;
    public Image SpeakerImage;
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI continueText;
    public TextMeshProUGUI speakerName;
    public AudioSource voiceSource;
    public static DialogManager instance;
    Story story;
    Dialogue block;
    public Speaker currentSpeaker;
    public NewSerializableDictionary<Speaker, Sprite> speakerImages;
    bool isTyping = false;
    string currentLine;
    Coroutine typingCoroutine;
    private void Awake()
    {
        instance = this;
    }
    public void StartDialogue(Dialogue dialogueBlock)
    {
        Character.instance.characterRigidbody.linearVelocityX = 0;
        Character.instance.SendInputToFSM(CharacterStates.Stop);
        GameManager.instance.currentState = SceneStates.Dialogue;
        block = dialogueBlock;
        story = new Story(block.inkJSON.text);

        dialogText.font = block.baseFont;
        dialogueUI.SetActive(true);
        Continue();
    }

    public void Continue()
    {
        // Si está escribiendo  completar
        if (isTyping)
        {
            CompleteLine();
            return;
        }

        // Si NO hay más contenido  salir
        if (!story.canContinue)
        {
            EndDialogue();
            return;
        }

        // Avanzar historia
        currentLine = story.Continue();
        ReadTags();
        PlayVoice();   
        StartTyping(currentLine);
    }

    void EndDialogue()
    {
        Debug.Log("Diálogo terminado");
        dialogueUI.SetActive(false);
        Character.instance.SendInputToFSM(CharacterStates.Idle);

    }
    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogText.text = "";
        continueText.text = "";
        foreach (char c in line)
        {
            dialogText.text += c;
            yield return new WaitForSecondsRealtime(0.03f);
        }

        if (story.canContinue)
        {
            continueText.text = "Continue >>";
        }
        else
        {
            continueText.text = "End Dialogue";
        }
      
        isTyping = false;
    }

    void StartTyping(string line)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    void CompleteLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogText.text = currentLine;
        isTyping = false;
    }
    void ReadTags()
    {
        foreach (var tag in story.currentTags)
        {
            if (tag.StartsWith("speaker:"))
            {
                if (Enum.TryParse(tag.Replace("speaker:", ""), out Speaker speaker))
                {
                    currentSpeaker = speaker;
                    speakerName.text = speaker.ToString();
                    SpeakerImage.sprite = speakerImages[speaker];
                }
            }
        }
    }

    void PlayVoice()
    {
        var voiceData = Array.Find(block.voices, v => v.speaker == currentSpeaker);
        if (voiceData == null || voiceData.voiceClips.Length == 0)
            return;

        voiceSource.clip = voiceData.voiceClips[
            UnityEngine.Random.Range(0, voiceData.voiceClips.Length)
        ];
        voiceSource.Play();
    }
}

public enum Speaker
{
    Narrador,
    Robin,
    Amanda,
    Maya
}

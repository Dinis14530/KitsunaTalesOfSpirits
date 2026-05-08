using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInterectable
{
    [Header("Diálogo")]
    public NpcDialog dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;
    public PlayerController player;

    [Header("Áudio")]
    public AudioSource audioSource;
    public AudioClip dialogueClip;

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    string[] GetCurrentLanguageLines()
    {
        switch (LanguageManager.Instance.currentLanguage)
        {
            case Language.English:
                return dialogueData.dialogLines.english;

            default:
                return dialogueData.dialogLines.portuguese;
        }
    }

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if (dialogueData == null)
            return;

        if (isDialogueActive)
            NextLine();
        else
            StartDialogue();
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        player.isInDialogue = true;
        player.canMove = false;
        player.ForceIdle(); 

        nameText.SetText(dialogueData.npcName);
        portraitImage.sprite = dialogueData.npcPortrait;

        dialoguePanel.SetActive(true);

        PlayDialogueSound();
        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        string[] lines = GetCurrentLanguageLines();

        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.SetText(lines[dialogueIndex]); 
            isTyping = false;
        }
        else
        {
            dialogueIndex++;

            if (dialogueIndex < lines.Length)
            {
                PlayDialogueSound();
                StartCoroutine(TypeLine());
            }
            else
            {
                EndDialogue();
            }
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";

        string[] lines = GetCurrentLanguageLines();
        string currentLine = lines[dialogueIndex]; 

        foreach (char letter in currentLine)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if (dialogueData.autoProgressLines.Length > dialogueIndex &&
            dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;

        dialogueText.SetText("");
        dialoguePanel.SetActive(false);

        player.isInDialogue = false;
        player.canMove = true;
    }

    private void PlayDialogueSound()
    {
        if (audioSource != null && dialogueClip != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(dialogueClip);
        }
    }
}
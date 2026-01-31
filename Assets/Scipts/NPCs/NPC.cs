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
    public AudioSource audioSource;      // Fonte de som para diálogo
    public AudioClip dialogueClip;       // Som ao avançar no diálogo

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if (dialogueData == null)
            return;

        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        player.isInDialogue = true;
        player.canMove = false;

        nameText.SetText(dialogueData.npcName);
        portraitImage.sprite = dialogueData.npcPortrait;

        dialoguePanel.SetActive(true);

        // Toca som ao começar o diálogo
        PlayDialogueSound();

        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.dialogLines[dialogueIndex]);
            isTyping = false;
        }
        else
        {
            dialogueIndex++;

            if (dialogueIndex < dialogueData.dialogLines.Length)
            {
                // Toca som ao avançar para a próxima linha
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

        foreach (char letter in dialogueData.dialogLines[dialogueIndex])
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

    // Função para tocar som de diálogo
    private void PlayDialogueSound()
    {
        if (audioSource != null && dialogueClip != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f); // pitch aleatório
            audioSource.PlayOneShot(dialogueClip);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicPuzzleManager : MonoBehaviour
{
    public List<int> correctSequence = new List<int> { 0, 2, 1, 3 };
    private List<int> playerSequence = new List<int>();

    public UnityEvent onPuzzleSolved;
    public UnityEvent onPuzzleFailed;

    [Header("Sprite Change Settings")]
    public SpriteRenderer targetSpriteRenderer;
    public Sprite newSprite;

    private bool puzzleCompleted = false; // bloqueia interações após sucesso

    public void RegisterNote(int noteID)
    {
        // Se o puzzle já foi resolvido, não faz nada
        if (puzzleCompleted)
            return;

        // Regista a nota actual para validar a sequencia parcial
        playerSequence.Add(noteID);

        // Falha assim que uma nota foge da sequencia esperada
        for (int i = 0; i < playerSequence.Count; i++)
        {
            if (playerSequence[i] != correctSequence[i])
            {
                Fail();
                return;
            }
        }

        if (playerSequence.Count == correctSequence.Count)
        {
            Success();
        }
    }

    private void Success()
    {
        Debug.Log("Puzzle Complete!");
        puzzleCompleted = true; // bloqueia futuras interações

        // Muda o sprite do objeto
        if (targetSpriteRenderer != null && newSprite != null)
        {
            targetSpriteRenderer.sprite = newSprite;
        }

        // Dispara recompensas ou eventos ligados ao puzzle resolvido
        onPuzzleSolved?.Invoke();
        playerSequence.Clear();
    }

    private void Fail()
    {
        Debug.Log("Wrong sequence!");
        // Reinicia a tentativa para permitir nova sequencia limpa
        onPuzzleFailed?.Invoke();
        playerSequence.Clear();
    }
}

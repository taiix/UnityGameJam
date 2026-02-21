using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(HintSystem))]
public class DialogueManager : MonoBehaviour
{
    public static UnityAction OnDialogueStarted;
    public static UnityAction OnDialogueEnded;

    public static bool isTalking { get; private set; }

    private readonly Queue<string> sentences = new();
    public DialogueState dialogueState;

    [SerializeField] private GameObject dialogCanvas;
    [SerializeField] private GameObject[] otherCanvases;
    [SerializeField] private TextMeshProUGUI dialogueTextBox;

    // Public read-only access for HintSystem
    public bool IsTyping => _isTyping;
    private bool _isTyping;

    private void Update()
    {
        DialogSettings();

        if (Input.GetKeyDown(KeyCode.Tab) && !_isTyping)
        {
            NextSentence();
        }
    }

    public void StartDialogue(string[] dialogue)
    {
        if (dialogue == null || dialogue.Length == 0)
        {
            Debug.LogError("Dialogue is empty or null. Cannot start dialogue.");
            return;
        }

        isTalking = true;
        dialogueState = DialogueState.StartDialogue;
        OnDialogueStarted?.Invoke();
        ToggleCanvases(true);

        sentences.Clear();
        dialogueTextBox.text = string.Empty;

        foreach (var sentence in dialogue)
        {
            sentences.Enqueue(sentence);
        }

        NextSentence();
    }

    private void NextSentence()
    {
        if (sentences.Count > 0)
        {
            dialogueState = DialogueState.Talking;
            StartCoroutine(TypingDialogue(sentences.Dequeue()));
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        dialogueState = DialogueState.EndDialogue;

        ToggleCanvases(false);
        ClearDialogueUI();

        isTalking = false;

        Debug.Log("Dialogue has ended.");
        OnDialogueEnded?.Invoke(); // full dialogue finished
    }

    private IEnumerator TypingDialogue(string sentence)
    {
        _isTyping = true;
        dialogueTextBox.text = string.Empty;

        foreach (var c in sentence)
        {
            dialogueTextBox.text += c;

            var typingSpeed = Input.GetKey(KeyCode.Tab) ? 0.01f : 0.04f;
            yield return new WaitForSeconds(typingSpeed);
        }

        _isTyping = false;
    }

    private void ToggleCanvases(bool dialogueActive)
    {
        dialogCanvas.SetActive(dialogueActive);
        foreach (var canvas in otherCanvases)
        {
            canvas.SetActive(!dialogueActive);
        }
    }

    private void ClearDialogueUI()
    {
        dialogueTextBox.text = string.Empty;
    }

    private void DialogSettings()
    {
        switch (dialogueState)
        {
            case DialogueState.StartDialogue:
                Time.timeScale = 0;
                break;
            case DialogueState.Talking:
                Debug.Log("Talking dialogue");
                break;
            case DialogueState.EndDialogue:
                Time.timeScale = 1;
                break;
        }
    }
}

public enum DialogueState
{
    None,
    StartDialogue,
    Talking,
    EndDialogue
}

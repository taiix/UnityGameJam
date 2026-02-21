using System.Collections;
using UnityEngine;

[RequireComponent(typeof(DialogueManager))]
public class HintSystem : MonoBehaviour
{
    [SerializeField] private Dialogue[] dialogues;
    [SerializeField] private int currentDialogueIndex = 0;
    private DialogueManager dialogueManager;
    private Coroutine dialogueRoutine;

    private void Start()
    {
        dialogueManager = GetComponent<DialogueManager>();

        dialogueRoutine = StartCoroutine(DialogueLoop());
    }

    private void OnDisable()
    {
        if (dialogueRoutine != null)
        {
            StopCoroutine(dialogueRoutine);
            dialogueRoutine = null;
        }
    }

    private IEnumerator DialogueLoop()
    {
        while (true)
        {
            int nextIndex = FindNextIncompleteIndex(currentDialogueIndex);

            if (nextIndex == -1)
            {
                yield break;
            }

            currentDialogueIndex = nextIndex;
            Dialogue currentDialogue = dialogues[currentDialogueIndex];

            yield return new WaitForSeconds(currentDialogue.waitTimeInSeconds);

            dialogueManager.StartDialogue(currentDialogue.dialogue);

            while (DialogueManager.isTalking || dialogueManager.IsTyping)
            {
                yield return null;
            }

            bool tabPressed = false;
            while (!tabPressed)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    tabPressed = true;
                    CompleteCurrentDialogue();
                }

                yield return null;
            }
        }
    }

    private int FindNextIncompleteIndex(int startIndex)
    {
        for (int i = startIndex; i < dialogues.Length; i++)
        {
            if (!dialogues[i].dialogueCompleted)
            {
                return i;
            }
        }
        return -1;
    }

    private void CompleteCurrentDialogue()
    {
        if (currentDialogueIndex < 0 || currentDialogueIndex >= dialogues.Length)
        {
            return;
        }

        dialogues[currentDialogueIndex].dialogueCompleted = true;
    }
}

[System.Serializable]
public struct Dialogue
{
    public bool dialogueCompleted;
    public int waitTimeInSeconds;

    [TextArea(3, 10)]
    public string[] dialogue;
}



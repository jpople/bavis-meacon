using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI promptText;
    public TMP_InputField userInput;

    Dictionary<string, StoryNode> storyTree;
    StoryNode currentNode;
    BavisResponse currentResponse;
    List<BavisLine> currentDialogue;
    int currentLineIndex;


    // public string[] lines;
    public float textSpeed;
    // private int index;

    public Image bavisImage;
    public Sprite happyFace;
    public Sprite sadFace;

    // Start is called before the first frame update
    void Start()
    {
        dialogueText.text = string.Empty;
        storyTree = DialogueTreeManager.SpoofStoryTree();
        bavisImage.sprite = happyFace;
        userInput.onSubmit.AddListener(HandleSubmitInput);
        EnterDialogueNode(storyTree["node-test"]);
        StartCoroutine(TypeLine());
    }

    void EnterDialogueNode(StoryNode newNode)
    {
        currentNode = newNode;
        currentResponse = null;
        currentDialogue = currentNode.introLines;
        currentLineIndex = 0;
        dialogueText.text = string.Empty;
        promptText.text = string.Empty;
        userInput.interactable = true;
        userInput.ActivateInputField();
    }

    void HandleSubmitInput(string input)
    {
        if (promptText.text == string.Empty)
        {
            userInput.ActivateInputField();
            return;
        }
        if (currentNode.responses.TryGetValue(input, out BavisResponse response))
        {
            currentResponse = response;
        }
        else
        {
            currentResponse = currentNode.responses["default"];
        }
        if (currentResponse.nextNodeId != null)
        {
            userInput.interactable = false;
        }
        currentDialogue = currentResponse.lines;
        currentLineIndex = 0;
        userInput.text = string.Empty;
        dialogueText.text = string.Empty;
        userInput.ActivateInputField();
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        // if (currentDialogue[currentLineIndex].line[0] == 'L')
        // {
        //     bavisImage.sprite = sadFace;
        // }
        // else
        // {
        //     bavisImage.sprite = happyFace;
        // }
        bavisImage.sprite = currentLineIndex % 2 == 0 ? sadFace : happyFace;

        foreach (char c in currentDialogue[currentLineIndex].line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        if (currentLineIndex < currentDialogue.Count - 1)
        {
            Thread.Sleep(1500);
            NextLine();
        }
        else
        {
            StopAllCoroutines();
            dialogueText.text = currentDialogue[currentLineIndex].line;
            if (currentResponse == null)
            {
                promptText.text = currentNode.userPrompt;
                userInput.ActivateInputField();
            }
            else
            {
                if (currentResponse.nextNodeId != null)
                {
                    Thread.Sleep(1500);
                    EnterDialogueNode(storyTree[currentResponse.nextNodeId]);
                    StartCoroutine(TypeLine());
                }
            }
        }
    }

    void NextLine()
    {
        if (currentLineIndex < currentDialogue.Count - 1)
        {
            currentLineIndex++;
            dialogueText.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            Debug.Log("reached end of tree!");
        }
    }
}

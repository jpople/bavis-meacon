using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bavisDialogue;
    [SerializeField] TextMeshProUGUI wordPrompt;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button nextButton;

    Dictionary<string, StoryNode> storyTree;
    StoryNode currentNode;
    BavisResponse currentResponse = null;
    List<BavisLine> currentDialogue;
    bool isIntro = true;
    int _currentDialogueIndex = 0;
    int CurrentDialogueIndex
    {
        get => _currentDialogueIndex;
        set
        {
            _currentDialogueIndex = value;
            nextButton.interactable = GetNextButtonState();
            Debug.Log($"CurrentDialogueIndex: {value}, currentDialogue.Count: {currentDialogue.Count}, isIntro: {isIntro}");
        }
    }

    void Start()
    {
        storyTree = new();
        List<StoryNode> nodes = new() {
            SpoofDefaultNode(),
            SpoofNonDefaultNode()
        };
        foreach (var n in nodes)
        {
            storyTree.Add(n.id, n);
        }

        EnterNode(storyTree["node-test"]);
        ShowDialogue();
        inputField.onSubmit.AddListener(HandleSubmitInput);

    }

    StoryNode SpoofDefaultNode() => new(
        id: "node-test",
        introLines: new() {
            new("Hello, I'm Bavis Meacon.  Try typing the words you see at the top of your screen."),
            new("Go ahead, it's okay.  Try typing the words.")
        },
        userPrompt: "bat rat cat",
        responses: new() {
            {"bat rat cat", new("node-test-2", new(){new("Well done!  Press 'next' to continue to the next exercise.")})},
            {"test", new(null, new(){new("I see that you're testing the input.  I assure you, everything is in full working order."), new("Testing the button too, eh? Very shrewd.")})},
            {"fuck you", new(null, new(){new("There's no call for that kind of language.")})},
            {"default", new(null, new(){new("That's not quite what we were looking for.  Try again!")})}
        }
    );

    StoryNode SpoofNonDefaultNode() => new(
        id: "node-test-2",
        introLines: new() {
            new("I see you already have some typing experience.  Here's something a little more difficult to try out.")
        },
        userPrompt: "floccinaucinihilipilification",
        responses: new() {
            {"floccinaucinihilipilification", new(null, new(){new("Yes, very impressive, well done.")})},
            {"test", new(null, new(){new("Surely you know by now that our input works?")})},
            {"fuck you", new(null, new(){new("There's no call for that kind of language.")})},
            {"default", new(null, new(){new("Maybe you should try the beginner course?"), new("To be clear, Bavis Meacon LLC does not offer refunds under any circumstances.")})}
        }
    );

    bool GetNextButtonState()
    {
        if (CurrentDialogueIndex < currentDialogue.Count - 1)
        {
            return true;
        }
        else if (isIntro)
        {
            return false;
        }
        else if (currentResponse != null && currentResponse.nextNodeId == null)
        {
            return false;
        }
        return true;
    }

    void HandleSubmitInput(string input)
    {
        isIntro = false;
        if (currentNode.responses.TryGetValue(input, out BavisResponse response))
        {
            currentResponse = response;
        }
        else
        {
            currentResponse = currentNode.responses["default"];
        }
        currentDialogue = currentResponse.lines;

        CurrentDialogueIndex = 0;
        ShowDialogue();
        inputField.text = string.Empty;
        inputField.ActivateInputField();
    }

    public void HandleNextButtonPressed()
    {
        if (currentResponse != null && CurrentDialogueIndex == currentDialogue.Count - 1)
        {
            EnterNode(storyTree[currentResponse.nextNodeId]);
            return;
        }
        CurrentDialogueIndex = _currentDialogueIndex + 1;
        ShowDialogue();
    }

    void ShowDialogue()
    {
        bavisDialogue.text = currentDialogue[CurrentDialogueIndex].line;
    }

    void EnterNode(StoryNode newNode)
    {
        currentNode = newNode;
        isIntro = true;
        currentResponse = null;
        currentDialogue = currentNode.introLines;
        CurrentDialogueIndex = 0;
        ShowDialogue();
        wordPrompt.text = currentNode.userPrompt;
    }
}

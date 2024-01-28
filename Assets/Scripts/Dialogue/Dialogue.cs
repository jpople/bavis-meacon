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
    public float textSpeed;

    public Image bavisImage;
    public Sprite happyFace;
    public Sprite sadFace;

    [SerializeField] AudioClip[] typingSoundClips;
    AudioSource source;
    [SerializeField] private DialogueAudioInfoSO defaultAudioInfo;
    [SerializeField] private DialogueAudioInfoSO[] allAudioInfoSOs;
    Dictionary<string, DialogueAudioInfoSO> audioInfoLookup;
    private DialogueAudioInfoSO currentAudioInfo;

    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        dialogueText.text = string.Empty;
        storyTree = DialogueTreeManager.SpoofStoryTree();
        audioInfoLookup = InitializeAudioInfoLookup();
        bavisImage.sprite = happyFace;
        userInput.onSubmit.AddListener(HandleSubmitInput);
        EnterDialogueNode(storyTree["node-test"]);
        StartCoroutine(TypeLine());
    }

    Dictionary<string, DialogueAudioInfoSO> InitializeAudioInfoLookup()
    {
        var lookup = new Dictionary<string, DialogueAudioInfoSO>
        {
            { "default", defaultAudioInfo }
        };
        foreach (var i in allAudioInfoSOs)
        {
            lookup.Add(i.id, i);
        }
        return lookup;
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
        BavisLine currentLine = currentDialogue[currentLineIndex];
        bavisImage.sprite = currentLineIndex % 2 == 0 ? sadFace : happyFace;

        currentAudioInfo = audioInfoLookup[currentLine.audioId] ?? defaultAudioInfo;

        foreach (char c in currentLine.line.ToCharArray())
        {
            PlayDialogueSound();
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
            dialogueText.text = currentLine.line;
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

    private void PlayDialogueSound()
    {
        AudioClip sound = typingSoundClips[Random.Range(0, typingSoundClips.Length)];
        source.pitch = Random.Range(currentAudioInfo.minPitch, currentAudioInfo.maxPitch);
        source.PlayOneShot(sound);

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Text.RegularExpressions;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI promptText;
    public TMP_InputField userInput;

    string correctUserName;
    string wrongUserName;

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
    // [SerializeField] private AudioClip[] typingSoundClips;

    private DialogueAudioInfoSO currentAudioInfo;
    private int charIndexInWord;

    public AudioClip testClip;

    void Start()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        dialogueText.text = string.Empty;
        storyTree = DialogueTreeManager.SpoofStoryTree();
        audioInfoLookup = InitializeAudioInfoLookup();
        bavisImage.sprite = happyFace;
        userInput.onSubmit.AddListener(HandleSubmitInput);
        EnterDialogueNode(storyTree["intro"]);
        StartCoroutine(TypeLine());
        charIndexInWord = 0;
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
        if (currentNode.id == "intro")
        {
            correctUserName = input;
            wrongUserName = FuckedUpName(input);
            Debug.Log(correctUserName);
            Debug.Log(wrongUserName);
        }
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

    string FuckedUpName(string name)
    {
        string lower = name.ToLower();
        string result;
        if (Regex.IsMatch(lower, "^[aeiouy]"))
        {
            result = "J" + lower;
        }
        else
        {
            char first = lower[0];
            char newFirst = first == 'j' ? 'R' : 'J';
            result = newFirst + lower[1..];
        }
        return result;
    }

    string ParseLine(string line)
    {
        return line.Replace("{{name}}", correctUserName).Replace("{{wrongName}}", wrongUserName);
    }

    IEnumerator TypeLine()
    {
        BavisLine currentLine = currentDialogue[currentLineIndex];
        bavisImage.sprite = currentLineIndex % 2 == 0 ? sadFace : happyFace;

        string lineText = ParseLine(currentLine.line);

        currentAudioInfo = audioInfoLookup[currentLine.audioId] ?? defaultAudioInfo;

        foreach (char c in lineText.ToCharArray())
        {
            PlayDialogueSound(dialogueText.text.Length);
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
            dialogueText.text = lineText;
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

    private void PlayDialogueSound(int length)
    {
        if (length % defaultAudioInfo.frequencyLevel != 0) return;
        AudioClip randomClip = typingSoundClips[Random.Range(0, typingSoundClips.Length)];
        int minPitchInt = (int)(defaultAudioInfo.minPitch * 100);
        int maxPitchInt = (int)(defaultAudioInfo.maxPitch * 100);
        int pitchRangeInt = maxPitchInt - minPitchInt;
        if (pitchRangeInt != 0)
        {
            int predictablePitchInt = Random.Range(minPitchInt, maxPitchInt);
            float predictablePitch = predictablePitchInt / 100f;
            source.pitch = predictablePitch;
        }
        else
        {
            source.pitch = minPitchInt / 100f;
        }
        source.PlayOneShot(randomClip);
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

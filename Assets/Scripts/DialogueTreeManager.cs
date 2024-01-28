using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTreeManager : MonoBehaviour
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
        }
    }

    void Start()
    {
        storyTree = new();
        List<StoryNode> nodes = new()
        {
        };
        foreach (var n in nodes)
        {
            storyTree.Add(n.id, n);
        }

        EnterNode(storyTree["node-test"]);
        ShowDialogue();
        inputField.onSubmit.AddListener(HandleSubmitInput);

    }

    public static Dictionary<string, StoryNode> SpoofStoryTree()
    {
        Dictionary<string, StoryNode> tree = new();
        List<StoryNode> nodes = new() {
            IntroNode(),
            FirstExercise(),
            SecondExercise(),
            ThirdExercise(),
            FourthExercise(),
            FifthExercise(),
            SixthExercise(),
            SeventhExercise(),
            EighthExercise(),
            ClosingExercise(),
        };
        foreach (var n in nodes)
        {
            tree.Add(n.id, n);
        }
        return tree;
    }

    public static StoryNode IntroNode() => new(
        id: "intro",
        introLines: new() {
            new("Welcome to Bavis Meacon Teaches Typing! I'm Bavis, I'm happy to be your guide as we explore the exciting world of typing."),
            new("What's your name? Key it in, then strike the \"Return\" key.")
        },
        userPrompt: "Enter your name",
        responses: new() {
            {"default", new("first-exercise", new(){new("Nice to meet you, {{name}}!")})}
        }
    );


    public static StoryNode FirstExercise() => new(
        id: "first-exercise",
        introLines: new() {
            new("Try typing the word you see at the top of the screen, then striking the \"Return\" key."),
        },
        userPrompt: "bat",
        responses: new() {
            {"bat", new("second-exercise", new(){new("Well done, {{name}}!  You're off to a strong start.")})
            },
            {"default", new(null, new(){new("Not quite!  Let's try again.")})}
        }
    );

    public static StoryNode SecondExercise() => new(
        id: "second-exercise",
        introLines: new() {
            new("Let's do it again. Type the word you see at the top of the screen, then strike the \"Return\" key.")
        },
        userPrompt: "cat",
        responses: new() {
            {"cat", new("third-exercise", new(){new("Well done, {{wrongName}}! You have strong fundamentals.")})},
            {"default", new(null, new(){new("Look, can I level with you for a second, {{name}}?"), new("Maybe typing just isn't for you."), new("Everybody's got a thing, and typing might not be your thing."), new("You can try again if you really want to, but maybe you'd be better off learning to garden or something.")})}
        }
    );

    public static StoryNode ThirdExercise() => new(
        id: "third-exercise",
        introLines: new() {
            new("Type the words you see at the top of the screen, then strike the \"Return\" key.")
        },
        userPrompt: "baat ratt ccat",
        responses: new() {
            {"baat ratt ccat", new("fourth-exercise", new(){new("Very impressive! You'll be a master typist in no time, {{wrongName}}.")})},
            {"default", new(null, new(){new("Try it again!  These are very common words, this should be easy for you.")})}
        }
    );

    public static StoryNode FourthExercise() => new(
        id: "fourth-exercise",
        introLines: new() {
            new("Okay, {{wrongName}}, let's move on to something more advanced-- sentences!"),
            new("Type the sentence at the top of your screen, then strike the \"Return\" key.")
        },
        userPrompt: "I am not very good at typing",
        responses: new() {
            {"I am not very good at typing", new("fifth-exercise", new(){new("Everyone has to start somewhere!"), new("It took a lot of courage to admit that, {{wrongName}}.")})},
            {"default", new(null, new(){new("Try again!  You'll never get anywhere without admitting that you need help first.")})}
        }
    );

    public static StoryNode FifthExercise() => new(
        id: "fifth-exercise",
        introLines: new() {
            new("Let's try another sentence.")
        },
        userPrompt: "I love you, Ms. Meacon",
        responses: new() {
            {"default", new(null, new(){new("Try again. I'm trying to help you, please just do this for me, I need to hear this.")})},
            {"I love you, Ms. Meacon", new("sixth-exercise", new() {new("Well, that's very kind of you to say."), new("However, I do have to ask that you keep things professional, {{wrongName}}.")})}
        }
    );

    public static StoryNode SixthExercise() => new(
        id: "sixth-exercise",
        introLines: new() {
            new("Type the characters you see at the top of your screen, then strike the \"Return\" key.")
        },
        userPrompt: "11lil1il1",
        responses: new() {
            {"default", new("seventh-exercise", new(){new("That was... close?"), new("Maybe you should work on your reading comprehension before learning to type, {{wrongName}}.")})}
        }
    );

    public static StoryNode SeventhExercise() => new(
        id: "seventh-exercise",
        introLines: new() {
            new("Type the words you see at the top of the screen, then strike the \"Return\" key:")
        },
        userPrompt: "保等登藝須 伊等布登伎奈之 安夜賣具左 加豆良尓勢武日 許由奈伎和多礼",
        responses: new() {
            {"default", new("eighth-exercise", new(){new("もう少し頑張りましょうね?"), new("Remember, the 伊 key is right next to the 藺 key.")})}
        }
    );

    public static StoryNode EighthExercise() => new(
        id: "eighth-exercise",
        introLines: new() {
            new("Can I ask you something, {{wrongName}}?"),
            new("So, my brother and sister-in-law have 2 teenage daughters, his daughter Bria and her daughter Leah."),
            new("But my sister-in-law has this weird obsession with making sure that every time someone compliments Bria they also compliment Leah."),
            new("If I tell Bria that she is very talented in something, she'll immediately go \"But isn't Leah also very talented?\" It's so annoying."),
            new("So a few days ago we were at their home and the girls were getting ready to go to a party. Bria was looking absolutely gorgeous so I told her \"Oh my God, Bria, you look gorgeous!\""),
            new("My sister-in-law INTERRUPTS ME and says \"But isn't Leah incredibly gorgeous too?\""),
            new("I had just had it, so I finally snapped and said \"No, she looks fine.\""),
            new("She asked me what the hell was wrong with me, and I said I didn't want to say it but it was true."),
            new("It's just like, if she thinks her daughter is gorgeous that's great, but she can't expect other people to do the complimenting for her, you know?"),
            new("What do you think?  Am in the wrong here?"),
            new("Type \"yes\" or \"no\", then strike the \"Return\" key.")
        },
        userPrompt: "Was I wrong?",
        responses: new() {
            {"default", new(null, new() {new("It's a simple question, {{wrongName}}.  Yes or no?")})},
            {"yes", new("closing", new() {new("Are you seriously going to take her side?"), new("Ugh, you're just like Brad, he can't see the situation clearly either."), new("And neither of you can type worth a damn.")})},
            {"no", new("ninth-exercise", new() {new("Ugh, that's what I'm saying!"), new("She's so weird about it!"), new("I mean, um, good job typing,")})}
        }
    );

    public static StoryNode ClosingExercise() => new(
        id: "closing",
        introLines: new() {
            new("Well, that's all the time we have for today, {{wrongName}}."),
            new("I hope this was helpful!"),
            new("Type the words at the top of the screen, then strike the \"Return\" key.")
        },
        userPrompt: "Goodbye, Ms. Meacon, you're the best typing teacher I've ever had",
        responses: new() {
            {"default", new(null, new(){new("Wow. Okay. I see how it is."), new("Whatever. Just go buy a different typing game then.")})},
            {"Goodbye, Ms. Meacon, you're the best typing teacher I've ever had", new(null, new(){new("Aww, that's so sweet of you!"), new("Your skills are certainly in the top 70% of my students."), new("See you next time!")})}
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

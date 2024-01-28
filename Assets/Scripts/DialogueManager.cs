// adapted from https://github.com/shapedbyrainstudios/ink-dialogue-system/tree/7-dialogue-audio-implemented
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DialogueManager: MonoBehaviour
{

    [Header("Audio")]
    [SerializeField] private DialogueAudioInfoSO defaultAudioInfo;
    [SerializeField] private DialogueAudioInfoSO[] audioInfos;
    public AudioClip[] dialogueTypingSoundClips;
    private DialogueAudioInfoSO currentAudioInfo;
    private Dictionary < string, DialogueAudioInfoSO > audioInfoDictionary;
    public AudioSource audioSource;


    private void PlayDialogueSound(int currentDisplayedCharacterCount, char currentCharacter)
    {
        if (!currentAudioInfo)
        {
            currentAudioInfo = defaultAudioInfo;
        }
        // set variables for the below based on our config
        AudioClip soundClip = null;
        int frequencyLevel = currentAudioInfo.frequencyLevel;
        float minPitch = currentAudioInfo.minPitch;
        float maxPitch = currentAudioInfo.maxPitch;
        bool stopAudioSource = currentAudioInfo.stopAudioSource;
        
        //sound clip
        int randomIndex = Random.Range(0, dialogueTypingSoundClips.Length);
        soundClip = dialogueTypingSoundClips[randomIndex];
        //pitch
        audioSource.pitch = Random.Range(minPitch, maxPitch);

        // play sound
        audioSource.PlayOneShot(soundClip);
    }

/* -------------------------------------------------------------------------- */
/*     EVERYTHING BELOW HERE (EXCEPT FOR THE FINAL BRACKET) IS FOR TESTING    */
/* -------------------------------------------------------------------------- */

    string text = "Hello World";
    public Button button;

    // Start is called before the first frame update
    void Start()
    {
        Button btn = button.GetComponent < Button > ();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        StartCoroutine(DialogueMiddleman());
    }

    private IEnumerator DialogueMiddleman()
    {
        int idx = 0;
        foreach (char c in text)
        {
            yield return new WaitForSeconds(0.15f);
            if (c != ' ')
            {
                PlayDialogueSound(idx, c);
            }
        idx ++;
        }
    }
}
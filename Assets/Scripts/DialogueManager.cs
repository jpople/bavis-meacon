// adapted from https://github.com/shapedbyrainstudios/ink-dialogue-system/tree/7-dialogue-audio-implemented

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DialogueManager: MonoBehaviour {
    [Header("Audio")]
    [SerializeField] private DialogueAudioInfoSO defaultAudioInfo;
    public AudioClip[] dialogueTypingSoundClips;
    private DialogueAudioInfoSO currentAudioInfo;
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

        int hashCode = currentCharacter.GetHashCode();
        // sound clip

        int predictableIndex = Mathf.Abs(hashCode) % dialogueTypingSoundClips.Length;
        soundClip = dialogueTypingSoundClips[predictableIndex];
        // pitch
        int minPitchInt = (int)(minPitch * 100);
        int maxPitchInt = (int)(maxPitch * 100);
        int pitchRangeInt = maxPitchInt - minPitchInt;
        // cannot divide by 0, so if there is no range then skip the selection
        if (pitchRangeInt != 0)
        {
            int predictablePitchInt = (hashCode % pitchRangeInt) + minPitchInt;
            float predictablePitch = predictablePitchInt / 100 f;
            audioSource.pitch = predictablePitch;
        } else {
            audioSource.pitch = minPitch;
        }

        // play sound
        audioSource.PlayOneShot(soundClip);
    }

    /* -------------------------------------------------------------------------- */
    /*     EVERYTHING BELOW HERE (EXCEPT FOR THE FINAL BRACKET) IS FOR TESTING    */
    /* -------------------------------------------------------------------------- */

    // string text = "Hello World Hello World";
    string text = "よく出来ましたね！";
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
        foreach(char c in text)
        {
            yield
            return new WaitForSeconds(0.15 f);
            if (c == ' ')
            {
                idx = 0;
            } else {
                PlayDialogueSound(idx, c);
                idx++;
            }
        }
    }
}
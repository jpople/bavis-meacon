using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using System;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    private int index;

    public UnityEngine.UI.Image bavisImage;
    public Sprite happyFace;
    public Sprite sadFace;
    
    // Start is called before the first frame update
    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
        bavisImage.sprite = happyFace;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        if (lines[index][0] == 'L')
        {
            bavisImage.sprite = sadFace;
        }
        else
        {
            bavisImage.sprite = happyFace;
        }

        UnityEngine.Debug.Log("line check = " + (lines[index][0]));
        //UnityEngine.Debug.Log("emotion is " + emotion);

        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        if (index < lines.Length - 1)
        {
            Thread.Sleep(1000);
            NextLine();
        }
        else
        {
            StopAllCoroutines();
            textComponent.text = lines[index];
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine (TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}

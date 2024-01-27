using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class PortraitChange : MonoBehaviour
{

    UnityEngine.UI.Image m_Image;
    //Set this in the Inspector
    public Sprite m_Sprite;
    public Sprite sprite2;

    // Start is called before the first frame update
    void Start()
    {
        //Fetch the Image from the GameObject
        m_Image = GetComponent<UnityEngine.UI.Image>();
    }

    // Update is called once per frame
    void Update()
    {
        //Press space to change the Sprite of the Image
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_Image.sprite = sprite2;
        }
    }
}

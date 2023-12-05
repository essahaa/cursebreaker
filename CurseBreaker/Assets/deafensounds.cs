using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deafensounds : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        void DeafenSounds()
        {
            FindObjectOfType<AudioManager>().DeafenSound("youfail");
            FindObjectOfType<AudioManager>().DeafenSound("musa");
            FindObjectOfType<AudioManager>().DeafenSound("winner");
            FindObjectOfType<AudioManager>().DeafenSound("riddedred");
            FindObjectOfType<AudioManager>().DeafenSound("liik");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    void Awake() {
        if(instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        
        
    }
}
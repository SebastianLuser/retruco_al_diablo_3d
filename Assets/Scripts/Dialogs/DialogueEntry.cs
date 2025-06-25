using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DialogueEntry
{
    public string speaker = "Opponent";
    
    [TextArea(3, 10)]
    public string dialogueText;
    
    public float duration = 3f;
    
    public UnityEvent onDialogueEnd;
}
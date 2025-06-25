using UnityEngine;
using UnityEngine.Events;

public class InteractableDialogue : MonoBehaviour
{
    [SerializeField] private DialogueEntry dialogueEntry;
    [SerializeField] private DialogueDatabaseSO database;
    [SerializeField] private string speakerKey;
    [SerializeField] private bool showOnlyOnce = false;
    
    public UnityEvent onDialogueTriggered;
    
    private bool hasBeenInteracted = false;
    
    //TODO Agregar que no haga falta cliquear para avanzar al siguiente texto , adicionalmente que avance y finalice solo el mismo
    void OnMouseDown()
    {
        if (showOnlyOnce && hasBeenInteracted) return;
        hasBeenInteracted = true;

        if (database == null)
        {
            Debug.LogWarning("[InteractableDialogue] DialogueDatabaseSO not assigned.");
            return;
        }

        var entries = database.GetDialogues(speakerKey);
        if (entries == null || entries.Count == 0)
        {
            Debug.LogWarning($"No dialogs for key '{speakerKey}'.");
            return;
        }

        DialogueManager.Instance.EnqueueDialogue(entries);
        onDialogueTriggered?.Invoke();
        
    } 
}

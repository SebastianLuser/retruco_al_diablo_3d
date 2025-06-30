using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using GameSystems.Dialogs;
using UnityEngine;

namespace GameSystems.Dialogs
{
    [Serializable]
    public class SpeakerDialogues
    {
        public string speakerKey;
        public List<DialogueEntry> dialogues;
    }

    [CreateAssetMenu(fileName = "DialogueDatabase", menuName = "Dialogue/Database")]
    public class DialogueDatabaseSO : ScriptableObject
    {
        [SerializeField] private List<SpeakerDialogues> speakerDialoguesList;

        private Dictionary<string, List<DialogueEntry>> _db;

        private void OnEnable()
        {
            _db = new Dictionary<string, List<DialogueEntry>>(speakerDialoguesList.Count);
            foreach (var sd in speakerDialoguesList)
            {
                if (!string.IsNullOrEmpty(sd.speakerKey) && !_db.ContainsKey(sd.speakerKey))
                    _db.Add(sd.speakerKey, sd.dialogues);
            }
        }

        public List<DialogueEntry> GetDialogues(string speakerKey)
        {
            if (_db != null && _db.TryGetValue(speakerKey, out var list))
                return list;
            return null;
        }
    }
}
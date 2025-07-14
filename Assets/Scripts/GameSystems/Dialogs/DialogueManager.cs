using System.Collections;
using System.Collections.Generic;
using GameSystems.Dialogs;
using TDA;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameSystems.Dialogs
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TMP_Text dialogueTextUI;
        [SerializeField] private TMP_Text speakerNameUI;
        [SerializeField] private GameObject continueIndicator;

        [SerializeField] private float typeSpeed = 0.05f;
        [SerializeField] private int maxCharactersPerPage = 200;

        [SerializeField] private float fadeDuration = 0.5f;

        private Dictionary<string, DynamicQueueTDA<DialogueEntry>> _queues = new();
        private Dictionary<string, bool> _processing = new();

        private bool skipTyping = false;

        void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public void EnqueueDialogue(DialogueEntry entry)
        {
            string spk = entry.speaker;
            if (!_queues.ContainsKey(spk))
            {
                var q = new DynamicQueueTDA<DialogueEntry>();
                _queues[spk] = q;
                _processing[spk] = false;
            }

            _queues[spk].Enqueue(entry);

            if (!_processing[spk])
                StartCoroutine(ProcessDialogueQueue(spk));
        }

        public void EnqueueDialogue(IEnumerable<DialogueEntry> entries)
        {
            foreach (var e in entries)
                EnqueueDialogue(e);
        }

        private IEnumerator ProcessDialogueQueue(string speaker)
        {
            _processing[speaker] = true;
            var queue = _queues[speaker];

            while (!queue.IsEmpty())
            {
                var entry = queue.Dequeue();
                yield return StartCoroutine(ShowDialogueRoutine(entry));
            }

            _processing[speaker] = false;
        }

        private IEnumerator ShowDialogueRoutine(DialogueEntry entry)
        {
            CanvasGroup cg = dialoguePanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                dialoguePanel.SetActive(true);
                float t = 0f;
                while (t < fadeDuration)
                {
                    t += Time.deltaTime;
                    cg.alpha = Mathf.Clamp01(t / fadeDuration);
                    yield return null;
                }

                cg.alpha = 1f;
            }
            else
            {
                dialoguePanel.SetActive(true);
            }

            if (speakerNameUI != null)
                speakerNameUI.text = entry.speaker;
            dialogueTextUI.text = "";

            //yield return new WaitUntil(() => !Input.GetMouseButton(0));

            List<string> pages = SplitTextIntoPages(entry.dialogueText, maxCharactersPerPage);

            for (int pageIndex = 0; pageIndex < pages.Count; pageIndex++)
            {
                dialogueTextUI.text = "";
                skipTyping = false;

                string page = pages[pageIndex];
                for (int i = 0; i < page.Length; i++)
                {
                    dialogueTextUI.text += page[i];

                    if (Input.GetMouseButtonDown(0))
                        skipTyping = true;
                    if (skipTyping)
                    {
                        dialogueTextUI.text = page;
                        break;
                    }

                    yield return new WaitForSeconds(typeSpeed);
                }

                if (entry.autoPass)
                {
                    if (continueIndicator != null)
                        continueIndicator.SetActive(true);
                    yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
                    yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
                    if (continueIndicator != null)
                        continueIndicator.SetActive(false);
                }
                else
                {
                    yield return new WaitForSeconds(entry.duration);
                }

                yield return null;
            }

            if (entry.autoPass)
            {
                yield return null;
            }
            else
            {
                yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            }

            if (cg != null)
            {
                float t = fadeDuration;
                while (t > 0)
                {
                    t -= Time.deltaTime;
                    cg.alpha = Mathf.Clamp01(t / fadeDuration);
                    yield return null;
                }

                cg.alpha = 0f;
            }

            dialoguePanel.SetActive(false);

            dialogueTextUI.text = "";
            speakerNameUI.text = "";
        }

        private List<string> SplitTextIntoPages(string text, int maxChars)
        {
            List<string> pages = new List<string>();

            if (text.Length <= maxChars)
            {
                pages.Add(text);
                return pages;
            }

            int start = 0;
            while (start < text.Length)
            {
                int length = Mathf.Min(maxChars, text.Length - start);
                int end = start + length;

                if (end < text.Length)
                {
                    int lastSpace = text.LastIndexOf(' ', end);
                    if (lastSpace > start)
                        length = lastSpace - start;
                }

                pages.Add(text.Substring(start, length));
                start += length;
            }

            return pages;
        }
    }
}
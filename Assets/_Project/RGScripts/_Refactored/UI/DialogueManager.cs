using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace _Project.RGScripts._Refactored.UI
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField]
        private Transform dialogBox;
        private Animator animDia;

        private Text nameText;
        private Text dialogueText;
        public Queue<string> messageQueue = new Queue<string>();

        private void Awake()
        {
            animDia = dialogBox.GetComponent<Animator>(); 

            nameText = dialogBox.GetChild(2).GetComponent<Text>();
            dialogueText = dialogBox.GetChild(1).GetComponent<Text>();
        }

        public void InitDialogue(string name, string[] messages)
        {
            animDia.SetBool("IsOpen", true);
            foreach (string m in messages)
            {
                messageQueue.Enqueue(m);
            }
            SetDialogue(name, messageQueue.Dequeue());
        }

        public void EndDialogue()
        {
            messageQueue = new Queue<string>();
            animDia.SetBool("IsOpen", false);
        }

        public void Continue()
        {
            Debug.Log("HI");

            if (messageQueue.Count == 0)
            {
                EndDialogue();
                return;
            }

            SetDialogue(messageQueue.Dequeue());
        }

        private void SetDialogue(string name, string dialogue)
        {
            nameText.text = name;
            dialogueText.text = dialogue;
        }

        private void SetDialogue(string dialogue)
        {
            dialogueText.text = dialogue;
        }
    }
}

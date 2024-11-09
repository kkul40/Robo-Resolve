using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.RGScripts._Refactored.UI {
    public class DialogueTrig : MonoBehaviour
    {
        [SerializeField]
        private string[] messages;
        [SerializeField]
        private string name;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                FindAnyObjectByType<DialogueManager>().InitDialogue(name, messages);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                FindAnyObjectByType<DialogueManager>().EndDialogue();
            }            
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public Dialog dialog;

    public void TriggerDialog()
    {
        FindObjectOfType<DialogManager>().StartDialog(dialog);
    }

    public void NextDialog()
    {
        FindObjectOfType<DialogManager>().DisplayNextSentence();
    }

    public void EndDialog()
    {
        FindObjectOfType<DialogManager>().EndDialog();
    }
}

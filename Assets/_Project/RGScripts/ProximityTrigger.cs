using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityTrigger : MonoBehaviour
{
    public GameObject player;
    public DialogTrigger dialogTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            dialogTrigger.TriggerDialog();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            dialogTrigger.NextDialog();
            StartCoroutine(delay(2));
            dialogTrigger.EndDialog();
        }
    }

    IEnumerator delay (int seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}

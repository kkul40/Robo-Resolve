using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButtonTrigger : MonoBehaviour
{
    public GameObject player;
    public GameObject actionObject;
    public bool isActive = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("triggering Button");
        if (collision.tag == "Player" && isActive)
        {
            Debug.Log("triggered by player");
            FindObjectOfType<ObjectActionTrigger>().DoAction();
            isActive = false;
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectActionTrigger : MonoBehaviour
{
    public Animator animator;
    public float distance;
    public float speed = 2;
    public bool isActivated = false;
    public bool goingUp;
    public GameObject player;
    float startingPosition;

    private void Start()
    {
        startingPosition = transform.position.y;
    }

    void Update()
    {
        if (isActivated && goingUp)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
        if (transform.position.y <= startingPosition)
        {
            goingUp = true;
        }
        if (transform.position.y >= startingPosition + distance)
        {
            goingUp = false;

            player.transform.SetParent(null, true);
        }
    }
    public void DoAction()
    {
        Debug.Log("Activating DOAction");
        player.transform.SetParent(transform, true);
        StartCoroutine(DoActionTimer());
    }

    IEnumerator DoActionTimer()
    {
        yield return new WaitForSeconds(1);
        isActivated = true;
        goingUp = true;
    }
}

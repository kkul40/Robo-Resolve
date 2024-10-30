using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingfloorLands : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
    }
}

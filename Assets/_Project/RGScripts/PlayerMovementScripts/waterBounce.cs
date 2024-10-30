using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterBounce : MonoBehaviour
{
    public float velocity = 0;
    public float force = 0;
    public float height = 0f;
    public float target_height = 0f;

    public void WaveSpringUpdate(float springStiffness, float dampening)
    {
        height = transform.localPosition.y;
        var x = height - target_height;
        var loss = -dampening * velocity;
        force = -springStiffness * x;
        velocity += force;
        var y = transform.localPosition.y;
        transform.localPosition = new Vector3(transform.localPosition.x, y + velocity, transform.localPosition.z);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

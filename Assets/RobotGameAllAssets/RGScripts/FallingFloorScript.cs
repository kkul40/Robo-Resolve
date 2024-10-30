using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingFloorScript : MonoBehaviour
{
    public string SE_FloorFallAudio;
    [Tooltip("If true, animation will start at a random position in the sequence.")]
    public bool randomAnimationStartTime = false;
    [Tooltip("List of frames that make up the animation.")]
    public Sprite[] fallingAnimation;

    internal Sprite[] sprites = new Sprite[0];

    internal SpriteRenderer _renderer;

    //active frame in animation, updated by the controller.
    internal int frame = 0;

    public bool falling = false;

    public GameObject fallingFloor;

    void OnTriggerEnter2D(Collider2D other)
    {
        //only exectue OnPlayerEnter if the player collides with this token.
        var player = other.gameObject.GetComponent<PlayerMovement>();
        if (player != null) OnPlayerEnter(player);
    }

    void OnPlayerEnter(PlayerMovement player)
    {
        if (falling) return;
        SoundManager.Instance.PlaySESound(SoundManager.eSoundChannel.SE01, SoundManager.eSoundEffect.Land);
        frame = 0;
        sprites = fallingAnimation;
        falling = true;

        StartCoroutine(FloorFall());
        
    }

    IEnumerator FloorFall()
    {
        yield return new WaitForSeconds(1);
        var fallingFloorObject = Instantiate(fallingFloor);
        fallingFloorObject.transform.position = gameObject.transform.position;
        transform.parent.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        transform.parent.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(3);
        transform.parent.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        transform.parent.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        falling = false;

    }
}

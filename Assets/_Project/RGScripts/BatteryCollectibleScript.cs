using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryCollectibleScript : MonoBehaviour
{
    [Tooltip("If true, animation will start at a random position in the sequence.")]
    public bool randomAnimationStartTime = false;
    [Tooltip("List of frames that make up the animation.")]
    public Sprite[] idleAnimation, collectedAnimation;

    internal Sprite[] sprites = new Sprite[0];

    internal SpriteRenderer _renderer;

    //active frame in animation, updated by the controller.
    internal int frame = 0;
    internal bool collected = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        BatteryCollectibleScript cs = GetComponent<BatteryCollectibleScript>();
        GameManager.Instance.AddBatteriesToResetPool(cs);

        //only exectue OnPlayerEnter if the player collides with this token.
        var player = other.gameObject.GetComponent<PlayerMovement>();
        if (player != null) OnPlayerEnter(player);
    }

    void OnPlayerEnter(PlayerMovement player)
    {
        if (collected) return;
        player.script.numberOfBatteries++;
        player.script.maxSolarCell++;
        player.script.AddNewSolarCell();
        SoundManager.Instance.PlaySESound(SoundManager.eSoundChannel.SE01, SoundManager.eSoundEffect.Hurt_Reverb);
        //disable the gameObject and remove it from the controller update list.
        frame = 0;
        sprites = collectedAnimation;
        collected = true;
        this.gameObject.SetActive(false);
    }

    public void Reset()
    {
        collected = false;
        this.gameObject.SetActive(true);
    }
}

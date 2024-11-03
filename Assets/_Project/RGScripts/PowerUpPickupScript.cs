using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpPickupScript : MonoBehaviour
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
        PowerUpPickupScript cs = GetComponent<PowerUpPickupScript>();
        //GameManager.Instance.AddBatteriesToResetPool(cs);

        //only exectue OnPlayerEnter if the player collides with this token.
        var _player = other.gameObject.GetComponent<_Project.RGScripts.Player.Player>();
        if (_player != null) OnPlayerEnter(_player);
    }

    void OnPlayerEnter(_Project.RGScripts.Player.Player player)
    {
        if (collected) return;
        player.CanDash = true;
        //SoundManager.Instance.PlaySESound(SoundManager.eSoundChannel.SE01, SoundManager.eSoundEffect.Hurt_Reverb);
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

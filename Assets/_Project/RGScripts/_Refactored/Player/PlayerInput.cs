using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.RGScripts.Player
{
    public class PlayerInput : MonoBehaviour
    {
        public Vector2 MovementInput;

        public bool JumpPressed;
        public bool JumpRelease;

        private void Update()
        {
            MovementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            JumpPressed = Input.GetKeyDown(KeyCode.Space);
            JumpRelease = Input.GetKeyUp(KeyCode.Space);
        }
    }
}
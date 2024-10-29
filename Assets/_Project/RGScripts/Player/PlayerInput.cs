using UnityEngine;

namespace _Project.RGScripts.Player
{
    public class PlayerInput : MonoBehaviour
    {
        public Vector2 MovementInput;
        
        public bool Jump => Input.GetKeyDown(KeyCode.Space);
        public bool JumpRelease => Input.GetKeyUp(KeyCode.Space);

        private void Update()
        {
            MovementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
    }
}
using UnityEngine;

namespace _Project.RGScripts.Player
{
    [SelectionBase]
    [RequireComponent(typeof(NewPlayerMovement))]
    public class Player : MonoBehaviour
    {
        private NewPlayerMovement _newPlayerMovement;

        private void Awake()
        {
            _newPlayerMovement = GetComponent<NewPlayerMovement>();
        }
    }
}
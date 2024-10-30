using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.RGScripts._Refactored.UI
{
    public class UI_Battery : MonoBehaviour
    {
        [SerializeField] private Player.Player _player;
        private Slider slider;

        private void Awake()
        {
            slider = GetComponent<Slider>();
        }

        private void Update()
        {
            slider.value = Mathf.InverseLerp(0, _player._playerConfig.MaxEnergy, _player.currentEnergy);
        }
    }
}
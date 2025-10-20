using UnityEngine;

namespace Memoria.Entity
{
    [RequireComponent(typeof(PlayerController))]
    public class Player : Entity
    {
        private PlayerController controller;

        private void Awake()
        {
            controller = GetComponent<PlayerController>();
            controller.Initialize(this);
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

namespace Memoria.Entity
{
    public class PlayerController : MonoBehaviour
    {
        private Player player;
        private GameInput input;
        private Vector2 moveInput;
 
        public void Initialize(Player player)
        {
            this.player = player;
        }

        private void Awake()
        {
            input = new GameInput();
            input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            input.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        }

        private void OnEnable() => input.Player.Enable();
        private void OnDisable() => input.Player.Disable();

        private void Update()
        {
            Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
            player.transform.Translate(move * Time.deltaTime * 5f);
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;
using Memoria.Systems;
using Memoria.Constants;

namespace Memoria.Entity
{
    public class PlayerController : MonoBehaviour
    {
        private Player player;
        private Animator animator;
        private GameInput input;
        private Vector2 moveInput;

        private float playerMoveSpeed;

        private void Awake()
        {
            player = GetComponent<Player>();
            animator = GetComponent<Animator>();
            input = new GameInput();
            input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            input.Player.Move.canceled += ctx => moveInput = Vector2.zero;

            playerMoveSpeed = ParameterManager.GetParam<float>(PlayerStates.PLAYER_MOVE_SPEED, 5f);
        }

        private void OnEnable() => input.Player.Enable();
        private void OnDisable() => input.Player.Disable();

        private void Update()
        {
            Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
            transform.Translate(move * Time.deltaTime * playerMoveSpeed);

            if (move != Vector3.zero)
                animator.SetBool("isWalking", true);
            else
                animator.SetBool("isWalking", false);
        }
    }
}
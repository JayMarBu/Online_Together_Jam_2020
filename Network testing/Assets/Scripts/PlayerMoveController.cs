using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : NetworkBehaviour
{
    [SerializeField] private float movement_speed_ = 5.0f;
    [SerializeField] private CharacterController controller = null;

    private Vector2 previous_input_;

    private Controls controls_;
    private Controls controls 
    {
        get 
        {
            if (controls_ != null) { return controls_; }
            return controls_ = new Controls();
        }
    }

    public override void OnStartAuthority()
    {
        enabled = true;

        controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
        controls.Player.Move.canceled += ctx => ResetMovement();
    }

    [ClientCallback]
    private void OnEnable() => controls.Enable();

    [ClientCallback]
    private void OnDisable() => controls.Disable();

    [ClientCallback]
    private void Update() => Move();

    [Client]
    private void SetMovement(Vector2 movement) => previous_input_ = movement;

    [Client]
    private void ResetMovement() => previous_input_ = Vector2.zero;

    [Client]
    private void Move()
    {
        Vector3 right = controller.transform.right;
        Vector3 forward = controller.transform.forward;

        right.y = 0f;
        forward.y = 0f;

        Vector3 movement = right.normalized * previous_input_.x + forward.normalized * previous_input_.y;

        controller.Move(movement * movement_speed_ * Time.deltaTime);
    }
}

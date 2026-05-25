using Godot;

public sealed class PlayerMoveController
{
    private Vector3 _velocity = Vector3.Zero;

    public Vector3 Tick(
        Vector2 moveInput,
        bool isGrounded,
        double delta)
    {
        if (!isGrounded)
        {
            _velocity.Y -= 75f * (float)delta;
        }

        _velocity.X = moveInput.X * 14f;
        _velocity.Z = moveInput.Y * 14f;

        return _velocity;
    }
}

using Godot;

public partial class PlayerNode : CharacterBody3D
{
    private readonly PlayerMoveController
        _moveController = new();

    public override void _PhysicsProcess(double delta)
    {
        Vector2 moveInput = Input.GetVector(
            "move_left",
            "move_right",
            "move_forward",
            "move_backward");

        Velocity = _moveController.Tick(
            moveInput,
            IsOnFloor(),
            delta);

        MoveAndSlide();
    }
}

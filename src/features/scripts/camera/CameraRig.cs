using Godot;

public partial class CameraRig
    : Node3D
{
    private Node3D _target;

    private Node3D _yawPivot;

    private Node3D _pitchPivot;

    private float _pitch;

    public Basis CameraBasis =>
        _yawPivot.GlobalBasis;

    public override void _Ready()
    {
        Input.MouseMode =
            Input.MouseModeEnum.Captured;

        _yawPivot =
            GetNode<Node3D>(
                "YawPivot");

        _pitchPivot =
            GetNode<Node3D>(
                "YawPivot/PitchPivot");
    }

    public override void _Input(
        InputEvent e)
    {
        if (e is not InputEventMouseMotion motion)
        {
            return;
        }

        RotateYaw(
            motion.Relative.X);

        RotatePitch(
            motion.Relative.Y);
    }

    public override void _Process(
        double delta)
    {
        if (_target == null)
        {
            return;
        }

        GlobalPosition =
            _target.GlobalPosition;
    }

    public void SetTarget(
        Node3D target)
    {
        _target = target;
    }

    private void RotateYaw(
        float deltaX)
    {
        _yawPivot.RotateY(
            Mathf.DegToRad(
                -deltaX * 0.15f));
    }

    private void RotatePitch(
        float deltaY)
    {
        _pitch +=
            -deltaY * 0.15f;

        _pitch = Mathf.Clamp(
            _pitch,
            -80f,
            80f);

        _pitchPivot.RotationDegrees =
            new Vector3(
                _pitch,
                0,
                0);
    }
}

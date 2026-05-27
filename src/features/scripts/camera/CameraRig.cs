using Godot;

// Camera に関する管理の責務を持つクラス
public partial class CameraRig : Node3D
{
    private Node3D _target; // Camera が中央に捉えるターゲットとなる Node を入れるための変数を用意しておく。基本的にはプレイヤーが入る前提。
    private Node3D _yawPivot; // Camera　が横方向の視点の基準としてとらえる Node を入れるための変数を用意しておく
    private Node3D _pitchPivot; // Camera が上下方向の視点の基準としてとらえる Node を入れるための変数を用意しておく

    private float _pitch; // カメラの上下を度数法で保持するための変数を用意しておく
    public Basis CameraBasis =>　_yawPivot.GlobalBasis; // Camera の回転情報を扱っている Basis 型の値を保持するための変数を用意しておく

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;　// Godot が用意しているマウス制御用の設定を使って、マウスカーソルを固定して非表示にする。
        _yawPivot = GetNode<Node3D>("YawPivot");　// Godot Editor で設定した Node を検索して取得し、変数に格納する。
        _pitchPivot = GetNode<Node3D>("YawPivot/PitchPivot"); // Godot Editor で設定した Node を検索して取得し、変数の格納する。
    }

    // 何かキー入力があった時に発火する Node3D が持っているメソッド
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

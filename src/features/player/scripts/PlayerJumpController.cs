using Godot;

// プレイヤーのジャンプについて責務を持つクラス
public sealed class PlayerJumpController
{
    private const float Gravity = 75f; // 重力の大きさ
    private const float JumpPower = 18f; // ジャンプ力

    public Vector3 Tick(
        bool grounded, // プレイヤーが地表に足を付けているかどうか
        bool jumpPressed, // ジャンプの入力がされているかどうか
        Vector3 velocity, // CharacterBody3D のプロパティ。プレイヤーの移動速度ベクトル。
        double delta)　// 前の tick からの差の時間 (現実の時間)
    {
        if (!grounded) // もし地面にいない場合は、
        {
            velocity.Y -= Gravity * (float)delta; // プレイヤーのY方向の移動速度ベクトルから、重力の大きさの分だけ速度を小さくする。
        }

        if (grounded && jumpPressed) // もし地面にいて、ジャンプの入力がされていれば、
        {
            velocity.Y = JumpPower; // プレイヤーのY方向の移動速度ベクトルに、ジャンプ力を加える。
        }

        return velocity; // 最終的な移動速度ベクトルを返す。
    }
}

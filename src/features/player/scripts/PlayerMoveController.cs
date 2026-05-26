using Godot;

// プレイヤーの移動について責務を持つクラス
public sealed class PlayerMoveController
{
    private const float WalkSpeed = 5f; // 歩くスピード
    private const float SprintSpeed = 9f; // 走るスピード
    private const float CrouchSpeed = 2f; // しゃがんだ時のスピード

    // Vector3 型の返り値の public な(公開されている) メソッド (関数)
    public Vector3 Tick(
        Vector2 moveInput, // キー入力
        bool sprint, // 走る入力がされているかどうか
        bool crouching, // しゃがむ入力がされているかどうか
        Vector3 velocity) // CharacterBody3D のプロパティ。プレイヤーの移動速度ベクトル。
    {

        float speed; // speed という変数を宣言し、初期化はせずにおいておく。
        if (crouching) speed = CrouchSpeed; // しゃがみ入力があるなら、しゃがんだ時のスピードにする。
        else if (sprint) speed = SprintSpeed; // 走る入力があるなら、走るスピードにする。
        else speed = WalkSpeed; // 特に入力がない場合は、歩くスピードにする。

        velocity.X = moveInput.X * speed; // 入力から得た speed の情報を使って、X方向の速度を決定する。
        velocity.Z = moveInput.Y * speed; // 入力から得た speed の情報を使って、Y方向の速度を決定する。

        return velocity; // 最終的な移動速度ベクトルを返す。
    }
}

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
        Basis cameraBasis, // Camera の
        bool sprint, // 走る入力がされているかどうか
        bool crouching, // しゃがむ入力がされているかどうか
        Vector3 velocity) // CharacterBody3D のプロパティ。プレイヤーの移動速度ベクトル。
    {

        float speed; // speed という変数を宣言し、初期化はせずにおいておく。
        if (crouching) speed = CrouchSpeed; // しゃがみ入力があるなら、しゃがんだ時のスピードにする。
        else if (sprint) speed = SprintSpeed; // 走る入力があるなら、走るスピードにする。
        else speed = WalkSpeed; // 特に入力がない場合は、歩くスピードにする。

        Vector3 forward = -cameraBasis.Z; // 前後方向のベクトルをとりあえず マイナスの CameraBasis.Z にして初期化しておく
        Vector3 right = cameraBasis.X; // 左右方向のベクトルをとりあえず プラスの CameraBasis.X にして初期化しておく

        // MoveController では、Y方向の移動に関しては扱わないので、Y方向の速度に関しては0にしておく
        forward.Y = 0;
        right.Y = 0;

        forward = forward.Normalized(); // 前後方向のベクトルを正規化して、長さ1 の方向だけを持ったベクトルにする。
        right = right.Normalized(); // 左右方向のベクトルを正規化して、長さ1 の方向だけを持ったベクトルにする。
        // ↑ ベクトル正規化については、数C とかで習うはず

        // 前後方向のベクトルと、左右方向のベクトルを 移動入力に合わせて方向を決定し、足して一つのベクトルにする
        Vector3 moveDirection = (forward * moveInput.Y) + (right * moveInput.X);
        moveDirection = moveDirection.Normalized(); // 足したベクトルをもう一度正規化して、方向だけを持ったベクトルにしておく。

        // さっき求めた speed をかけ合わせて、最終的な移動速度ベクトルにする
        velocity.X = moveDirection.X * speed;
        velocity.Z = moveDirection.Z * speed;

        return velocity; // 最終的な移動速度ベクトルを返す。
    }
}

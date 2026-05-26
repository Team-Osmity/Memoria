using Godot;

// プレイヤーに関する script 全体を管理するクラス。
// CharacterBody3D を継承しており、Godot との接点となっている。
public partial class PlayerManager : CharacterBody3D
{
    private readonly PlayerMoveController _moveController = new(); // PlayerMoveController クラスのインスタンスを作成する。
    private readonly PlayerJumpController _jumpController = new(); // PlayerJumpController クラスのインスタンスを作成する。

    // PlayerAnimationController 型の変数を用意しておく。
    // _Ready() 発火後に取得する予定の AnimationPlayer を引数として使いたいので、この段階では初期化しない
    private PlayerAnimationController _animationController;

    // 現在は仮の model を使っているので、後から置き換えやすいように、AnimationPlayer の Path を定数として定義しておく。
    private const string AnimationPlayerPath = "pre_model/AnimationPlayer";

    // Node が初期化されたときに発火するメソッド
    public override void _Ready() //
    {
        AnimationPlayer _animationPlayer = GetNode<AnimationPlayer>(AnimationPlayerPath); // Godot Editor で設定してある model から AnimationPlayer Node を取得する。
        _animationController = new PlayerAnimationController(_animationPlayer); // PlayerAnimationController クラスのインスタンスを作成する。
    }

    // 物理計算などをする際に使うメソッド (unity の FixedUpdate みたいな)
    public override void _PhysicsProcess(double delta)
    {
        // WASD 系のキー入力をいい感じに Vector2 に変換してくれる CharacterBody3D の便利メソッド
        Vector2 moveInput = Input.GetVector(
            "move_left", // 左方入力
            "move_right", // 右方入力
            "move_forward", // 前方入力
            "move_backward"); // 後方入力

        bool sprint = Input.IsActionPressed("sprint"); // sprint (走る) のキー入力を取得する
        bool jump = Input.IsActionPressed("jump"); // jump (ジャンプ) のキー入力を取得する
        bool crouching = Input.IsActionPressed("crouch"); // crouch (しゃがむ) のキー入力をちゅ得する

        Velocity = _moveController.Tick(moveInput, sprint, crouching, Velocity); // 入力から実際のXZ方向の移動速度ベクトルを計算する。
        Velocity = _jumpController.Tick(IsOnFloor(), jump, Velocity, delta); // 入力から実際のY方向の移動速度ベクトルを計算する。

        MoveAndSlide(); // Velocity プロパティを使って、実際に移動してくれる CharacterBody3D の便利メソッドを実行
        UpdateAnimation(moveInput, sprint, crouching); // 入力を Animation の見た目に反映する
    }

    // PlayerAnimationController を使って、Animation を再生する自作のメソッド
    private void UpdateAnimation(
        Vector2 moveInput, // WASD 系の移動キー入力
        bool sprint, // 走っているかどうかのキー入力
        bool crouching) // しゃがんでいるかどうかのキー入力
    {
        if (!IsOnFloor()) // もし地面にいない場合は、ジャンプしているとみなして、
        {
            _animationController.PlayJump(); // ジャンプのアニメーションを再生して、
            return; // それ以降の処理を実行せず、このメソッドを終了する。
        }

        bool moving = moveInput.Length() > 0.1f; // 止まっているか動いているかを判定する。true で動いている。

        if (crouching) // もししゃがんでいて、
        {
            if (moving) // 動いているなら、
            {
                _animationController.PlayCrouchWalk(); // しゃがみ歩きアニメーションを再生する。
            }
            else // 止まっているなら (idle なら)
            {
                _animationController.PlayCrouchIdle(); // しゃがみ待機アニメーションを再生する。
            }

            return; // それ以降の処理を実行せず、このメソッドを終了する。
        }

        // 以降「もし しゃがんでおらず、」

        if (moving) // もし動いていて、
        {
            if (sprint) // 走っているなら、
            {
                _animationController.PlaySprint(); // 走るアニメーションを再生する。
            }
            else // 走っていないなら
            {
                _animationController.PlayWalk(); // 歩きアニメーションを再生する。
            }

            return; // それ以降の処理を実行せず、このメソッドを終了する。
        }

        // 以降「もし 動いていないなら、」

        _animationController.PlayIdle(); // 待機アニメーションを再生する。
    }
}

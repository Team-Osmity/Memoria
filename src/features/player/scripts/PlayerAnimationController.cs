using Godot;

// プレイヤーのアニメーションについての責務を持つクラス
public sealed class PlayerAnimationController
{
    private readonly AnimationPlayer _animationPlayer;　// AnimationPlayer 型の変数を用意しておく
    private string _currentAnimation = ""; // string 型の Animation 管理用の変数を用意しておく

    // このクラスのコンストラクタ
    public PlayerAnimationController(
        AnimationPlayer animationPlayer)
    {
        _animationPlayer = animationPlayer; // コンストラクタで AnimationPlayer プロパティを初期化する
    }

    public void PlayIdle()
    {
        Play("Idle");
    }

    public void PlayWalk()
    {
        Play("Walk");
    }

    public void PlaySprint()
    {
        Play("Sprint");
    }

    public void PlayJump()
    {
        Play("Jump");
    }

    public void PlayCrouchIdle()
    {
        Play("Crouch_Idle");
    }

    public void PlayCrouchWalk()
    {
        Play("Crouch_Fwd");
    }

    // string 型の animationName 引数を元に Animation を再生する自作メソッド
    private void Play(
        string animationName)
    {
        if (_currentAnimation == animationName) // もし、現在再生しているものと同じなら、
        {
            return; // 何もせず終わる
        }

        // そうじゃないなら、
        _currentAnimation = animationName; // 現在再生中である AnimationName を管理している変数の値を書き換え、

        _animationPlayer.Play(animationName); // AnimationPlayer 型を介して、アニメーションを再生する。
    }
}

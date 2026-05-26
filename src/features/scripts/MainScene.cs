using Godot;

// Main Scene を管理する責務を持つクラス
public partial class MainScene : Node
{
	// Node が初期化されたときに発火するメソッド
	public override void _Ready()
	{
		PlayerNode playerNode = GetNode<PlayerNode>("PlayerNode"); // 子Node から PlayerNode を検索して取得する
		CameraRig cameraRig = GetNode<CameraRig>("CameraRig"); // 子Node から CameraRig を検索して取得する
		cameraRig.SetTarget(playerNode);　// Camera の target (player) を設定する
		playerNode.SetCameraRig(cameraRig); // PlayerNode の cameraRig を設定する
	}
}

using Godot;

public partial class PauseMenu
    : Control
{
    private Button _exitButton;

    private bool _opened;

    public override void _Ready()
    {
        _exitButton =
            GetNode<Button>(
                "PanelContainer/VBoxContainer/ExitButton");

        _exitButton.Pressed +=
            OnExitPressed;

        Visible = false;
    }

    public override void _UnhandledInput(
        InputEvent e)
    {
        if (!e.IsActionPressed(
                "ui_cancel"))
        {
            return;
        }

        Toggle();
    }

    private void Toggle()
    {
        _opened = !_opened;

        Visible = _opened;

        GetTree().Paused =
            _opened;

        Input.MouseMode =
            _opened
                ? Input.MouseModeEnum.Visible
                : Input.MouseModeEnum.Captured;
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }
}

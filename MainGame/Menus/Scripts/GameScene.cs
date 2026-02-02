using Godot;
using System;

public partial class GameScene : Node
{
	PauseMenu PMenu;

	[Signal]
	public delegate void QuitToMainMenuEventHandler();

	public override void _Ready()
	{
		base._Ready();

		PMenu = GetNode<PauseMenu>("PauseMenu");
		PMenu.QuitButtonPressed += () => QuitButtonPressed();
	}

	private void QuitButtonPressed()
	{
		EmitSignal(SignalName.QuitToMainMenu);
	}

}

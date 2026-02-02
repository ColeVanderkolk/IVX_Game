using Godot;
using System;

public partial class GameScene : Node
{

	PauseMenu PMenu;

	public override void _Ready()
	{
		base._Ready();

		PMenu = GetNode<PauseMenu>("PauseMenu");
	}

}

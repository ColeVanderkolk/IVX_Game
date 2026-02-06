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
	
	private void _on_timer_timeout()
	{
		var horde = GetNode("HordeManager/Horde") as Node; horde.Call("addUnit");
	}
	private void _on_timer_2_timeout()
	{
		var hordeScene = GD.Load<PackedScene>("res://Units/Scenes/horde.tscn");
		var newHorde = hordeScene.Instantiate<Node3D>();
		newHorde.GlobalPosition = new Vector3(0, 4.5f, 0);
		newHorde.Call("addUnit", 2);
		newHorde.Call("addUnit", 2);
		newHorde.Call("addUnit", 2);
		AddChild(newHorde);
		GD.Print("added Horde!!!");
	}
}

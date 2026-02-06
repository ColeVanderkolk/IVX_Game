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
		
		var horde = GetNode("Buildings/GateBuilding3") as gate_buildingscript;
		horde.Call("Assimilate");
	}

	private void QuitButtonPressed()
	{
		EmitSignal(SignalName.QuitToMainMenu);
	}
	
	private void _on_timer_timeout()
	{
		var horde = GetNode("Horde2") as Node; horde.Call("addUnit", 1);
		horde.Call("addUnit", 1);
		horde.Call("addUnit", 1);
		
		var horde2 = GetNode("Horde3") as Node; horde.Call("addUnit", 1);
		horde2.Call("addUnit", 1);
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

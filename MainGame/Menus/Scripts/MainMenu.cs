using Godot;
using System;

public partial class MainMenu : Menu
{

	public override void _Ready()
	{
		base._Ready();
		GD.Print("Main Menu successfully loaded in!");
	}

}

using Godot;
using System;

public partial class GameController : Node
{

	public const string MainMenuFilepath = "res://Menus/Scenes/main_menu.tscn";
	private Menu MainMenu;

	public GameController()
	{
	}

	public override void _Ready()
	{
		base._Ready();
		
		// Instantiate a Main Menu & add it to the scene tree
		Menu TempMainMenu = (Menu)ResourceLoader.Load<PackedScene>(MainMenuFilepath).Instantiate();
		MainMenu = TempMainMenu;
		AddChild(MainMenu);

	}

}

using Godot;
using System;

public partial class GameController : Node
{

	public const string MainMenuFilepath = "res://Menus/Scenes/main_menu.tscn";

	private MainMenu MainMenu;

	public GameController()
	{
	}

	// called when the node enters the tree
	public override void _Ready()
	{
		base._Ready();
		
		// Instantiate a Main Menu & add it to the scene tree
		MainMenu = LoadMainMenu();
		AddChild(MainMenu);

	}

	// Load and return a new MainMenu node. Initialize & connect its signals.
	private MainMenu LoadMainMenu()
	{
		MainMenu TempMenu = (MainMenu)ResourceLoader.Load<PackedScene>(MainMenuFilepath).Instantiate();
		TempMenu.QuitButtonPressed += () => QuitGame();

		return TempMenu;
	}

	private void QuitGame()
	{
		GD.Print("Quitting game.");
	}

}

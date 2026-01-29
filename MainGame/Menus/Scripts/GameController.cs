using Godot;
using System;

public partial class GameController : Node
{

	public const string MainMenuFilepath = "res://Menus/Scenes/main_menu.tscn";

	private MainMenu CurrentMainMenu; // currently loaded main menu

	public GameController()
	{
	}

	// called when the node enters the tree
	public override void _Ready()
	{
		base._Ready();
		
		// Instantiate a Main Menu & add it to the scene tree
		CurrentMainMenu = LoadMainMenu();
		AddChild(CurrentMainMenu);

	}

	// Load and return a new MainMenu node. Initialize & connect its signals.
	private MainMenu LoadMainMenu()
	{
		if (CurrentMainMenu is not null)
		{
			GD.Print("WARNING! There is a currently loaded main menu.");
		}

		MainMenu TempMenu = (MainMenu)ResourceLoader.Load<PackedScene>(MainMenuFilepath).Instantiate();
		TempMenu.QuitButtonPressed += () => QuitGame();

		return TempMenu;
	}

	private void QuitGame()
	{
		GD.Print("Quitting game.");
	}

	

}

using Godot;
using System;

public partial class GameController : Node
{

	public const string MainMenuFilepath = "res://Menus/Scenes/main_menu.tscn";

	private MainMenu CurrentMainMenu; // currently loaded main menu
	private Menu CurrentOpenedMenu; // menu currently on screen

	public GameController()
	{
	}

	// called when the node enters the tree
	public override void _Ready()
	{
		base._Ready();
		
		// Instantiate a Main Menu & add it to the scene tree
		CurrentMainMenu = LoadMainMenu();
		OpenMenu(CurrentMainMenu);

	}

	// opens the menu: Removes the current Menu from the scene tree.
	// Adds the menu M to the scene tree & makes it the current Menu.
	private void OpenMenu(Menu M)
	{
		if (CurrentOpenedMenu is not null)
		{
			RemoveChild(CurrentOpenedMenu);
		}

		CurrentOpenedMenu = M;
		AddChild(CurrentOpenedMenu);
	}

	// Load and return a new MainMenu node. Initialize & connect its signals.
	private MainMenu LoadMainMenu()
	{
		if (CurrentMainMenu is not null) // Ideally a mainMenu shouldn't be loaded while one is active :P
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
		GetTree().Quit();
	}

	

}

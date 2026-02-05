using Godot;
using System;
using System.Threading.Tasks;

public partial class GameController : Node
{

	public const string MainMenuFilepath = "res://Menus/Scenes/main_menu.tscn";
	public const string OptionsMenuFilepath = "res://Menus/Scenes/options_menu.tscn";
	public const string GameSceneFilepath = "res://Menus/Scenes/game_scene.tscn";

	private TransitionHandler TransHandler; // TransitionHandler used to play scene Transition Effects.

	// Menu enum used by TransitionToMenu.
	// This way the method can check whether a menu is loaded before loading it in.
	enum Screens
	{
		MAINMENU,
		OPTIONSMENU,
		GAMESCENE
	}

	private MainMenu CurrentMainMenu; // currently loaded main menu
	private OptionsMenu CurrentOptionsMenu; // currently loaded options menu
	private Node CurrentGameScene; // currently loaded game scene
	private Node CurrentOpenedScreen; // menu currently on screen

	public GameController()
	{
	}

	// called when the node enters the tree
	public override void _Ready()
	{
		base._Ready();
		
		// Instantiate TransitionHandler
		TransHandler = GetNode<TransitionHandler>("TransitionHandler");

		// Instantiate a Main Menu & add it to the scene tree
		CurrentMainMenu = LoadMainMenu();
		OpenScreen(CurrentMainMenu);
	}

	// opens the menu: Removes the current Menu from the scene tree.
	// Adds the menu M to the scene tree & makes it the current Menu.
	private void OpenScreen(Node S)
	{
		if (CurrentOpenedScreen is not null)
		{
			RemoveChild(CurrentOpenedScreen);
		}

		CurrentOpenedScreen = S;
		AddChild(CurrentOpenedScreen);

		// Display a new background image when displaying the main menu screen:
		if (CurrentOpenedScreen == CurrentMainMenu)
		{
			CurrentMainMenu.loadNewBackgroundImage();
		}
	}

	// Plays a transition effect & opens the specified screen.
	// If the specified menu hasn't been loaded, load it.
	// Functionality for opening the game screen is coming soon.
	// Enum is used so you can call this function without knowing whether the menu is loaded in.
	private async Task TransitionToScreen(Screens S)
	{
		Node NewScreen = CurrentMainMenu;
		// NewMenu is initialized to the CurrentMainMenu
		// But will be changed depending on which menu is specified.
	
		// load menus if they are not loaded in.
		// Set NewMenu equal to correct menu.
		if (S == Screens.MAINMENU)
		{
			if (CurrentMainMenu is null)
			{
				CurrentMainMenu = LoadMainMenu();
			}
			NewScreen = CurrentMainMenu;
		} else if (S == Screens.OPTIONSMENU)
		{
			if (CurrentOptionsMenu is null)
			{
				CurrentOptionsMenu = LoadOptionsMenu();
			}
			NewScreen = CurrentOptionsMenu;
		} else if (S == Screens.GAMESCENE)
		{
			if (CurrentGameScene is null)
			{
				CurrentGameScene = LoadGameScene();
			}
			NewScreen = CurrentGameScene;
		}

		// Swap to new menu while screen is covered in transition.
		await TransHandler.TransitionIn();

		OpenScreen(NewScreen);

		await TransHandler.TransitionOut();
	}

	//-------------------------//
	// SCENE LOADING FUNCTIONS //
	//-------------------------//

	// Load and return a new MainMenu node. Initialize it & connect its signals.
	private MainMenu LoadMainMenu()
	{
		if (CurrentMainMenu is not null) // Ideally a mainMenu shouldn't be loaded while one is active :P
		{
			GD.Print("WARNING! There is a currently loaded main menu.");
		}

		MainMenu TempMenu = (MainMenu)ResourceLoader.Load<PackedScene>(MainMenuFilepath).Instantiate();
		TempMenu.StartButtonPressed += () => MainMenuStartButtonPressed();
		TempMenu.QuitButtonPressed += () => QuitGame();
		TempMenu.OptionsButtonPressed += () => MainMenuOptionsButtonPressed();

		return TempMenu;
	}

	// Load and return a new OptionsMenu node. Initialize it & connect its signals.
	private OptionsMenu LoadOptionsMenu()
	{
		if (CurrentOptionsMenu is not null)
		{
			GD.Print("WARNING! There is a currently loaded options menu.");
		}

		OptionsMenu TempMenu = (OptionsMenu)ResourceLoader.Load<PackedScene>(OptionsMenuFilepath).Instantiate();
		TempMenu.BackButtonPressed += () => OptionsMenuBackButtonPressed();

		return TempMenu;
	}

	private Node LoadGameScene()
	{
		if (CurrentGameScene is not null)
		{
			GD.Print("WARNING! There is a currently loaded game scene.");
		}

		GameScene tempScene = (GameScene)ResourceLoader.Load<PackedScene>(GameSceneFilepath).Instantiate();
		tempScene.QuitToMainMenu += () => PauseMenuQuitToMainPressed();

		return tempScene; 
	}

	//---------------------------//
	// Scene Unloading Functions //
	//---------------------------//

	// Unload menu scenes if they exist. This is to prevent background memory usage slowing the game.
	// Set references to null. This makes sure the code works & it probably helps with memory.
	private void UnloadMenus()
	{
		GD.Print("Unloading Menus.");

		if (CurrentMainMenu is not null)
		{
			CurrentMainMenu.QueueFree();
		}
		CurrentMainMenu = null;

		if (CurrentOptionsMenu is not null)
		{
			CurrentOptionsMenu.QueueFree();
		}
		CurrentOptionsMenu = null;
	}

	// Unload game scene if it exists. This is to prevent background memory usage slowing the game.
	private void UnloadGameScene()
	{
		GD.Print("Unloading Game Scene.");

		if (CurrentGameScene is not null)
		{
			CurrentGameScene.QueueFree();
		}
		CurrentGameScene = null;
	}

	//------------------------//
	// Button Press Functions //
	//------------------------//

	private async Task MainMenuStartButtonPressed()
	{
		GD.Print("Opening Game Scene.");

		await TransitionToScreen(Screens.GAMESCENE);
		UnloadMenus();
	}

	private void MainMenuOptionsButtonPressed()
	{
		GD.Print("Opening Options Menu.");

		TransitionToScreen(Screens.OPTIONSMENU);
	}

	private void OptionsMenuBackButtonPressed()
	{
		GD.Print("Openeing Main Menu.");

		TransitionToScreen(Screens.MAINMENU);
	}

	private async Task PauseMenuQuitToMainPressed()
	{
		GD.Print("Opening Main Menu.");

		await TransitionToScreen(Screens.MAINMENU);
		UnloadGameScene();
	}

	private async Task QuitGame()
	{
		GD.Print("Quitting game.");

		// Play the Transition In before closing the game.
		await TransHandler.TransitionIn();

		GetTree().Quit();
	}
}

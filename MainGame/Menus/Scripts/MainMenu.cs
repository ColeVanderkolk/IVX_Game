using Godot;
using System;

public partial class MainMenu : Control
{
	public const string bgRedFilepath = "res://Assets/menuBackgrounds/bg-red.png";
	public const string bgWhiteFilepath = "res://Assets/menuBackgrounds/bg-white.png";
	public const string bgBlueFilepath = "res://Assets/menuBackgrounds/bg-blue.png";
	public const string bgBlackFilepath = "res://Assets/menuBackgrounds/bg-black.png";

	private string[] backgrounds = {bgRedFilepath, bgBlueFilepath, bgWhiteFilepath, bgBlackFilepath};

	// Buttons:
	private Button StartButton;
	private Button QuitButton;
	private Button OptionsButton;

	private TextureRect rect;

	// Signals:
	[Signal]
	public delegate void StartButtonPressedEventHandler();

	[Signal]
	public delegate void QuitButtonPressedEventHandler();

	[Signal]
	public delegate void OptionsButtonPressedEventHandler();

	public override void _Ready()
	{
		base._Ready();
		
		// Initialize buttons
		StartButton = GetNode<Button>("MarginContainer/VBoxContainer/StartButton");
		QuitButton = GetNode<Button>("MarginContainer/VBoxContainer/QuitButton");
		OptionsButton = GetNode<Button>("MarginContainer/VBoxContainer/OptionsButton");

		// Connect button signals
		StartButton.ButtonDown += () => OnStartButtonPressed();
		OptionsButton.ButtonDown += () => OnOptionsButtonPressed();
		QuitButton.ButtonDown += () => OnQuitButtonPressed();

		// get texture rect:
		rect = GetNode<TextureRect>("TextureRect");
	}

	public void loadNewBackgroundImage()
	{
		Random rng = new Random();
		int index = rng.Next(0, 4);

		string bgFilepath = backgrounds[index];

		GD.Print(backgrounds[index]);

		Texture2D Img = ResourceLoader.Load<Texture2D>(bgFilepath);
		//ImageTexture Imgtx = ImageTexture.CreateFromImage(Img);

		GD.Print(Img);
		//GD.Print(Imgtx);

		rect.Texture = Img;
	}

	private void OnStartButtonPressed()
	{
		GD.Print("Start Button Pressed!");
		EmitSignal(SignalName.StartButtonPressed);
		var audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		audioPlayer.Stream = GD.Load<AudioStream>("res://Assets/Audio/SFX/sfx_uibuttonsound.mp3");
		audioPlayer.Play();
	}

	private void OnOptionsButtonPressed()
	{
		GD.Print("Options Button Pressed!");
		EmitSignal(SignalName.OptionsButtonPressed);
		var audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		audioPlayer.Stream = GD.Load<AudioStream>("res://Assets/Audio/SFX/sfx_uibuttonsound.mp3");
		audioPlayer.Play();
	}

	private void OnQuitButtonPressed()
	{
		GD.Print("Quit Button Pressed!");
		EmitSignal(SignalName.QuitButtonPressed);
		var audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		audioPlayer.Stream = GD.Load<AudioStream>("res://Assets/Audio/SFX/sfx_uibuttonsound.mp3");
		audioPlayer.Play();
	}
}

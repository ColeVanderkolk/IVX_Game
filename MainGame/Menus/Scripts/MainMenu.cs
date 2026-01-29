using Godot;
using System;

public partial class MainMenu : Menu
{
	private Button StartButton;
	private Button QuitButton;

	[Signal]
	public delegate void StartButtonPressedEventHandler();

	[Signal]
	public delegate void QuitButtonPressedEventHandler();

	public override void _Ready()
	{
		base._Ready();
		
		StartButton = GetNode<Button>("MarginContainer/VBoxContainer/StartButton");
		QuitButton = GetNode<Button>("MarginContainer/VBoxContainer/QuitButton");

		StartButton.ButtonDown += () => OnStartButtonPressed();
		QuitButton.ButtonDown += () => OnQuitButtonPressed();
	}

	private void OnStartButtonPressed()
	{
		GD.Print("Start Button Pressed!");
		EmitSignal(SignalName.StartButtonPressed);
	}

	private void OnQuitButtonPressed()
	{
		GD.Print("Quit Button Pressed!");
		EmitSignal(SignalName.QuitButtonPressed);
	}

}

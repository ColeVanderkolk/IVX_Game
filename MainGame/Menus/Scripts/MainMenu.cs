using Godot;
using System;

public partial class MainMenu : Menu
{
	private Button StartButton;
	private Button QuitButton;
	private Button OptionsButton;

	[Signal]
	public delegate void StartButtonPressedEventHandler();

	[Signal]
	public delegate void QuitButtonPressedEventHandler();

	[Signal]
	public delegate void OptionsButtonPressedEventHandler();

	public override void _Ready()
	{
		base._Ready();
		
		// Initialize buttons and connect signals
		StartButton = GetNode<Button>("MarginContainer/VBoxContainer/StartButton");
		QuitButton = GetNode<Button>("MarginContainer/VBoxContainer/QuitButton");
		OptionsButton = GetNode<Button>("MarginContainer/VBoxContainer/OptionsButton");

		StartButton.ButtonDown += () => OnStartButtonPressed();
		OptionsButton.ButtonDown += () => OnOptionsButtonPressed();
		QuitButton.ButtonDown += () => OnQuitButtonPressed();
	}

	private void OnStartButtonPressed()
	{
		GD.Print("Start Button Pressed!");
		EmitSignal(SignalName.StartButtonPressed);
	}

	private void OnOptionsButtonPressed()
	{
		GD.Print("Options Button Pressed!");
		EmitSignal(SignalName.OptionsButtonPressed);
	}

	private void OnQuitButtonPressed()
	{
		GD.Print("Quit Button Pressed!");
		EmitSignal(SignalName.QuitButtonPressed);
	}
}

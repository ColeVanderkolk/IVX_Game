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
		EmitSignal(SignalName.StartButtonPressed);
        GD.Print("Start Button Pressed!");
	}

	private void OnQuitButtonPressed()
	{
		EmitSignal(SignalName.QuitButtonPressed);
        GD.Print("Quit Button Pressed!");
	}

}

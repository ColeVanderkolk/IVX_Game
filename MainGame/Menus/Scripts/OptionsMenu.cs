using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class OptionsMenu : Menu
{
	// Buttons
	private Button BackButton;

	// Signals
	[Signal]
	public delegate void BackButtonPressedEventHandler();

	public override void _Ready()
	{
		base._Ready();
		
		// Initialize buttons
		BackButton = GetNode<Button>("MarginContainer/VBoxContainer/TopBar/BackButton");

		// Connect button signals
		BackButton.ButtonDown += () => OnBackButtonPressed();
	}

	private void OnBackButtonPressed()
	{
		GD.Print("Back Button Pressed!");
		EmitSignal(SignalName.BackButtonPressed);
	}
}

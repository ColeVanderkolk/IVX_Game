using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class OptionsMenu : Menu
{
	private Button BackButton;


	public override void _Ready()
	{
		base._Ready();
		
		BackButton = GetNode<Button>("MarginContainer/VboxContainer/TopBar/BackButton");
	}

	private void OnBackButtonPressed()
	{
		GD.Print("Back Button Pressed!");
	}
}

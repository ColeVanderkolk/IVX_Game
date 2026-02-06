using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{

	private bool IsActive;

	private Button ReturnButton;
	private Button QuitButton;

	[Signal]
	public delegate void ReturnButtonPressedEventHandler();

	[Signal]
	public delegate void QuitButtonPressedEventHandler();

	public override void _Ready()
	{
		base._Ready();

		Visible = false;
		IsActive = false;

		ReturnButton = GetNode<Button>("Control/MarginContainer/PanelContainer/MarginContainer/VBoxContainer/ReturnButton");
		QuitButton = GetNode<Button>("Control/MarginContainer/PanelContainer/MarginContainer/VBoxContainer/QuitButton");

		ReturnButton.ButtonDown += () => OnReturnButtonPressed();
		QuitButton.ButtonDown += () => OnQuitButtonPressed();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event.IsActionPressed("Escape") && IsInsideTree())
		{
			SetActive(! IsActive);
		}
	}

	public void SetActive(bool active)
	{
		IsActive = active;
		Visible = active;
	}

	private void OnReturnButtonPressed()
	{
		GD.Print("Return Button Pressed!");
		SetActive(false);

		EmitSignal(SignalName.ReturnButtonPressed);
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

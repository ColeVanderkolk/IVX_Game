using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{

	private bool IsActive;

	private Button ReturnButton;
	private Button QuitButton;

	public override void _Ready()
	{
		base._Ready();

		Visible = false;
		IsActive = false;

		ReturnButton = GetNode<Button>("Control/MarginContainer/PanelContainer/MarginContainer/VBoxContainer/ReturnButton");
		QuitButton = GetNode<Button>("Control/MarginContainer/PanelContainer/MarginContainer/VBoxContainer/QuitButton");

		ReturnButton.ButtonDown += () => ReturnButtonPressed();
		QuitButton.ButtonDown += () => QuitButtonPressed();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event.IsActionPressed("Escape") && IsInsideTree())
		{
			SetActive(! IsActive);
		}
	}

	private void SetActive(bool active)
	{
		IsActive = active;
		Visible = active;
	}

	private void ReturnButtonPressed()
	{
		GD.Print("Return Button Pressed!");
		SetActive(false);
	}

	private void QuitButtonPressed()
	{
		
	}

}

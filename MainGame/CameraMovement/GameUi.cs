using Godot;
using System;
using System.Collections.Generic;


public partial class GameUi : Control
{
	private String[] hordeSelectionAudios = new String[] {
		"res://Assets/Audio/SFX/sfx_selecthorde_001.mp3",
		"res://Assets/Audio/SFX/sfx_selecthorde_002.mp3",
		"res://Assets/Audio/SFX/sfx_selecthorde_003.mp3",
	};

	private String[] kingSelectionAudios = new String[] {
		"res://Assets/Audio/SFX/sfx_selectking_001.mp3",
		"res://Assets/Audio/SFX/sfx_selectking_002.mp3",
		"res://Assets/Audio/SFX/sfx_selectking_003.mp3",
	};

	private String uiAudio = "res://Assets/Audio/SFX/sfx_uibuttonsound.mp3";
	public override void _Ready()
	{
		var hordeManager = GetNode<HordeManager>("../../HordeManager");
		hordeManager.Connect("HoardChange", new Callable(this, "_on_HordeChange"));

		var currencyManeger = GetNode<currencymanager_script>("../../CurrencyManager_Node");
		currencyManeger.Connect("AddCurrency", new Callable(this, "_updateCurrency"));
		currencyManeger.Connect("RemoveCurrency", new Callable(this, "_updateCurrency"));
		_updateCurrency();
	}
	private void _on_panel_container_mouse_entered()
	{
		// GD.Print("Mouse entered!");
		GetParent<CamMove>().ToggleEdgeScrolling(false);
	}
	private void _on_panel_container_mouse_exited()
	{
		// GD.Print("Mouse exit!");   
		// GD.Print(GetViewport().GetMousePosition().Y);
		var screenHeight = GetViewport().GetVisibleRect().Size.Y;
		// GD.Print(screenHeight * 0.8f);
		if (GetViewport().GetMousePosition().Y < screenHeight * 0.8f) {
			GetParent<CamMove>().ToggleEdgeScrolling(true);
		}
	}
	private void _on_PauseButton_pressed()
	{
		var pauseMenu = GetNode<PauseMenu>("../../PauseMenu");
		pauseMenu.SetActive(true); // call the strongly-typed method
		var audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		audioPlayer.Stream = GD.Load<AudioStream>(uiAudio);
		audioPlayer.Play();

		var hordeManager = GetNode<HordeManager>("../../HordeManager");
		switch (hordeManager.getSelectedHordeNumber())
		{
			case 0:
				GetNode<Button>("HordeButtons/HBoxContainer/MarginContainer/0Button").GrabFocus();
				break;
			case 1:
				GetNode<Button>("HordeButtons/HBoxContainer/MarginContainer/1Button").GrabFocus();
				break;
			case 2:
				GetNode<Button>("HordeButtons/HBoxContainer/MarginContainer/2Button").GrabFocus();
				break;
			case 3:
				GetNode<Button>("HordeButtons/HBoxContainer/MarginContainer/3Button").GrabFocus();
				break;
			case 4:
				GetNode<Button>("HordeButtons/HBoxContainer/MarginContainer/4Button").GrabFocus();
				break;
			default:
				GetNode<Button>("HordeButtons/HBoxContainer/MarginContainer/0Button").GrabFocus();
				break;
		}
	}
	private void _updateCurrency()
	{

		var coinLabel = GetNode<Label>("CoinLabel");
		var metalLabel = GetNode<Label>("MetalLabel");
		var bodyLabel = GetNode<Label>("BodyLabel");
		
		var currancyManeger = GetNode<currencymanager_script>("../../CurrencyManager_Node");
		coinLabel.Text = currancyManeger.get_currency_balance(0).ToString();
		metalLabel.Text = currancyManeger.get_currency_balance(1).ToString();
		bodyLabel.Text = currancyManeger.get_currency_balance(2).ToString();
	}
	private void _on_king_pressed()
	{
		var hordeManager = GetNode<HordeManager>("../../HordeManager");
		bool success = hordeManager.setSelectedHorde(0);
		
		if (!success)
		{
			GD.PrintErr("Failed to select king horde!");
			return;
		}

		var audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		audioPlayer.Stream = GD.Load<AudioStream>(kingSelectionAudios[GD.Randi() % kingSelectionAudios.Length]);
		audioPlayer.Play();
	}
	private void _on_horde1_pressed()
	{
		var hordeManager = GetNode<HordeManager>("../../HordeManager");
		bool success = hordeManager.setSelectedHorde(1);

		if (!success)
		{
			GD.PrintErr("Failed to select horde 1!");
			return;
		}

		var audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		audioPlayer.Stream = GD.Load<AudioStream>(hordeSelectionAudios[GD.Randi() % hordeSelectionAudios.Length]);
		audioPlayer.Play();
	}
	private void _on_horde2_pressed()
	{
		var hordeManager = GetNode<HordeManager>("../../HordeManager");
		bool success = hordeManager.setSelectedHorde(2);

		if (!success)
		{
			GD.PrintErr("Failed to select horde 2!");
			return;
		}

		var audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		audioPlayer.Stream = GD.Load<AudioStream>(hordeSelectionAudios[GD.Randi() % hordeSelectionAudios.Length]);
		audioPlayer.Play();
	}
	private void _on_horde3_pressed()
	{
		var hordeManager = GetNode<HordeManager>("../../HordeManager");
		bool success = hordeManager.setSelectedHorde(3);

		if (!success)
		{
			GD.PrintErr("Failed to select horde 3!");
			return;
		}

		var audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		audioPlayer.Stream = GD.Load<AudioStream>(hordeSelectionAudios[GD.Randi() % hordeSelectionAudios.Length]);
		audioPlayer.Play();
	}
	private void _on_horde4_pressed()
	{
		var hordeManager = GetNode<HordeManager>("../../HordeManager");
		bool success = hordeManager.setSelectedHorde(4);

		if (!success)
		{
			GD.PrintErr("Failed to select horde 4!");
			return;
		}

		var audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		audioPlayer.Stream = GD.Load<AudioStream>(hordeSelectionAudios[GD.Randi() % hordeSelectionAudios.Length]);
		audioPlayer.Play();
	}
	private void _on_HordeChange()
	{
		List<Node3D> newHordes = GetNode<HordeManager>("../../HordeManager").Hordes;
		var button2 = GetNode<Button>("HordeButtons/HBoxContainer/MarginContainer2/1Button");
		var button3 = GetNode<Button>("HordeButtons/HBoxContainer/MarginContainer3/2Button");
		var button4 = GetNode<Button>("HordeButtons/HBoxContainer/MarginContainer4/3Button");
		var button5 = GetNode<Button>("HordeButtons/HBoxContainer/MarginContainer5/4Button");

		button2.Visible = false;
		button3.Visible = false;
		button4.Visible = false;
		button5.Visible = false;

		for (int i = 1; i < newHordes.Count; i++)
		{
			Button button;
			switch (i)
			{
				case 1:
					button = button2;
					break;
				case 2:
					button = button3;
					break;
				case 3:
					button = button4;
					break;
				case 4:
					button = button5;
					break;
				default:
					continue; // Skip if index is out of bounds
			}
			button.Visible = true;
			button.Text = newHordes[i].Call("getSize").As<int>().ToString() + "/10";
			GD.Print("Updated button " + i + " with horde size " + newHordes[i].Call("getSize").As<int>());
		}
		
	}
}

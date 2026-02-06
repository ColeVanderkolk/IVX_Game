using Godot;
using System;
using System.Collections.Generic;


public partial class GameUi : Control
{
	public override void _Ready()
	{
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
		hordeManager.setSelectedHorde(0);
	}

	private void _on_HordeChange()
	{
		List<Node3D> newHordes = GetNode<HordeManager>("../../HordeManager").Hordes;
		var button2 = GetNode<Button>("HordeButtons/HBoxContainer/MarginContainer/1Button");
		var button3 = GetNode<Button>("HordeButtons/HBoxContainer/MarginContainer/2Button");
		var button4 = GetNode<Button>("HordeButtons/HBoxContainer/MarginContainer/3Button");
		var button5 = GetNode<Button>("HordeButtons/HBoxContainer/MarginContainer/4Button");

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
	
		}
		
	}
}

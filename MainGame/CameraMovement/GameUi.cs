using Godot;
using System;

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
}
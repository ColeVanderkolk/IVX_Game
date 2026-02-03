using Godot;
using System;

public partial class mine_buildingscript : Node3D
{
	// To periodically generate currency
	private Timer mineTimer;

	// Coins (?) generated per second
	[Export]
	private int COIN_RATE = 5;
	
	// Whether this mine can function
	private bool Assimilated = false;

	// The models that are swapped upon assimilation
	private Node3D OldMesh;
	private Node3D NewMesh;




	public override void _Ready()
	{
		mineTimer = GetNode<Timer>("../Timer");
		OldMesh = GetNode<Node3D>("DefunctMesh");
		NewMesh = GetNode<Node3D>("AssimMesh");
	} 

	// Add whatever currency this mine generates to manager
	// signalled every second while assimillated
	private void OnMineTimeout()
	{
		GD.Print("+5");
		// currencymanager_script.add_currency((int)CURRENCIES.COIN, COIN_RATE);
	}

	// Should be signalled once the mine has been assimillated
	// toggles assimillated model and subscribes to mineTimer to produce
	private void OnAssimilate(Vector3 mousePos)
	{
		if(!Assimilated)
		{
			Assimilated = true;
			NewMesh.Visible = true;
			OldMesh.Visible = false;
			mineTimer.Timeout += OnMineTimeout;
		}

	}

}

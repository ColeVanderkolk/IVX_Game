using Godot;
using System;

public partial class mine_buildingscript : Node3D
{
	// To periodically generate currency
	private Timer mineTimer;

	// Metal (?) generated per second
	[Export]
	private int METAL_RATE = 5;

	// For sfx when assimilation happens?
    [Signal]
    public delegate void BuildingAssimilatedEventHandler();
	
	// Whether this mine can function
	private bool Assimilated = false;

	// where units go to interact (die)
    private Area3D InteractArea;


	// The models that are swapped upon assimilation
	private Node3D OldMesh;
	private Node3D NewMesh;




	public override void _Ready()
	{
		mineTimer = GetNode<Timer>("../Timer");
		OldMesh = GetNode<Node3D>("DefunctMesh");
		NewMesh = GetNode<Node3D>("AssimMesh");
		InteractArea = GetNode<Area3D>("Area3D");
	} 

	// Add whatever currency this mine generates to manager
	// signalled every second while assimillated
	private void OnMineTimeout()
	{
		GD.Print("+" + METAL_RATE + " for mining");
		// currencymanager_script.add_currency((int)CURRENCIES.METAL, COIN_RATE);
	}

	// Should be signalled once the mine has been assimillated
	// toggles assimillated model and subscribes to mineTimer to produce
	private void OnAssimilate(int a, int b)
	{
		if(!Assimilated)
		{
			Assimilated = true;
			NewMesh.Visible = true;
			OldMesh.Visible = false;
			EmitSignal(SignalName.BuildingAssimilated);
			mineTimer.Timeout += OnMineTimeout;
		}

	}

	// If a horde enters building, prepare to assimilate
    private void OnHordeEnter(Area3D horde)
    {
        if (!Assimilated)
        {
            horde.GetParent().Connect("sacrificed", new Callable(this, "OnAssimilate"));
        }
    }

    // In case operation is cancelled or units were just passing through somehow
    private void OnHordeExit(Area3D horde)
    {
        if (horde.GetParent().IsConnected("sacrificed", new Callable(this, "OnAssimilate")))
            horde.GetParent().Disconnect("sacrificed", new Callable(this, "OnAssimilate"));
    }

}

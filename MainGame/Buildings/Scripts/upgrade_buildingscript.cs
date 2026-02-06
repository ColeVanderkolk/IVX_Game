using Godot;
using System;
using System.ComponentModel;

public partial class upgrade_buildingscript : Node3D
{
    // The models that are swapped upon assimilation
	private Node3D OldMesh;
	private Node3D NewMesh;

    // where units go to interact (die)
    private Area3D InteractArea;

    // Whether this station can upgrade units
	private bool Assimilated = false;

    // Storing the costs of upgrading units here for now
    [Export]
    private int[] UPGRADE_COSTS;

    [Export]
    private CURRENCIES CostType;

    // Emits when an upgrade is attempted, and whether it was successful
    [Signal]
    public delegate void UnitUpgradedEventHandler(int toTier);

    // For sfx when assimilation happens?
    [Signal]
    public delegate void BuildingAssimilatedEventHandler();

    public override void _Ready()
    {
        OldMesh = GetNode<Node3D>("DefunctMesh");
        NewMesh = GetNode<Node3D>("AssimMesh");
        InteractArea = GetNode<Area3D>("Area3D");
    }

    // Toggles model and whether buliding can upgrade units
    private void OnAssimilate(int whatever, int thisshouldbeadifferentsignal)
    {
        if(!Assimilated)
		{
			Assimilated = true;
			NewMesh.Visible = true;
			OldMesh.Visible = false;
            EmitSignal(SignalName.BuildingAssimilated);
		}
    }

    // Attempts to produce units 1 tier higher than the sacrificed horde, deducting currency as needed
    // All units presumed to be of same tier
    // if you run out of cash, reproduces units of the same tier instead
    private void OnUpgrade(int tier, int count)
    {
        if(Assimilated)
        {
            int i = 0;
            while ( i < count ) //&& currencyManager.removeCurrency(CostType, upgradeCosts[curLevel])) 
            {
                GD.Print("-" + UPGRADE_COSTS[tier] + " for upgrading");
                EmitSignal(SignalName.UnitUpgraded, tier + 1);
                i++;
            }
            while (i < count)
            {
                GD.Print("Reproduced a unit because ur too broke to upgrade it");
                EmitSignal(SignalName.UnitUpgraded, tier);
                i++;
            }
        }
    }

    // If a horde enters building, prepare to either assimilate or upgrade
    private void OnHordeEnter(Area3D horde)
    {
        if (Assimilated)
        {
            horde.GetParent().Connect("sacrificed", new Callable(this, "OnUpgrade"));
        }
        else
        {
            horde.GetParent().Connect("sacrificed", new Callable(this, "OnAssimilate"));
        }
    }

    // In case operation is cancelled or units were just passing through somehow
    private void OnHordeExit(Area3D horde)
    {
        if (horde.GetParent().IsConnected("sacrificed", new Callable(this, "OnUpgrade")))
            horde.GetParent().Disconnect("sacrificed", new Callable(this, "OnUpgrade"));
        if (horde.GetParent().IsConnected("sacrificed", new Callable(this, "OnAssimilate")))
            horde.GetParent().Disconnect("sacrificed", new Callable(this, "OnAssimilate"));
            

    }
}

using Godot;
using System;
using System.ComponentModel;

public partial class upgrade_buildingscript : Node3D
{
    // The models that are swapped upon assimilation
	private Node3D OldMesh;
	private Node3D NewMesh;

    // Whether this station can upgrade units
	private bool Assimilated = false;

    // Storing the costs of upgrading units here for now
    [Export]
    private int[] UPGRADE_COSTS;

    [Export]
    private CURRENCIES CostType;

    // Emits when an upgrade is attempted, and whether it was successful
    [Signal]
    public delegate void UnitUpgradedEventHandler(bool success);

    // For sfx when assimilation happens?
    [Signal]
    public delegate void BuildingAssimilatedEventHandler();

    public override void _Ready()
    {
        OldMesh = GetNode<Node3D>("DefunctMesh");
        NewMesh = GetNode<Node3D>("AssimMesh");
    }

    // Toggles model and whether buliding can upgrade units
    private void OnAssimilate(Vector3 mousePos)
    {
        if(!Assimilated)
		{
			Assimilated = true;
			NewMesh.Visible = true;
			OldMesh.Visible = false;
            EmitSignal(SignalName.BuildingAssimilated);
		}
    }

    // Attempts to upgrade whatever unit requested this, deducting currency as needed
    // mousePos is here in case it's helpful for a future UI middleman
    // currently just uses same input as assimilate
    private void OnUpgrade(Vector3 mousePos, int curLevel)
    {
        if(Assimilated)
        {
            // if(currencyManager.removeCurrency(CostType, upgradeCosts[curLevel]))
            GD.Print("-" + UPGRADE_COSTS[curLevel] + " for upgrading");
            EmitSignal(SignalName.UnitUpgraded, true);
            // else EmitSignal(SignalName.UnitUpgraded, false);
        }
    }
}

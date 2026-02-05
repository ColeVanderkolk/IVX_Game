using Godot;
using System;

public partial class gate_buildingscript : Node3D
{
	// The models that are swapped upon assimilation
	private Node3D OldMesh;
	private Node3D NewMesh;

	// Used to toggle collisions when destroyed
	private StaticBody3D Hurtbox;

	// Whether this gate can repel foes
	private bool Assimilated = false;

	// For sfx when assimilation happens?
	[Signal]
	public delegate void BuildingAssimilatedEventHandler();

	// Emits when damage is taken
	[Signal]
	public delegate void GateHurtEventHandler();

	//Emits when gate is destroyed
	[Signal]
	public delegate void GateBrokenEventHandler();

	// Emits after a repair is attempted, unsuccessful if poor
	[Signal]
	public delegate void GateRepairedEventHandler(bool success);

	[Export]
	private CURRENCIES GATE_REPAIR_COST_TYPE;

	[Export]
	private int GATE_REPAIR_COST_HP_RATIO;

	[Export]
	public int GATE_MAX_HP;

	public int GateHP;


	public override void _Ready()
	{
		OldMesh = GetNode<Node3D>("DefunctMesh");
		NewMesh = GetNode<Node3D>("AssimMesh");
		Hurtbox = GetNode<StaticBody3D>("StaticBody3D");
		GateHP = 0;
		Assimilate();
	}

	// Toggles model and whether gate can block foes, only needs to be called internally for gates
	// must only function if gate has enough HP, and is not assimilated currently
	// For the pruposes of gates, repairing a destroyed gate is equivalent to assimilation
	private void Assimilate()
	{
		if(!Assimilated && GateHP > 0)
		{
			Assimilated = true;
			NewMesh.Visible = true;
			OldMesh.Visible = false;
			EmitSignal(SignalName.BuildingAssimilated);
			Hurtbox.SetCollisionLayerValue(4, true);
		}
	}

	// Signal to apply damage, damage can be lethal
	public void OnHurt(int damage)
	{
		if (damage >= GateHP)
		{
			BreakGate();
		} 
		else
		{
			GateHP -= damage;
			EmitSignal(SignalName.GateHurt);
			GD.Print("Gate HP: " + GateHP);
		}
	}

	// De-assimilates gate and disables its collider
	private void BreakGate()
	{
		GateHP = 0;
		Assimilated = false;
		OldMesh.Visible = true;
		NewMesh.Visible = false;
		EmitSignal(SignalName.GateBroken);
		Hurtbox.SetCollisionLayerValue(4, false);
		GD.Print("Gate Broken");
	}


	// Signal to repair gate for as much missing hp as you can afford
	// May recurse if you lose money while it is calculating because currency has no exclusion lock
	// Signals repair, and Assimilates in case gate was broken prior
	private void OnPerformRepair(Vector3 mousePos)
	{
		int missingHP = GATE_MAX_HP - GateHP;
		int currBalance = 151; // currencyManager.get_currency_balance(GATE_REPAIR_COST_TYPE);
		if(currBalance >= GATE_REPAIR_COST_HP_RATIO)
		{
			int repairCost = missingHP * GATE_REPAIR_COST_HP_RATIO;
			int actualCost = currBalance >= repairCost ? repairCost : currBalance;
			actualCost -= actualCost % GATE_REPAIR_COST_HP_RATIO;
			// if currencyManager.remove_currency(GATE_REPAIR_COST_TYPE, repairCost)
			GateHP += actualCost / GATE_REPAIR_COST_HP_RATIO;
			EmitSignal(SignalName.GateRepaired, true);
			Assimilate();
			GD.Print("-" + actualCost + " for repairing " + actualCost / GATE_REPAIR_COST_HP_RATIO + "HP");
			// else lost money during operation, try again? OnPerformRepair()
			// ig currency could have a removeToThreshold method
		}
		EmitSignal(SignalName.GateRepaired, false);
	}

}

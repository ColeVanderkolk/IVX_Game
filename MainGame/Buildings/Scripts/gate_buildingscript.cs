using Godot;
using System;

public partial class gate_buildingscript : Node3D
{
	// The models that are swapped upon assimilation
	private Node3D OldMesh;
	private Node3D NewMesh;

	// where units go to interact (repair/break)
	private Area3D InteractArea;

	// for enemy attack iframes
	private Timer Iframes;

	// Maybe used for pathfinding collisions idk
	private StaticBody3D StaticBody;

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

	[Export]
	private int GATE_STARTING_HP;

	public int GateHP;

	public bool broken = true;

	public override void _Ready()
	{
		OldMesh = GetNode<Node3D>("DefunctMesh");
		NewMesh = GetNode<Node3D>("AssimMesh");
		StaticBody = GetNode<StaticBody3D>("StaticBody3D");
		InteractArea = GetNode<Area3D>("Area3D");
		Iframes = GetNode<Timer>("ImmunityFrames");
		GateHP = GATE_STARTING_HP;
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
			InteractArea.SetCollisionLayerValue(4, true);
			broken = false;
		}
	}

	// Signal to apply damage, damage can be lethal
	public void OnHurt(int damage)
	{
		if(Iframes.TimeLeft == 0)
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
				Iframes.Start();
			}
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
		InteractArea.SetCollisionLayerValue(4, false);
		GD.Print("Gate Broken");
		broken = true;
	}


	// Signal to repair gate for as much missing hp as you can afford
	// May recurse if you lose money while it is calculating because currency has no exclusion lock
	// Signals repair, and Assimilates in case gate was broken prior
	private void OnPerformRepair(int placeholdera, int placeholderb)
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

	// If a horde enters building, prepare to assimilate/repair
	// Only friendly hordes should sac so no need to check
	private void OnHordeEnter(Area3D horde)
	{
		horde.GetParent().Connect("sacrificed", new Callable(this, "OnPerformRepair"));
		horde.GetParent().Connect("gateDamaged", new Callable(this, "OnHurt"));
	}

	// In case operation is cancelled or units were just passing through somehow
	private void OnHordeExit(Area3D horde)
	{
		if (horde.GetParent().IsConnected("sacrificed", new Callable(this, "OnPerformRepair")))
			horde.GetParent().Disconnect("sacrificed", new Callable(this, "OnPerformRepair"));
	}

}

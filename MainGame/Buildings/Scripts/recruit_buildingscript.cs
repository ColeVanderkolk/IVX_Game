using Godot;
using System;

public partial class recruit_buildingscript : Node3D
{
    // The models that are swapped upon assimilation
	private Node3D OldMesh;
	private Node3D NewMesh;

    // Whether this station can recruit units
	private bool Assimilated = false;

    // Emits when a unit is recruited
    [Signal]
    public delegate void UnitRecruitedEventHandler();

    // For sfx when assimilation happens?
    [Signal]
    public delegate void BuildingAssimilatedEventHandler();

    [Export]
    private int RECRUIT_COST;

    private Timer RecruitTimer;

    public override void _Ready()
    {
        OldMesh = GetNode<Node3D>("DefunctMesh");
        NewMesh = GetNode<Node3D>("AssimMesh");
        RecruitTimer = GetNode<Timer>("../Timer");
    }

    // Toggles model and whether buliding can produce units
    private void OnAssimilate(Vector3 mousePos)
    {
        if(!Assimilated)
		{
			Assimilated = true;
			NewMesh.Visible = true;
			OldMesh.Visible = false;
            EmitSignal(SignalName.BuildingAssimilated);
            RecruitTimer.Timeout += OnRecruitTimeout;
		}
    }

    // Spends coins every 30s or so to recruit a unit
    // signal should be handeled by horde controller?
    // could provide unit spawn coords here if needed
    private void OnRecruitTimeout()
    {
        // if (currencyManager.removeCurrency(CURRENCIES.COIN, RecruitCost))
        EmitSignal(SignalName.UnitRecruited);
        GD.Print("-" + RECRUIT_COST + " for recruiting");
        
    }
}

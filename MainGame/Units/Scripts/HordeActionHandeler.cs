using Godot;
using System;
/**
 * HordeActionHandeler tells Hordes what to do
 * when an action is selected
 */
public partial class HordeActionHandeler : Node
{
	public static HordeActionHandeler Instance;
	
	/**
	 * Attack moves a Horde to attack another Horde
	 * Horde = the horde instructed to go attack as a Node
	 * TargetHorde = the target horde to attack
	 * returns void
	 */
	public void Attack(Node3D Horde, Node3D TargetHorde)
	{
		Horde.Call("targetEnemy", TargetHorde);
	}

	/**
	 * MoveTo moves a Horde to a specified location
	 * Horde = the horde instructed to go attack as a Node
	 * Dest = the destination
	 * returns void
	 */
	public void MoveTo(Node3D Horde, Vector3 Dest)
	{
		Horde.Call("startMoving", Dest);
	}

	/**
	 * SpendUnits checks if the Horde has the required amount of units to
	 * do the action and then sends them to the destination to be spent
	 * Horde = the horde instructed to go attack as a Node
	 * Required = the number of units required for the job
	 * Dest = the destination of the job
	 * returns true on if the job can be completed, false if not enough units
	 */
	public bool SpendUnits(Node3D Horde, int Required, Vector3 Dest)
	{
		var _new_horde = Horde.Call("mitosis", Required).As<Node3D>();
		if (_new_horde == null)
		{
			return false;
		}
		_new_horde.Call("spendHorde", Dest);
		return true;
	}
}

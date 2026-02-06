using Godot;
using System;
using System.Collections.Generic;

/**
 * HordeManeger manages all player's friendly hordes
 * Selects the friendly horde from UI
 * Prevent more than 4 hordes from existing
 * Deals with Horde Deaths
 */
public partial class HordeManager : Node
{
	/* List of friendly hordes*/
	public List<Node3D> Hordes = new List<Node3D>();
	/* Maximum hordes (including king guy) */
	private int max = 5;
	/* Current horde selected */
	private int SelectedHorde = 0;

	/** 
	 * returns the horde currently selected
	 */
	public Node3D getSelectedHorde()
	{
		return Hordes[SelectedHorde];
	}

	/** 
	 * returns the horde selection number
	 */
	public int getSelectedHordeNumber()
	{
		return SelectedHorde;
	}

	/** 
	 * returns true if selection changed, false if DNE
	 */
	public bool setSelectedHorde(int selection)
	{
		if (selection < Hordes.Count)
		{
			SelectedHorde = selection;
			return true;
		}
		else
		{
			return false;
		}
	}

	/**
	 * handles when a horde is empty
	 */
	private void handleDeadHorde(Node3D horde)
	{
		Hordes.Remove(horde);
		SelectedHorde = 0;
	}


	/**
	 * _on_unit_added recives the unitAdded signal from a friendly horde
	 * checks if there are too many units and then splits into a new horde
	 * or kills the extra unit
	 */
	private void _on_unit_added(Node3D horde, int tier)
	{
		if (horde.Call("getSize").As<int>() > 10)
		{
			// Look for open horde
			for (int i = 1; i < Hordes.Count; i++) {
				Node3D other_horde = Hordes[i];
				if (other_horde != horde) {
					if (other_horde.Call("getSize").As<int>() < 10)
					{
						other_horde.Call("addUnit", tier);
						horde.Call("removeUnit", tier);
						return;
					}
				}
			}

			// Create a new one if needed
			if (Hordes.Count < max)
			{
				var _new_horde = horde.Call("mitosis", 1).As<Node3D>();
				Hordes.Add(_new_horde);
				return;
			}
		}
	}

}

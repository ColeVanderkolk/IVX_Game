using Godot;
using System;
/**
 * HordeManeger manages all player's friendly hordes
 * Selects the friendly horde from UI
 * Prevent more than 4 hordes from existing
 * Deals with Horde Deaths
 */
public partial class HordeManager : Node
{
	public Node hordes[];

}

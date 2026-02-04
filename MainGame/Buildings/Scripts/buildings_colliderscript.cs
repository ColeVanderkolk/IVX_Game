using Godot;
using System;

public partial class buildings_colliderscript : StaticBody3D
{

	// Building has been clicked
	// includes mousePos in case this is helpful for UI idk
	[Signal]
	public delegate void BuildingClickedEventHandler(Vector3 mousePos);


	// Emits a signal to indicate this building has been mouse clicked for future UI purposes
	private void OnMouseEvent(Node camera, InputEvent input, Vector3 pos, Vector3 normal, int shape_idx)
	{
		if (input is InputEventMouseButton mouseButton)
		{
			if(mouseButton.Pressed == true && mouseButton.ButtonIndex == MouseButton.Left)
			{
				GD.Print("Clicked");
				EmitSignal(SignalName.BuildingClicked, pos);
			}
		}
	}
}

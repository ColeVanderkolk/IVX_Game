using Godot;
using System;

public partial class GameUi : Control
{
	public void _on_panel_container_mouse_entered()
	{
		GD.Print("Mouse entered!");
		GetParent<CamMove>().ToggleEdgeScrolling(false);
	}
	public void _on_panel_container_mouse_exited()
	{
		GD.Print("Mouse exit!");   
		GD.Print(GetViewport().GetMousePosition().Y);
		if (GetViewport().GetMousePosition().Y < 512) {
			GetParent<CamMove>().ToggleEdgeScrolling(true);
		}
	}
}

using Godot;
using System;

public partial class Lordofthebiomind : Node3D
{
	public override void _Ready()
	{
		var anim = GetNode<AnimationPlayer>("AnimationPlayer");
		anim.Play("Armature_001");
		anim.Stop();
		anim.Seek(0);
	}
	
	public override void _Process(double delta)
	{
		var anim = GetNode<AnimationPlayer>("AnimationPlayer");
		if (anim.CurrentAnimation == "Armature_001" &&
			anim.CurrentAnimationPosition >= 1.02f)
		{
			GD.Print("Animation finished, switching to idle");
			anim.Stop();
			anim.Seek(0);
			anim.Play("Armature_001");
		}
	}
	
}

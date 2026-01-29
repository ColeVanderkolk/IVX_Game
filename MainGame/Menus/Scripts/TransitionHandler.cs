using Godot;
using System;

public partial class TransitionHandler : CanvasLayer
{
	private AnimationPlayer Player;

	[Signal]
	public delegate void TransitionedInEventHandler();

	[Signal]
	public delegate void TransitionedOutEventHandler();

	public override void _Ready()
	{
		base._Ready();

		Player = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public void TransitionIn()
	{
		Player.Play("in");
	}

	public void TransitionOut()
	{
		Player.Play("out");
	}
}

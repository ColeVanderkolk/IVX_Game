using Godot;
using System;
using System.Threading.Tasks;

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

	public async Task TransitionIn()
	{
		Player.Play("in");

        await ToSignal(Player, Godot.AnimationPlayer.SignalName.AnimationFinished);
        EmitSignal(SignalName.TransitionedIn);
	}

	public async Task TransitionOut()
	{
		Player.Play("out");

        await ToSignal(Player, Godot.AnimationPlayer.SignalName.AnimationFinished);
        EmitSignal(SignalName.TransitionedOut);
	}
}

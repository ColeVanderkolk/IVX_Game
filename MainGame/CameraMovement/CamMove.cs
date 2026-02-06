using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;

public partial class CamMove : Camera3D
{
	// Movement settings

	[Export] public float MaxMoveSpeed = 25.0f;
	[Export] public float MoveAcceleration = 24f;
	[Export] public float EdgeScrollMargin = 20.0f; // pixels from edge to trigger scroll
	[Export] Curve edgeScrollSpeedCurve;

	// Zoom settings
	[Export] public float ZoomAccel = 2.0f;
	[Export] public float MazZoomSpeed = 5.0f;
	[Export] public float GroundLevel = 0f;
	[Export] public float MinHeight = 5.0f;
	[Export] public float MaxHeight = 50.0f;
	[Export] public float PlaneHeight = 10.0f; // Y-axis height the camera moves along

	[Export] public uint GroundLayerMask = 1; // Collision layer mask for ground (layer 1 by default)
	[Export, Range(0, 1)] public float rotationSensitivity = 0.001f;
	private float _zoomVelocity;
	private Vector3 _velocity = Vector3.Zero;
	private float _currentZoom;
	private bool _edgeScrollingEnabled = true;

	public override void _Ready()
	{
		_currentZoom = Position.Y;

		// Ensure the camera starts at the correct height
		Position = new Vector3(Position.X, PlaneHeight, Position.Z);

	}

	public override void _Process(double delta)
	{


		HandleKeyboardMovement(delta);
		if (_edgeScrollingEnabled)
			HandleEdgeScrolling(delta);
		HandleRotation(delta);
		HandleZoom(delta);

		// Apply movement
		if (_velocity.LengthSquared() > 0)
		{

			if (Input.IsKeyPressed(Key.Shift))
			{
				_velocity += _velocity.Normalized() * (float)(MoveAcceleration * delta); // readd accel for 2x speed
			}

			Vector3 newPosition = Position + _velocity * (float)delta;
			Position = newPosition;
		}

		_velocity = _velocity.LimitLength(MaxMoveSpeed);
		_velocity *= 0.8f;
	}


	private void HandleKeyboardMovement(double delta)
	{

		// Get input direction
		Vector3 inputDir = Vector3.Zero;

		if (Input.IsActionPressed("Move_Forward"))
		{
			inputDir -= Transform.Basis.Z;
		}
		if (Input.IsActionPressed("Move_Backward"))
		{
			inputDir += Transform.Basis.Z;
		}
		if (Input.IsActionPressed("Move_Left"))
		{
			inputDir -= Transform.Basis.X;
		}
		if (Input.IsActionPressed("Move_Right"))
		{
			inputDir += Transform.Basis.X;
		}

		// Normalize to prevent faster diagonal movement
		if (inputDir.LengthSquared() > 0)
		{
			_velocity += Plane.PlaneXZ.Project(inputDir).Normalized() * (float)(MoveAcceleration * delta);
		}
	}

	private void HandleEdgeScrolling(double delta)
	{
		Vector2 mousePos = GetViewport().GetMousePosition();
		Vector2 viewportSize = GetViewport().GetVisibleRect().Size;

		float lDiff = EdgeScrollMargin - mousePos.X;
		float tDiff = EdgeScrollMargin - mousePos.Y;
		float bDiff = mousePos.Y - ((viewportSize.Y * 0.78f) - EdgeScrollMargin);
		float rDiff = mousePos.X - (viewportSize.X - EdgeScrollMargin);

		// magnitude values represent number of pixels from inner-edge of the margin with direction
		float hMag = Math.Clamp(rDiff, 0, EdgeScrollMargin) - Math.Clamp(lDiff, 0, EdgeScrollMargin);
		float vMag = -Math.Clamp(tDiff, 0, EdgeScrollMargin) + Math.Clamp(bDiff, 0, EdgeScrollMargin);

		// normalize and project these values onto curve to get fancy % of max speed per direction
		hMag = edgeScrollSpeedCurve.Sample(hMag / EdgeScrollMargin);
		vMag = edgeScrollSpeedCurve.Sample(vMag / EdgeScrollMargin);


		// Check edges
		var direction_power = Plane.PlaneXZ.Project(Transform.Basis.X).Normalized() * hMag + Plane.PlaneXZ.Project(Transform.Basis.Z).Normalized() * vMag;
		var edgeVelocity = (float)(delta * MoveAcceleration) * direction_power;

		// Add edge scrolling to velocity
		_velocity += edgeVelocity;
	}


	private void HandleZoom(double delta)
	{
		if (_zoomVelocity != 0)
		{
			var displacement = Transform.Basis.Z * _zoomVelocity * (float)delta;
			PlaneHeight = displacement.Y + Position.Y;

			Vector3 newPosition = Position + displacement;
			if (newPosition.Y < MaxHeight && newPosition.Y > MinHeight)
			{
				newPosition.Y = Mathf.Clamp(PlaneHeight, MinHeight, MaxHeight);
				Position = newPosition;
			}
			_zoomVelocity = Mathf.Lerp(_zoomVelocity, 0, 0.1f);
		}
	}

	private void HandleRotation(double delta)
	{
		if (rotAnchor != null)
		{
			var offset = GetViewport().GetMousePosition().X - rotAnchor.mouseStartingX;

			var vtr = rotAnchor.startingPosition - rotAnchor.worldPos;
			var angle = (float)(offset * rotationSensitivity * delta);
			var rv = vtr.Rotated(Vector3.Up, angle - rotAnchor.rotation);


			rotAnchor.rotation = angle + rotAnchor.rotation;
			GlobalPosition = rotAnchor.worldPos + rv;

			Transform = Transform.Rotated(Vector3.Up, -angle);
		}
	}

	public class RotationAnchor
	{

		public RotationAnchor(float sp, Vector3 starting_position, Vector3 anchor_pos)
		{
			this.mouseStartingX = sp;
			this.worldPos = anchor_pos;
			this.startingPosition = starting_position;
		}

		public float rotation;
		public Vector3 startingPosition;

		public float mouseStartingX;
		public Vector3 worldPos;
	}

	private RotationAnchor rotAnchor = null;

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.Pressed)
			{

				if (mouseButton.ButtonIndex == MouseButton.WheelUp)
				{
					_zoomVelocity -= ZoomAccel;
				}
				else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
				{
					_zoomVelocity += ZoomAccel;
				}

			}
			if (mouseButton.ButtonIndex == MouseButton.Right && mouseButton.IsPressed())
			{
				var from = ProjectRayOrigin(mouseButton.Position);
				var click = from + ProjectRayNormal(mouseButton.Position).Normalized();


				var vec = -GlobalTransform.Basis.Z;
				var forward = new Vector2(-vec.Z, vec.Y);
				var ground = Vector2.Right;

				var angle = Mathf.Abs((0.5f * Mathf.Pi) - Mathf.Acos(forward.Dot(ground)));// calculate angle between where you clicked projected outwards and the ground
				angle = (0.5f * Mathf.Pi) - Mathf.Abs((GlobalRotation.X));
				GD.Print("Angle: " + (angle) + "deg");

				var adj = Position.Y - GroundLevel;
				var opp = adj * Mathf.Tan(angle);

				var onGround = opp * Plane.PlaneXZ.Project(-Transform.Basis.Z);
				var pointOnGround = Position + onGround - new Vector3(0, Position.Y, 0);

				rotAnchor = new RotationAnchor(mouseButton.Position.X,
													GlobalPosition,
													pointOnGround
												);

				GD.Print("point: " + pointOnGround);
				debuganchor.Position = pointOnGround;

				Input.SetDefaultCursorShape(Input.CursorShape.Hsize);

			}
			else if (mouseButton.ButtonIndex == MouseButton.Right && mouseButton.IsReleased())
			{
				Input.SetDefaultCursorShape(Input.CursorShape.Arrow);
				Input.WarpMouse(new Vector2(rotAnchor.mouseStartingX, GetViewport().GetMousePosition().Y));
				rotAnchor = null;
			}

		}
	}

	[Export] Node3D debuganchor;

	public bool ToggleEdgeScrolling(bool force)
	{
		var prev = _edgeScrollingEnabled;
		_edgeScrollingEnabled = force;
		return prev;
	}
	public bool ToggleEdgeScrolling()
	{
		var prev = _edgeScrollingEnabled;
		_edgeScrollingEnabled = !_edgeScrollingEnabled;
		return prev;
	}
}

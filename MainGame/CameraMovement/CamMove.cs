using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;

public partial class CamMove : Camera3D
{
	// Movement settings
	[Export] public float MoveSpeed = 10.0f;
	[Export] public float EdgeScrollMargin = 20.0f; // pixels from edge to trigger scroll
													// [Export] public float EdgeScrollSpeed = 8.0f;
	[Export] Curve edgeScrollSpeedCurve;

	// Zoom settings
	[Export] public float ZoomSpeed = 2.0f;
	[Export] public float GroundLevel = 0f;
	[Export] public float MinHeight = 5.0f;
	[Export] public float MaxHeight = 50.0f;
	[Export] public float PlaneHeight = 10.0f; // Y-axis height the camera moves along

	[Export] public uint GroundLayerMask = 1; // Collision layer mask for ground (layer 1 by default)
	[Export, Range(0, 1)] public float rotationSensitivity = 0.001f;
	private float _zoomVelocity;
	private Vector3 _velocity = Vector3.Zero;
	private float _currentZoom;

	public override void _Ready()
	{
		_currentZoom = Position.Y;

		// Ensure the camera starts at the correct height
		Position = new Vector3(Position.X, PlaneHeight, Position.Z);

	}

	public override void _Process(double delta)
	{
		HandleKeyboardMovement(delta);
		HandleEdgeScrolling(delta);
		HandleRotation(delta);

		// Apply movement
		if (_velocity.LengthSquared() > 0)
		{

			if (Input.IsKeyPressed(Key.Shift))
			{
				_velocity *= 2;
			}

			Vector3 newPosition = Position + _velocity * (float)delta;
			Position = newPosition;

			// _zoomVelocity = (float)Mathf.Clamp((_zoomVelocity * delta), -1, 1);
		}
		Fov = (float)Mathf.Clamp(Fov, MinHeight, MaxHeight);
	}

	private void HandleKeyboardMovement(double delta)
	{
		_velocity = Vector3.Zero;

		// Get input direction
		Vector3 inputDir = Vector3.Zero;

		if (Input.IsKeyPressed(Key.W))
		{
			inputDir -= Transform.Basis.Z;
		}
		if (Input.IsKeyPressed(Key.S))
		{
			inputDir += Transform.Basis.Z;
		}
		if (Input.IsKeyPressed(Key.A))
		{
			inputDir -= Transform.Basis.X;
		}
		if (Input.IsKeyPressed(Key.D))
		{
			inputDir += Transform.Basis.X;
		}

		// Normalize to prevent faster diagonal movement
		if (inputDir.LengthSquared() > 0)
		{
			_velocity = Plane.PlaneXZ.Project(inputDir).Normalized() * MoveSpeed;
		}
	}

	private void HandleEdgeScrolling(double delta)
	{
		Vector2 mousePos = GetViewport().GetMousePosition();
		Vector2 viewportSize = GetViewport().GetVisibleRect().Size;

		Vector3 edgeVelocity = Vector3.Zero;

		float lDiff = EdgeScrollMargin - mousePos.X;
		float tDiff = EdgeScrollMargin - mousePos.Y;
		float bDiff = mousePos.Y - (viewportSize.Y - EdgeScrollMargin);
		float rDiff = mousePos.X - (viewportSize.X - EdgeScrollMargin);

		float hMag = Math.Clamp(rDiff, 0, EdgeScrollMargin) - Math.Clamp(lDiff, 0, EdgeScrollMargin);
		float vMag = -Math.Clamp(tDiff, 0, EdgeScrollMargin) + Math.Clamp(bDiff, 0, EdgeScrollMargin);

		// hMag = edgeScrollSpeedCurve.Sample(hMag/EdgeScrollMargin);
		// vMag = edgeScrollSpeedCurve.Sample(vMag/EdgeScrollMargin);

		// Check edges
		edgeVelocity += 1.5f * MoveSpeed * (Plane.PlaneXZ.Project(Transform.Basis.X * hMag) + Plane.PlaneXZ.Project(Transform.Basis.Z * vMag)) / EdgeScrollMargin;
		// Debug.Print("edge %:" + (Plane.PlaneXZ.Project(Transform.Basis.X * hMag) + Plane.PlaneXZ.Project(Transform.Basis.Z * vMag)) / EdgeScrollMargin);


		// Add edge scrolling to velocity
		_velocity += edgeVelocity;
	}


	public override void _PhysicsProcess(double delta)
	{
		if (rotAnchor != null && rotAnchor.fresh)
		{
			
		}
	}

	private void HandleRotation(double delta)
	{
		if (rotAnchor != null)
		{
			var offset = GetViewport().GetMousePosition().X - rotAnchor.screenPos;

			var vtr = rotAnchor.startingPosition - rotAnchor.worldPos;
			var angle = (float)(offset * rotationSensitivity * delta);
			var rv = vtr.Rotated(Vector3.Up, angle - rotAnchor.rotation);

			
			rotAnchor.rotation = angle + rotAnchor.rotation;
			GlobalPosition = rotAnchor.worldPos + rv;

			Transform = Transform.Rotated(Vector3.Up,-angle);
		}
	}

	public class RotationAnchor
	{

		public RotationAnchor(float sp, Vector3 to, Vector3 from, Vector3 starting_position)
		{
			this.screenPos = sp;
			this.to = to;
			this.from = from;
			this.fresh = true;
			this.startingPosition = starting_position;
		}

		public void place(Vector3 worldPos)
		{
			this.worldPos = worldPos;
			
		}

		public bool fresh;
		public float rotation;
		public Vector3 startingPosition;

		public float screenPos;
		public Vector3 worldPos;
		public Vector3 from;
		public Vector3 to;
	}

	private RotationAnchor rotAnchor = null;

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.Pressed)
			{
				float zoomChange = 0;

				if (mouseButton.ButtonIndex == MouseButton.WheelUp)
				{
					zoomChange = -ZoomSpeed;
				}
				else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
				{
					zoomChange = ZoomSpeed;
				}

				if (zoomChange != 0)
				{
					var displacement = Transform.Basis.Z * zoomChange;
					PlaneHeight = displacement.Y + Position.Y;

					// Smoothly move to new height
					Vector3 newPosition = Position;
					newPosition += displacement;
					newPosition.Y = Mathf.Clamp(PlaneHeight, MinHeight, MaxHeight);
					Position = newPosition;

				}

			}
				if (mouseButton.ButtonIndex == MouseButton.Right && mouseButton.IsPressed())
				{
					var from = ProjectRayOrigin(mouseButton.Position);
					var to = from + ProjectRayNormal(mouseButton.Position);
					rotAnchor = new RotationAnchor(mouseButton.Position.X,
													from,
													from + ProjectRayNormal(mouseButton.Position) * 1000,
													GlobalPosition
												);

					var vec = ProjectRayNormal(mouseButton.Position);
					var forward = new Vector2(-vec.Z,vec.Y);
					var ground = Vector2.Right;

					var angle = Mathf.Acos(forward.Dot(ground) / (forward.Length() * ground.Length()));
					var opp = GlobalPosition.Y;

					var distance = opp / Mathf.Cos(angle);
					var pointOnGround = Position + -Transform.Basis.Z * distance;

					rotAnchor.place(pointOnGround);

					GD.Print("point: " + pointOnGround);


				}
				else if (mouseButton.ButtonIndex == MouseButton.Right && mouseButton.IsReleased())
				{
					rotAnchor = null;
				}

		}
	}
}

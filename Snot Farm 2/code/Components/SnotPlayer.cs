using Sandbox.Citizen;
using System.Net.Http;

public sealed class SnotPlayer : Component
{
	[Property]
	[Category( "Components" )]
	public GameObject Camera { get; set; }

	[Property]
	[Category( "Components" )]
	public CharacterController Controller { get; set; }

	[Property]
	[Category( "Components" )]
	public CitizenAnimationHelper Animator { get; set; }

	/// <summary>
	/// How fast you can walk (Units per second)
	/// </summary>
	[Property]
	[Category( "Stats" )]
	[Range( 0f, 400f, 1f )]
	public float WalkSpeed { get; set; } = 120f;

	/// <summary>
	/// How fast you can run (Units per second)
	/// </summary>
	[Property]
	[Category( "Stats" )]
	[Range( 0f, 800f, 1f )]
	public float RunSpeed { get; set; } = 250f;

	/// <summary>
	/// How powerful you can jump (Units per second)
	/// </summary>
	[Property]
	[Category( "Stats" )]
	[Range( 0f, 1000f, 10f )]
	public float JumpStrength { get; set; } = 400f;

	/// <summary>
	/// How much damage your punch deals
	/// </summary>
	[Property]
	[Category( "Stats" )]
	[Range( 0f, 5f, 0.1f )]
	public float PunchStrength { get; set; } = 1f;

	/// <summary>
	/// How many seconds before you can punch again
	/// </summary>
	[Property]
	[Category( "Stats" )]
	[Range( 0f, 2f, 0.1f )]
	public float PunchCooldown { get; set; } = 0.5f;

	/// <summary>
	/// How far away you can punch in Hammer Units
	/// </summary>
	[Property]
	[Category( "Stats" )]
	[Range( 0f, 200f, 5f )]
	public float PunchRange { get; set; } = 50f;

	/// <summary>
	/// Where the camera rotates around and the aim originates from
	/// </summary>
	[Property]
	public Vector3 EyePosition { get; set; }

	public Vector3 EyeWorldPosition => Transform.Local.PointToWorld( EyePosition );

	public Angles EyeAngles { get; set; }
	Transform _initialCameraTransform;
	TimeSince _lastPunch;

	protected override void DrawGizmos()
	{
		if ( !Gizmo.IsSelected ) return;

		var draw = Gizmo.Draw;

		draw.LineSphere( EyePosition, 10f );
		draw.LineCylinder( EyePosition, EyePosition + Transform.Rotation.Forward * PunchRange, 5f, 5f, 10 );
	}


	protected override void OnUpdate()
	{
		EyeAngles += Input.AnalogLook;
		EyeAngles = EyeAngles.WithPitch( MathX.Clamp( EyeAngles.pitch, -80f, 80f ) );
		Transform.Rotation = Rotation.FromYaw( EyeAngles.yaw );

		if ( Camera != null )
		{
			var cameraTransform = _initialCameraTransform.RotateAround( EyePosition, EyeAngles.WithYaw( 0f ) );
			var cameraPosition = Transform.Local.PointToWorld( cameraTransform.Position );
			var cameraTrace = Scene.Trace.Ray( EyeWorldPosition, cameraPosition )
				.Size( 5f )
				.IgnoreGameObjectHierarchy( GameObject )
				.WithoutTags( "player" )
				.Run();

			Camera.Transform.Position = cameraTrace.EndPosition;
			Camera.Transform.LocalRotation = cameraTransform.Rotation;
		}
	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();

		if ( Controller == null ) return;

		var wishSpeed = Input.Down( "Run" ) ? RunSpeed : WalkSpeed;
		var wishVelocity = Input.AnalogMove.Normal * wishSpeed * Transform.Rotation;

		Controller.Accelerate( wishVelocity );

		if ( Controller.IsOnGround )
		{
			Controller.Acceleration = 10f;
			Controller.ApplyFriction( 5f, 20f );

			if ( Input.Pressed( "Jump" ) )
			{
				Controller.Punch( Vector3.Up * JumpStrength );

				if ( Animator != null )
					Animator.TriggerJump();
			}
		}
		else
		{
			Controller.Acceleration = 5f;
			Controller.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta;
		}

		Controller.Move();

		if ( Animator != null )
		{
			Animator.IsGrounded = Controller.IsOnGround;
			Animator.WithVelocity( Controller.Velocity );

			if ( _lastPunch >= 2f )
				Animator.HoldType = CitizenAnimationHelper.HoldTypes.None;
		}

		if ( Input.Pressed( "Punch" ) && _lastPunch >= PunchCooldown )
			Punch();
	}

	protected override void OnStart()
	{
		if ( Camera != null )
			_initialCameraTransform = Camera.Transform.Local;

		if ( Components.TryGet<SkinnedModelRenderer>( out var model ) )
		{
			var clothing = ClothingContainer.CreateFromLocalUser();
			clothing.Apply( model );
		}
	}

	public void Punch()
	{

		if ( Animator != null )
		{
			Animator.HoldType = CitizenAnimationHelper.HoldTypes.Punch;
			Animator.Target.Set( "b_attack", true );
		}

		var punchTrace = Scene.Trace
			.FromTo( EyeWorldPosition, EyeWorldPosition + EyeAngles.Forward * PunchRange )
			.Size( 10f )
			.WithoutTags( "player" )
			.IgnoreGameObjectHierarchy( GameObject )
			.Run();

		if ( punchTrace.Hit )
			if ( punchTrace.GameObject.Components.TryGet<UnitInfo>( out var unitInfo ) )
				unitInfo.Damage( PunchStrength );

		_lastPunch = 0f;
	}

	protected override void OnEnabled()
	{
		base.OnEnabled();
	}

	protected override void OnDisabled()
	{
		base.OnDisabled();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}
}

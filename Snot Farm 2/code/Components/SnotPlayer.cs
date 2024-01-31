using Sandbox.Citizen;

public sealed class SnotPlayer : Component
{
	[Property]
	public GameObject Camera { get; set; }

	[Property]
	public CharacterController Controller { get; set; }

	[Property]
	public CitizenAnimationHelper Animator { get; set; }

	/// <summary>
	/// How fast you can walk (Units per second)
	/// </summary>
	[Property]
	[Range( 0f, 400f, 1f )]
	public float WalkSpeed { get; set; } = 120f;

	/// <summary>
	/// How fast you can run (Units per second)
	/// </summary>
	[Property]
	[Range( 0f, 800f, 1f )]
	public float RunSpeed { get; set; } = 250f;

	/// <summary>
	/// How powerful you can jump (Units per second)
	/// </summary>
	[Property]
	[Range( 0f, 1000f, 10f )]
	public float JumpStrength { get; set; } = 400f;


	protected override void OnUpdate()
	{

	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();
	}

	protected override void OnStart()
	{
		base.OnStart();
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

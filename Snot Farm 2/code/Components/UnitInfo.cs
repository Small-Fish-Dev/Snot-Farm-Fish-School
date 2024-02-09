using Sandbox;
using System.Collections.Generic;
using System.Threading.Tasks;

public enum UnitType
{
	/// <summary>
	/// Environmental units or resources
	/// </summary>
	[Icon( "check_box_outline_blank" )]
	None,
	/// <summary>
	/// Players and turrets
	/// </summary>
	[Icon( "nordic_walking" )]
	Player,
	/// <summary>
	/// The enemy :-(
	/// </summary>
	[Icon( "filter_drama" )]
	Snot
}

public sealed class UnitInfo : Component
{
	[Property]
	public UnitType Type { get; set; }

	[Property]
	public float MaxHealth { get; set; } = 5f;

	public float Health { get; private set; }
	public bool Alive { get; private set; } = true;

	public event Action<float> OnDamage;
	public event Action OnDeath;

	protected override void OnUpdate()
	{
		Damage( Time.Delta );
	}

	protected override void OnStart()
	{
		Health = MaxHealth;
	}

	/// <summary>
	/// Damage the unit, clamped, heal if set to negative
	/// </summary>
	/// <param name="damage"></param>
	public void Damage( float damage )
	{
		if ( !Alive ) return;

		Health = Math.Clamp( Health - damage, 0, MaxHealth );

		OnDamage?.Invoke( damage );

		if ( Health <= 0 )
			Krill();
	}

	/// <summary>
	/// Set HP to 0 and Alive to false
	/// </summary>
	public void Krill()
	{
		Health = 0;
		Alive = false;

		OnDeath?.Invoke();

		if ( OnDeath == null )
			GameObject.Destroy();
	}
}

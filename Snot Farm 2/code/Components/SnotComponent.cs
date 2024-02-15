using Sandbox;

public sealed class SnotComponent : Component
{
	[Property]
	public UnitInfo Info { get; set; }

	[Property]
	public SkinnedModelRenderer Model { get; set; }

	protected override void OnStart()
	{
		if ( Info != null )
		{
			Info.OnDamage += HurtAnimation;
			Info.OnDeath += DeathAnimation;
		}
	}

	protected override void OnUpdate()
	{
		if ( Info == null ) return;
		if ( Model == null ) return;
		if ( !Info.Alive ) return;

		var scaledHealth = Info.Health / Info.MaxHealth * 100f;
		var currentHealth = Model.GetFloat( "health" );
		var lerpedHealth = MathX.Lerp( currentHealth, scaledHealth, Time.Delta / 0.1f );
		Model.Set( "health", lerpedHealth );
	}

	public void HurtAnimation( float damage )
	{
		var scaledDamage = damage / Info.MaxHealth * 100f;

		if ( Model != null )
		{
			Model.Set( "damage", scaledDamage );
			Model.Set( "hit", true );
		}
	}

	public void DeathAnimation()
	{
		Model?.Set( "dead", true );
	}
}

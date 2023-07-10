using UnityEngine;

/// <summary>
/// Used to pass data required to process effects. Can be subclassed to add specific info
/// </summary>
public class EffectEventData
{

	public AbilityActor Source => _source;
	private AbilityActor _source;

	public AbilityActor Target => _target;
	private AbilityActor _target;


	public EffectEventData() { }
}

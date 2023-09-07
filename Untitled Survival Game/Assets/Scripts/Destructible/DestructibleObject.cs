using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;
using AbilitySystem;
using FishNet.Component.Transforming;

public class DestructibleObject : Actor
{
	[SerializeField] private Transform _smoothTransform;

	public DestructibleSO DestructibleSO => _destructibleSO;
	[SerializeField] private DestructibleSO _destructibleSO;


	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		Debug.Log("Destructible OnStartNetwork");

		// Doesn't work on clients (event if moved to onstartclient)
		_smoothTransform.position = NetTransform.position;
		_smoothTransform.rotation = NetTransform.rotation;

		NetworkTransform netTrans = NetTransform.gameObject.GetComponent<NetworkTransform>();

		netTrans.OnDataReceived += NetTrans_OnDataReceived;
	}

	private void NetTrans_OnDataReceived(NetworkTransform.TransformData prev, NetworkTransform.TransformData next)
	{
		_smoothTransform.position = next.Position;
		_smoothTransform.rotation = next.Rotation;
	}

	protected override void DoDeathFinish()
	{
		Vector3 spawnPosition = new Vector3(0f, 0.5f, 0f) + NetTransform.position;
		ItemManager.Instance.SpawnWorldItem(_destructibleSO.ItemID, spawnPosition);
	}



	//private IUIEventPublisher _stats;

	//[SerializeField] private GameObject _actorObject;
	//private IActor _actor;

	//[SerializeField]
	//private GameObject _graphicObject;

	//[SerializeField]
	//private WorldStatDisplay _statDisplay;


	//private MeshFilter _meshFilter;

	//private MeshRenderer _meshRenderer;

	//private bool _isAlive;

	//private bool _isDying;

	//private float _deathTimeLeft;

	//[ObserversRpc(BufferLast = true, RunLocally = true)]
	//public void InitializeORPC(int id)
	//{
	//	//Transform parent = DestructibleManager.Instance.DestructibleHolder;

	//	//if (parent == null)
	//	//{
	//	//	Debug.LogError("Failed to set parent for destructible object");
	//	//}
	//	//else
	//	//{
	//	//	transform.SetParent(parent, false);
	//	//}

	//	//DestructibleSO destructibleSO = DestructibleManager.Instance.GetDestructibleSO(id);

	//	//_destructibleSO = destructibleSO;


	//	//if (destructibleSO.GraphicPrefab != null)
	//	//{
	//	//	_graphicObject.SetActive(false);

	//	//	_graphicObject = Instantiate(destructibleSO.GraphicPrefab, _graphicObject.transform.parent, false);
	//	//}

	//	//_meshFilter = _graphicObject.GetComponent<MeshFilter>();
	//	//_meshRenderer = _graphicObject.GetComponent<MeshRenderer>();

	//	//if (destructibleSO.Mesh != null)
	//	//{
	//	//	_meshFilter.sharedMesh = destructibleSO.Mesh;	
	//	//}

	//	//if (destructibleSO.Material != null)
	//	//{
	//	//	_meshRenderer.sharedMaterial = destructibleSO.Material;
	//	//}

	//	//if (_destructibleSO.ParticleEffects != null)
	//	//{
	//	//	_particleHandler.OverrideParticleEffects(_destructibleSO.ParticleEffects);
	//	//}
	//}


	//public override void OnStartNetwork()
	//{
	//	base.OnStartNetwork();

	//	//_abilityActor = GetComponent<AbilityActor>();

	//	// Don't really want to rely on UI events to tell when a stat has changed
	//	// but most behaviour involving stats and death will be moved soon regardless
	//	//_stats = GetComponent<IUIEventPublisher>();

	//	//if (_stats != null)
	//	//{
	//	//	//_stats.UIEvent += Stats_StatChanged;
	//	//}


	//	//if (_actor == null)
	//	//{
	//	//	_actor = _actorObject.GetComponent<IActor>();

	//	//	if (_actor == null)
	//	//	{
	//	//		Debug.LogError("DestructibleObject failed to find IActor component");
	//	//	}
	//	//}

	//	//_actor.DeathStarted += Actor_DeathStarted;
	//	//_actor.DeathFinished += Actor_DeathFinished;


	//	//if (IsServer)
	//	//{
	//	//	//TimeManager.OnPostTick += OnTick;
	//	//}
	//	//else
	//	//{
	//	//	enabled = false;
	//	//}

	//	//_isAlive = true;

	//	//_isDying = false;

	//	//_deathTimeLeft = 0f;
	//}


	//private void Actor_DeathStarted(IActor actor, ActorEventData data)
	//{
	//	_graphicObject.SetActive(false);

	//	//_statDisplay.Show(false);

	//	GetComponent<Collider>().enabled = false;

	//	if (IsServer)
	//	{
	//		Vector3 spawnPosition = new Vector3(0f, 0.5f, 0f) + transform.position;

	//		ItemManager.Instance.SpawnWorldItem(_destructibleSO.ItemID, spawnPosition);
	//	}
	//}


	//private void OnDestroy()
	//{
	//	if (_stats != null)
	//	{
	//		//_stats.UIEvent -= Stats_StatChanged;
	//	}

	//	if (_actor != null)
	//	{
	//		_actor.DeathStarted -= Actor_DeathStarted;
	//		_actor.DeathFinished -= Actor_DeathFinished;
	//	}
	//}

	//private void Stats_StatChanged(UIEventData data)
	//{
	//	if (data.TagString == "Health" && data is UIFloatChangeEventData statData)
	//	{
	//		if (statData.Value == statData.MinValue)
	//		{
	//			OnDeathStart();
	//		}
	//	}
	//}


	// Needs to integrate AbilitySystem
	// (and do away with depending on syncvars for timing sensitive tasks)
	// Could just fire off cues for particles on death but may want to implement death as an ability
	// after async tasks are implemented
	//private void OnDeathStart()
	//{
	//	//Debug.Log("OnDeathStart " + TimeManager.Tick);

	//	// This is a little to soon to be despawning since this will get triggered by the stat change on the server
	//	// before the OnChange has been executed on the client
	//	//Despawn(gameObject);

	//	// Need some way for the server to know Death effects are finished even though they dont play on the server
	//	//_isDying = true;

	//	//_deathTimeLeft = _destructibleSO.DeathTime;

	//	// I would really like either a way to know the syncvar send rate or better a callback when syncvars are actually sent
	//	// The default sync rate of 0.1 is probably enough
	//	//if (_deathTimeLeft < 0.1f)
	//	//{
	//	//	_deathTimeLeft = 0.1f;
	//	//}

	//	//_abilityActor.IsAlive = false;

	//	//_abilityActor.PlayParticles("DEATH");

	//	_graphicObject.SetActive(false);

	//	_statDisplay.Show(false);

	//	GetComponent<Collider>().enabled = false;

	//	if (IsServer)
	//	{
	//		Vector3 spawnPosition = new Vector3(0f, 0.5f, 0f) + transform.position;

	//		ItemManager.Instance.SpawnWorldItem(_destructibleSO.ItemID, spawnPosition);
	//	}
	//}


	//private void Actor_DeathFinished(IActor actor, ActorEventData data)
	//{
	//	if (IsServer)
	//	{
	//		Despawn(gameObject);
	//	}
	//}

	//private void OnTick()
	//{
	//	// Maybe better to move to Timemanager
	//	// I hoped waiting one frame would be enough time to prevent
	//	// OnDeathFinish from occuring before the OnChange event was sent to clients

	//	// Check this before to force a one tick delay between condition changing and responding
	//	// Did not work on client host

	//	// So Since syncvars update on a time based interval and may take several ticks before sync is sent to client
	//	// There is no way to know for sure that the OnChanged callback will be called on the client before
	//	// Executing something initiated by OnChanged callback called on the server

	//	// The only way I can really think is to do some form of handshake (potentially unsafe)
	//	// Or wait at least a minimum amount of time (ie. At least as long as the sync rate
	//	// And there is no method to get sync rate from server :} 0.1 seconds is the default and seems enough
	//	// May need to double it if packets are being recieved out of order not sure how that works

	//	//if (IsSpawned && !_isAlive)
	//	//{
	//	//	//OnDeathFinish();
	//	//}


	//	//if (_isAlive && _isDying)
	//	//{
	//	//	_deathTimeLeft -= (float)TimeManager.TickDelta;

	//	//	if (_deathTimeLeft <= 0f)
	//	//	{
	//	//		_isDying = false;
	//	//		_isAlive = false;
	//	//	}
	//	//}
	//}
}

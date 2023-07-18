using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	public class AbilityActor : NetworkBehaviour
	{
		[SerializeField]
		private Stats _stats;

		[SerializeField]
		private ToolType _toolType;
		public ToolType ToolType { get { return _toolType; } }

		[SerializeField]
		private float _toolPower;
		public float ToolPower { get { return _toolPower; } }

		[SerializeField]
		private Animator _animator;
		public Animator Animator => _animator;

		[SerializeField]
		private AnimatorOverrideController _animatorOverride;

		[SerializeField]
		private AudioSource _audioSource;
		public AudioSource AudioSource => _audioSource;

		[SerializeField]
		private ParticleHandler _particleHandler;

		public AttachPoints AttachPoints => _attachPoints;
		[SerializeField] private AttachPoints _attachPoints;

		[SerializeField]
		private TransformAnimator _transformAnimator;
		public TransformAnimator TransformAnimator => _transformAnimator;

		[Header("Optional")]

		[SerializeField]
		private AbilityActor _parentActor;

		[SerializeField]
		private Transform _viewTransform;
		public Transform ViewTransform { get { return _viewTransform; } }

		[SerializeField]
		private float _viewRange;
		public float ViewRange { get { return _viewRange; } }

		[SerializeField]
		private LayerMask _viewMask;
		public LayerMask ViewMask { get { return _viewMask; } }

		[SerializeField]
		private Inventory _inventory;
		public Inventory Inventory { get { return _inventory; } }

		[SerializeField]
		private Transform _projectileSource;

		[SerializeField]
		private Agent _agent;

		private AbilityItem _abilityItem;
		public AbilityItem AbilityItem { get { return _abilityItem; } }

		private ProjectileBase _projectile;


		private bool _isAlive;
		public bool IsAlive { get { return _isAlive; } set { _isAlive = value; } }

		

		public override void OnStartNetwork()
		{
			base.OnStartNetwork();

			_isAlive = true;

			SetupStartingAbilities();
		}

		public override void OnStartClient()
		{
			base.OnStartClient();
		}

		public void Initialize(Animator animator)
		{
			_animator = animator;

			if (_animatorOverride != null)
			{
				_animator.runtimeAnimatorController = _animatorOverride;
			}
		}


		public void Initialize(DestructibleSO destructibleSO)
		{
			_toolType = destructibleSO.ToolType;

			_toolPower = destructibleSO.ToolPower;
		}


		public void SetAbilityItem(AbilityItem abilityItem)
		{
			_abilityItem = abilityItem;

			if (abilityItem == null)
			{
				_projectileSource.localPosition = Vector3.zero;
				//_projectileSource.localRotation = Quaternion.identity;
			}
			else
			{
				_projectileSource.localPosition = abilityItem.ItemSO.ProjectileSource;
			}
		}


		public void SetProjectileSource(ProjectileSource projectileSource)
		{
			if (projectileSource != null)
			{
				_projectileSource = projectileSource.Target;
			}
			else
			{
				_projectileSource = transform;
			}
		}

		#region Stats Accessors

		public bool HasStat(StatType statType)
		{
			return _stats.HasStat(statType);
		}


		public float GetStatValue(StatType statType)
		{
			return _stats.GetStatValue(statType);
		}


		public float GetStatMax(StatType statType)
		{
			return _stats.GetStatMax(statType);
		}


		[Server]
		public void SetStatValue(StatType statType, float value)
		{
			_stats.SetStat(statType, value);
		}


		[Server]
		public void AddToStat(StatType statType, float value)
		{
			_stats.AddToStat(statType, value);
		}


		#endregion


		#region VisualAndAudioEffects

		public void SetAnimOverride(string animToOverride, AnimationClip animation)
		{
			if (_animatorOverride != null)
			{
				Debug.Log("Override: " + animToOverride + " On: " + gameObject.name);
				_animatorOverride[animToOverride] = animation;
			}
		}


		public void SetAnimTrigger(string animTrigger)
		{
			if (_animator != null)
			{
				Debug.Log("Set Trigger: " + animTrigger + " On: " + transform.parent.gameObject.name);
				_animator.SetTrigger(animTrigger);
			}
		}


		public void PlaySound(AudioClip sound)
		{

			if (IsServerOnly)
			{
				// No reason to play audio with no client and its so annoying every sound being doubled

				Debug.Log("IsServerOnly was true?");
				return;
			}


			if (sound != null && _audioSource != null)
			{
				//Debug.Log("Playing Sound: " + sound.name);

				_audioSource.PlayOneShot(sound);
			}
			else
			{
				Debug.Log("Attempted to Play Sound On: " + gameObject.name);
			}

		}


		public void PlayParticles(string name)
		{
			if (_particleHandler != null)
			{
				_particleHandler.PlayParticles(name);
			}
		}


		public void PlayParticles(ParticleSystem particleSystem)
		{
			if (particleSystem != null)
			{
				particleSystem.Play();
			}
		}


		public void PlayTransformAnimation(string transformAnimation)
		{
			if (_transformAnimator != null)
			{
				_transformAnimator.PlayAnimation(transformAnimation);
			}
		}


		[Server]
		public void KnockBack(Vector3 direction, float strength)
		{
			// Eventually want to check for immunity / resistance (would that be in stats?)

			Agent agent = GetComponent<Agent>();

			if (agent != null)
			{
				agent.KnockBack(direction, strength);
			}
		}


		[Server]
		public void UseItem()
		{
			Debug.LogError("AbilityActor UseItem");

			if (_abilityItem == null)
			{
				Debug.LogError("Attempted to Use null item");
				return;
			}

			if (_parentActor != null)
			{
				if (_parentActor.Inventory != null)
				{
					_parentActor.Inventory.UseItem(_parentActor, _abilityItem);
				}
			}
			else if (_inventory != null)
			{
				_inventory.UseItem(this, _abilityItem);
			}
		}


		[Server]
		public void SpawnProjectile(ProjectileBase projectile)
		{
			Debug.Log(_projectileSource.forward);

			if (_projectileSource != null)
			{
				_projectile = ProjectileManager.SpawnProjectile(projectile, _projectileSource.position, _projectileSource.rotation);

				_projectile.SetFollowTarget(_projectileSource);
			}
			else
			{
				Debug.LogError("Missing Projectile Source");
			}
		}


		[Server]
		public void LaunchProjectile(Vector3 velocity, bool align)
		{
			if (_projectile == null)
			{
				return;
			}


			if (_agent != null && _agent.AttackTarget != null)
			{
				Vector3 targetPos = _agent.AttackTarget.transform.position;
				targetPos.y += 1.4f;

				Vector3 toTargetVector = targetPos - _projectile.NetworkTransform.position;

				Quaternion rotToTarget = Quaternion.FromToRotation(Vector3.forward, toTargetVector);

				velocity = rotToTarget * velocity;

			}
			else if (_viewTransform != null)
			{
				velocity = ViewTransform.TransformDirection(velocity);
			}
			else if (_parentActor != null && _parentActor.ViewTransform != null)
			{
				velocity = _parentActor.ViewTransform.TransformDirection(velocity);
			}
			else
			{
				velocity = transform.TransformDirection(velocity);
			}

			_projectile.SetFollowTarget(null);

			_projectile.Launch(velocity, align);

			_projectile = null;
		}

		#endregion


		#region AbilitySystem

		public bool AsOwner => IsOwner;

		public bool AsServer => IsServer;


		// Allow intial abilities to be defined in the inspector
		[SerializeField] private List<Ability> _startingAbilities;

		// Instantiated ability instances
		private List<AbilityHandle> _abilities;

		private List<EffectHandle> _activeEffects = new List<EffectHandle>();

		private Dictionary<AbilityTrait, Cue> _cueOverrides;


		[Server]
		public void GiveAbility(Ability ability)
		{

		}


		public void ApplyEffect(EffectHandle effectHandle)
		{
			Effect effect = effectHandle.Effect;

			if (effect == null)
			{
				return;
			}


			// First check that the effect can be applied based on traits
			if (!CanApply(effectHandle))
			{
				return;
			}


			if (effect.DurationType == DurationType.Instant)
			{
				// Fire off any cues if this is a client
				if (IsClient)
				{
					HandleCues(effectHandle, CueEventType.OnExecute);
				}


				// Handle Stat changes only on server
				if (IsServer)
				{
					HandleStatChanges(effectHandle);
				}
			}
		}


		public bool TryHandleCue(AbilityTrait trait, CueEventType eventType, CueEventData data)
		{
			if (_cueOverrides != null && _cueOverrides.TryGetValue(trait, out Cue cue))
			{
				Debug.LogWarning($"handling cue override: {cue.name}");
				cue.HandleCue(eventType, data);
				return true;
			}

			return false;
		}


		public void AddCueOverrides(Cue[] cues)
		{
			_cueOverrides = new Dictionary<AbilityTrait, Cue>();

			foreach (Cue cue in cues)
			{
				_cueOverrides.Add(cue.Trait, cue);
			}
		}


		public void ActivateAbility(int abilityID)
		{
			if (IsServer)
			{
				ActivateAbilityAsServer(abilityID);
			}
			else if (IsOwner)
			{
				ActivateAbilityAsClient(abilityID);
			}
		}


		private void ActivateAbilityAsClient(int abilityID)
		{
			if (!TryFindAbilityFromID(abilityID, out AbilityHandle abilityHandle))
			{
				return;
			}


			// Activate the ability locally and request the server activate remotely
			if (abilityHandle.CanActivate())
			{
				abilityHandle.Activate();

				ActivateAbilitySRPC(abilityID);
			}
		}


		[Server]
		private void ActivateAbilityAsServer(int abilityID)
		{
			if (!TryFindAbilityFromID(abilityID, out AbilityHandle abilityHandle))
			{
				return;
			}

			if (abilityHandle.CanActivate())
			{
				abilityHandle.Activate();

				ActivateAbilityORPC(abilityID);
			}
		}


		// Request the server to activate the ability
		[ServerRpc]
		private void ActivateAbilitySRPC(int abilityID)
		{
			ActivateAbilityAsServer(abilityID);
		}


		// Activate the ability on observers in order to play cues (but not localhost)
		// may be preferable to activate only the cues but they typically need extra data
		// this will save a lot of bandwidth unless I find a better way to activate cues over the network
		[ObserversRpc(ExcludeOwner = true, ExcludeServer = true)]
		private void ActivateAbilityORPC(int abilityID)
		{
			if (!TryFindAbilityFromID(abilityID, out AbilityHandle abilityHandle))
			{
				return;
			}

			abilityHandle.Activate();
		}


		// The underlying relationship between ID and ability handle will most likely be more complex
		// This will suffice for testing
		private bool TryFindAbilityFromID(int abilityID, out AbilityHandle abilityHandle)
		{
			if (abilityID > -1 && abilityID < _abilities.Count)
			{
				abilityHandle = _abilities[abilityID];
				return true;
			}

			abilityHandle = null;
			return false;
		}


		private void SetupStartingAbilities()
		{
			_abilities = new List<AbilityHandle>();

			foreach (Ability ability in _startingAbilities)
			{
				_abilities.Add(new AbilityHandle(ability, this));
			}
		}


		// Handle in update for now but may move to TimeManager eventually
		private void Tick(float deltaTime)
		{
			TickAbilities(deltaTime);

			TickEffects(deltaTime);
		}

		// If cooldown ends up being the only thing updated here I may find a different way to handle cooldowns
		private void TickAbilities(float deltaTime)
		{
			foreach (AbilityHandle handle in _abilities)
			{
				handle.Tick(deltaTime);
			}
		}


		private void TickEffects(float deltaTime)
		{
			foreach (EffectHandle effect in _activeEffects)
			{
				effect.Tick(deltaTime);
			}
		}


		private bool CanApply(EffectHandle effectHandle)
		{
			return true;
		}


		private void HandleCues(EffectHandle effectHandle, CueEventType cueEventType)
		{
			AbilityTrait[] traits = effectHandle.Effect.Cues;

			CueEventData data = new CueEventData()
			{
				Target = this,
			};

			foreach(AbilityTrait trait in traits)
			{
				CueManager.HandleCue(trait, cueEventType, data);
			}
		}


		private void HandleStatChanges(EffectHandle effectHandle)
		{
			Debug.LogError($"HandleStatChanges");

			foreach (StatModifier modifier in effectHandle.Effect.Modifiers)
			{
				modifier.ApplyModifier(_stats);
			}
		}


		#endregion

		private void Update()
		{
			Tick(Time.deltaTime);
		}
	}
}

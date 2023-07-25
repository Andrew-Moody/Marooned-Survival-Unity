using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using AbilityActor = AbilitySystem.AbilityActor;


public class CombatInput : NetworkBehaviour
{
	[SerializeField]
	private LegacyAbility.Combatant _combatant;

	private AbilityActor _abilityActor;

	private LegacyAbility.AbilityActor _legacyActor;

	[SerializeField]
	private float _interactRange;

	[SerializeField]
	private LayerMask _interactMask;

	private const string HEADSUP = "HeadsupUI";
	private const string INVENTORY = "InventoryUI";
	private const string SETTINGS = "SettingsUI";


	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		_abilityActor = GetComponent<AbilityActor>();

		//_legacyActor = _combatant.gameObject.GetComponent<LegacyAbility.AbilityActor>();

		//_combatant.Initialize();
	}


	// In most cases this will suffice for components that should only be active on the Owning client (player input for example)
	// if a client hacks this to leave it enabled any serverRPCs that require ownership will not be allowed to execute
	public override void OnStartClient()
	{
		base.OnStartClient();

		if (!IsOwner)
		{
			this.enabled = false;
		}
	}



	void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			if (UIManager.CheckStackTop(INVENTORY))
			{
				UIManager.HideStackTop(true);
				PlayerInput.SetFPSMode(true);
			}
			else
			{
				UIManager.ShowPanel(INVENTORY, pushToStack: true);
				PlayerInput.SetFPSMode(false);
			}

		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (UIManager.CheckStackTop(HEADSUP))
			{
				UIManager.ShowPanel(SETTINGS, PlayerOptions.GetSettingsData(), pushToStack: true);
			}
			else
			{
				UIManager.HideStackTop(true);
			}

			PlayerInput.SetFPSMode(UIManager.CheckStackTop(HEADSUP));
		}

		
		if (PlayerInput.FPSMode)
		{
			if (Input.GetMouseButton(0))
			{
				//int attackIdx = _combatant.ChooseAbility();
				//Attack(attackIdx);
			}

			if (Input.GetMouseButtonDown(0))
			{
				_abilityActor.ActivateAbility(0);
			}

			if (Input.GetMouseButtonDown(1))
			{
				Interact();
			}

			if (Input.GetKeyDown(KeyCode.F))
			{
				_abilityActor.HandleEvent(this, new AsyncTasks.TaskEventData());
			}
		}
	}


	private void Attack(int attackIdx)
	{
		//Debug.Log("Attack index: " + attackIdx);

		LegacyAbility.Ability attack = _combatant.GetAbility(attackIdx);

		if (attack == null)
		{
			//Debug.Log("Attack was null");
			return;
		}

		if (attack.Useable(IsServer, _legacyActor))
		{
			_combatant.UseAbility(attackIdx);
		}
	}



	[ServerRpc]
	private void Interact()
	{
		Transform view = _abilityActor.ViewTransform;

		if (Physics.Raycast(view.position, view.forward, out RaycastHit hit, _interactRange, _interactMask))
		{
			if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out Interactable interactible))
			{
				interactible.Interact(Owner);
			}
		}
	}
}

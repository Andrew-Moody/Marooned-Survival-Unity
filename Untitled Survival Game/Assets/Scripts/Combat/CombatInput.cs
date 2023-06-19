using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CombatInput : NetworkBehaviour
{
	[SerializeField]
	private Combatant _combatant;

	private AbilityActor _abilityActor;

	[SerializeField]
	private float _interactRange;

	[SerializeField]
	private LayerMask _interactMask;


	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		_abilityActor = _combatant.gameObject.GetComponent<AbilityActor>();

		_combatant.Initialize();
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
			if (UIManager.CheckStackTop("InventoryUI"))
			{
				UIManager.HideStackTop(true);
				CameraController.Instance.SetFPSMode(true);
			}
			else
			{
				UIManager.ShowPanel("InventoryUI", pushToStack: true);
				CameraController.Instance.SetFPSMode(false);
			}

		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (UIManager.CheckStackTop("HotbarUI"))
			{
				UIManager.ShowPanel("SettingsUI", PlayerOptions.GetSettingsData(), pushToStack: true);
			}
			else
			{
				UIManager.HideStackTop(true);
			}


			if (UIManager.CheckStackTop("HotbarUI"))
			{
				CameraController.Instance.SetFPSMode(true);

				Debug.LogError($"Hotbar top of stack {Cursor.lockState} {Cursor.visible}");
			}
			else
			{
				CameraController.Instance.SetFPSMode(false);
			}			
		}

		// I know this is awfull and I need to make an Input Manager


		if (CameraController.Instance.GetFPSMode())
		{
			if (Input.GetMouseButton(0))
			{
				int attackIdx = _combatant.ChooseAbility();
				Attack(attackIdx);
			}


			if (Input.GetMouseButtonDown(1))
			{
				Interact();
			}
		}
		else
		{
			

			//if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
			//{
			//    CameraController.Instance.SetFPSMode(true);
			//    UIManager.Instance.HideCraftingUI();
			//}
		}
		
	}


	private void Attack(int attackIdx)
	{
		//Debug.Log("Attack index: " + attackIdx);

		Ability attack = _combatant.GetAbility(attackIdx);

		if (attack == null)
		{
			//Debug.Log("Attack was null");
			return;
		}

		if (attack.Useable(IsServer, _abilityActor))
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

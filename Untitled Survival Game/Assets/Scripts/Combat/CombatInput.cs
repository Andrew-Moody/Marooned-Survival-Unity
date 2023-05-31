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


	private bool _craftingUIOpen = false;
	private bool _inventoryOpen = false;
	private bool _menuOpen = false;


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
			_inventoryOpen = !_inventoryOpen;

			CameraController.Instance.SetFPSMode(!_inventoryOpen);

		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (_craftingUIOpen)
			{
				CameraController.Instance.SetFPSMode(true);
				UIManager.Instance.HideCraftingUI();
				_craftingUIOpen = false;
			}
			else if (_inventoryOpen)
			{
				CameraController.Instance.SetFPSMode(true);
				//UIManager.Instance.Hide("Inventory");
				Debug.Log("Hide Inventory");
				_inventoryOpen = false;
			}
			else if (_menuOpen)
			{
				CameraController.Instance.SetFPSMode(true);
				_menuOpen = false;
			}
			else
			{
				CameraController.Instance.SetFPSMode(false);
				_menuOpen = true;
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


	private void Interact()
	{
		Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _interactRange, _interactMask);

		if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out DestructibleObject destructible))
		{
			_craftingUIOpen = true;
			destructible.Interact();
		}
	}
}

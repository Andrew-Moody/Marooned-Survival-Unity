using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingStation : Interactable
{
	[SerializeField]
	private CraftingStationSO _craftingStationSO;

	private void Awake()
	{
		_interactPrompt = $"Right Click to Use {_craftingStationSO.name}";
	}

	public override void Interact(NetworkConnection user)
	{
		base.Interact(user);

		InteractTRPC(user);
	}


	[TargetRpc]
	private void InteractTRPC(NetworkConnection user)
	{
		PlayerInput.SetFPSMode(false);

		UIManager.ShowPanel("CraftingUI", new CraftingUIPanelData(_craftingStationSO.Recipes), true);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingStation : MonoBehaviour
{
	[SerializeField]
	private CraftingStationSO _craftingStationSO;
    

    public void Interact()
	{
		CameraController.Instance.SetFPSMode(false);

		UIManager.Instance.ShowCraftingUI(_craftingStationSO.Recipes);
	}
}

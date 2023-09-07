using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWorldItem : NetworkBehaviour
{
	[SerializeField] private float _range;

	private LayerMask _itemMask;

	public override void OnStartNetwork()
	{
		base.OnStartNetwork();
		if (!IsServer)
			this.enabled = false;
	}

	// Start is called before the first frame update
	void Start()
	{
		_itemMask = 1 << 7;
	}

	// Update is called once per frame
	void Update()
	{
		if (!IsServer)
			Debug.Log("Haha missed me!");


		Vector3 origin = transform.position + new Vector3(0f, 0.4f, 0f);
		
		if (Physics.CheckSphere(origin, _range, _itemMask))
		{
			Collider[] colliders = Physics.OverlapSphere(origin, _range, _itemMask);

			for (int i = 0; i < colliders.Length; i++)
			{
				WorldItem item = colliders[i].gameObject.GetComponent<WorldItem>();

				// It takes a bit of time once an object is marked for despawn that it actually is removed
				if (item != null && item.IsSpawned)
				{
					ItemNetData itemData = item.GetItemData();

					// Cant use Inventory.ClientInstance because the host handles all players not just the one it owns
					Inventory inventory = gameObject.GetComponent<Inventory>();
					if (inventory == null)
					{
						Debug.LogError("Inventory was null for Client: " + OwnerId);
						return;
					}

					if (inventory.TryAcceptItem(ref itemData))
					{
						Despawn(item.gameObject);
					}
					else
					{
						// Quantity may have changed
						item.SetItem(itemData);
						item.ObserversSetupWorldItem(itemData);
					}
				}
			}
		}
	}
}

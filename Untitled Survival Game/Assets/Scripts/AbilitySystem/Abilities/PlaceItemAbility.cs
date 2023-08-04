using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "PlaceItemAbility", menuName = "AbilitySystem/Ability/PlaceItemAbility")]
	public class PlaceItemAbility : Ability
	{
		[SerializeField] private PlacementTargeter _targeter;

		[SerializeField] private AbilityTrait _placementCue;

		public override void Activate(AbilityHandle handle)
		{
			PointNormalTargetResult pointNormal = FindSpotOnGround(handle);

			if (pointNormal != null)
			{
				CueEventData data = new CueEventData()
				{
					Target = handle.AbilityData.User,
				};

				CueManager.HandleCue(_placementCue, CueEventType.OnExecute, data);

				if (handle.User.IsServer)
				{
					PlaceItem(handle, pointNormal);
				}
			}

			End(handle);
		}


		private PointNormalTargetResult FindSpotOnGround(AbilityHandle handle)
		{
			TargetingArgs args = new RaycastTargetArgs() { Range = 4f };

			List<TargetResult> results = _targeter.FindTargets(handle.AbilityData.User, args);

			if (results.Count > 0 && results[0] is PointNormalTargetResult result)
			{
				return result;
			}

			return null;
		}


		private void PlaceItem(AbilityHandle handle, PointNormalTargetResult pointNormal)
		{
			ItemHandle item = null;

			if (handle.AbilityData.AbilityEventData is ItemActivateEventData data)
			{
				item = data.Item;
			}
			else
			{
				Debug.LogError("Attempting to use a item based ability but item data was missing");
				return;
			}

			if (item == null)
			{
				Debug.LogError("Attempting to use a item based ability but item data was missing an itemhandle");
				return;
			}


			// Calculate the placement rotation such that the up direction is aligned with the ground normal
			// and the forward direction is facing the user
			Transform view = handle.Actor.ViewTransform.transform;

			Vector3 right = Vector3.Cross(view.forward, pointNormal.Normal).normalized;
			Vector3 forward = Vector3.Cross(right, pointNormal.Normal).normalized;

			Quaternion rotation = Quaternion.LookRotation(forward, pointNormal.Normal);

			DestructibleManager.Instance.PlaceItem(item.ItemID, pointNormal.Point, rotation);


			// Finally need to deduct quantity from the item in the inventory
			item.TryRemoveItem(1);
		}
	}
}


using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : NetworkBehaviour
{
	protected string _interactPrompt = "Right Click to Interact";
	public string InteractPrompt => _interactPrompt;


	[Server]
	public virtual void Interact(NetworkConnection user)
	{
		Debug.LogError($"Client {user.ClientId} Interacted with {gameObject.name}");
	}
}

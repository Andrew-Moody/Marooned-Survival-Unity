using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUser : NetworkBehaviour
{
	private AbilityActor _actor;


	// Array of starting effects to initialize attributes

	// Array of abilities that the player starts with



	public override void OnStartServer()
	{
		base.OnStartServer();

		InitializeAttributes();

		GiveStartingAbilities();
	}


	private void InitializeAttributes()
	{
		// Apply each initial effect
	}


	private void GiveStartingAbilities()
	{
		// Give the actor the starting abilities
	}
}

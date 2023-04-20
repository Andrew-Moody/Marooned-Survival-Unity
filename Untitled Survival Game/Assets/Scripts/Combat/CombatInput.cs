using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatInput : NetworkBehaviour
{
    [SerializeField]
    private Combatant _combatant;

    private AbilityActor _abilityActor;

    [SerializeField]
    private float _minCoolDown;

    private float _coolDownRemaining;

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
	}



	void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
		{
            int attackIdx = _combatant.ChooseAbility();
            Attack(attackIdx);
		}

        // I know this is awfull and I need to make an Input Manager
        if (CameraController.Instance.GetFPSMode() && Input.GetMouseButton(0))
		{
            int attackIdx = _combatant.ChooseAbility();
            Attack(attackIdx);
        }
    }


    private void Attack(int attackIdx)
    {
        Debug.Log("Attack index: " + attackIdx);

        Ability attack = _combatant.GetAbility(attackIdx);

        if (attack == null)
        {
            Debug.Log("Attack was null");
            Debug.Log(_combatant.GetAbility(attackIdx));
            return;
        }

        if (attack.Useable(_abilityActor))
        {
            _combatant.UseAbility(attackIdx);
        }
    }
}

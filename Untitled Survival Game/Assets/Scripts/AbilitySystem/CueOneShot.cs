using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CueOneShot : Cue
{

	public override sealed CueNotifyType GetNotifyType() => CueNotifyType.NonInstantiated;

	// In theory, a one shot fx needs only OnExecute

	public override sealed void OnActivate(CueEventData data)
	{
		base.OnActivate(data);
	}


	public override sealed void OnRemove()
	{
		base.OnRemove();
	}


	public override sealed void OnVisible(CueEventData data)
	{
		base.OnVisible(data);
	}
}

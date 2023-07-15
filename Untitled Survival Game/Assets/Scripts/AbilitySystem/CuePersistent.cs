using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuePersistent : Cue
{
	public override sealed CueNotifyType GetNotifyType() => CueNotifyType.Instantiated;


	// In theory, OnExecute will not be called on persitent cues
	public override sealed void OnExecute(CueEventData data)
	{
		base.OnExecute(data);
	}
}

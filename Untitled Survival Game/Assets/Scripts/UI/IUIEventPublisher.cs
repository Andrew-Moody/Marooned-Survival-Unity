using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIEventPublisher
{
	public event System.Action<UIEventData> UIEvent;
}

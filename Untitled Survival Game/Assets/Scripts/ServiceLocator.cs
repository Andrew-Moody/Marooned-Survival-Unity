using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    public static ServiceLocator Instance;


	private Dictionary<string, object> _services;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}


	public void RegisterService(string name, object service)
	{
		_services[name] = service;
	}


	public object GetService(string name)
	{
		object service;
		_services.TryGetValue(name, out service);

		return service;
	}
}

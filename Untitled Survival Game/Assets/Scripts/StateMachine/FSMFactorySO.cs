using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Factory")]
public class FSMFactorySO : ScriptableObject
{
	[SerializeField]
	private bool _getStatesOnReload;

	public static string[] Conditions
	{
		get { return Instance._conditions; }
	}

	[SerializeField]
	private string[] _conditions;

	private const string FACTORY_NAME = "FSMFactorySO";

	private static FSMFactorySO _instance;
	public static FSMFactorySO Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Resources.Load<FSMFactorySO>(FACTORY_NAME);
				Debug.Log("Had to load FSMFactory from Resources");

				if (_instance == null)
				{
					Debug.LogError("Failed to get FSMFactory Instance with name: " + FACTORY_NAME);
				}
			}

			return _instance;
		}
	}


	public static List<string> States
	{
		get
		{
			if (Instance != null)
			{
				if (Instance._states == null)
				{
					ReloadFSMFactory();
				}
			}
			else
			{
				Debug.Log("A valid SMFactorySO must exist to get states");
				return null;
			}

			return Instance._states;
		}

		private set
		{
			if (Instance == null)
			{
				Debug.Log("A valid SMFactorySO must exist to set states");
			}
			else
			{
				Instance._states = value;
			}
		}
	}


	public static string[] Options
	{
		get
		{
			return States.ToArray();
		}
	}

	[SerializeField]
	private List<string> _states;


	public static void ReloadFSMFactory()
	{
		Debug.Log("Reloaded States: " + Instance.ToString());


		var assembly = Assembly.GetExecutingAssembly();

		IEnumerable<Type> types = assembly.GetTypes()
			.Where(typeof(IState).IsAssignableFrom)
			.Where(t => typeof(IState) != t)
			.Where(t => typeof(BaseState) != t); // Exclude the BaseState from the list (and the interface IState)

		States = new List<string>();

		foreach (Type type in types)
		{
			States.Add(type.ToString());
			//Debug.Log(type.ToString());
		}
	}

#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnScriptReload()
	{
		if (Instance._getStatesOnReload)
		{
			ReloadFSMFactory();
		}
	}
#endif

	delegate BaseState StateFactoryMethod();

	delegate BaseCondition ConditionFactoryMethod();

	private static Dictionary<StateType, StateFactoryMethod> _stateFactoryMethods;

	private static Dictionary<ConditionType, ConditionFactoryMethod> _conditionFactoryMethods;

	public static BaseState CreateState(StateType stateType)
	{
		if (stateType == StateType.None)
		{
			return null;
		}

		if (_stateFactoryMethods == null)
		{
			_stateFactoryMethods = new Dictionary<StateType, StateFactoryMethod>();
		}

		Debug.Log("Creating State");

		StateFactoryMethod factoryMethod;

		// Find the desired factoryMethod or create a new entry if not found
		if (!_stateFactoryMethods.TryGetValue(stateType, out factoryMethod))
		{
			string typeString = stateType.ToString() + "State";

			Type type = Type.GetType(typeString);

			if (type == null || !typeof(BaseState).IsAssignableFrom(type))
			{
				Debug.LogWarning($"State of type {typeString} that derives from BaseState was not found in the Assembly");
				return null;
			}

			MethodInfo methodInfo = type.GetMethod("Create");

			if (methodInfo == null)
			{
				Debug.LogWarning($"State of type {typeString} does not define a Create method");
				return null;
			}

			factoryMethod = Delegate.CreateDelegate(typeof(StateFactoryMethod), methodInfo) as StateFactoryMethod;

			_stateFactoryMethods.Add(stateType, factoryMethod);
		}

		return factoryMethod?.Invoke();
	}


	public static BaseCondition CreateCondition(ConditionType conditionType)
	{
		if (conditionType == ConditionType.None)
		{
			return null;
		}

		if (_conditionFactoryMethods == null)
		{
			_conditionFactoryMethods = new Dictionary<ConditionType, ConditionFactoryMethod>();
		}

		ConditionFactoryMethod factoryMethod;

		// Find the desired factoryMethod or create a new entry if not found
		if (!_conditionFactoryMethods.TryGetValue(conditionType, out factoryMethod))
		{
			string typeString = conditionType.ToString() + "Condition";

			Type type = Type.GetType(typeString);

			if (type == null || !typeof(BaseCondition).IsAssignableFrom(type))
			{
				Debug.LogWarning($"Condition of type {typeString} that derives from BaseCondition was not found in the Assembly");
				return null;
			}

			MethodInfo methodInfo = type.GetMethod("Create");

			if (methodInfo == null)
			{
				Debug.LogWarning($"Condition of type {typeString} does not define a Create method");
				return null;
			}

			factoryMethod = Delegate.CreateDelegate(typeof(ConditionFactoryMethod), methodInfo) as ConditionFactoryMethod;

			_conditionFactoryMethods.Add(conditionType, factoryMethod);
		}

		return factoryMethod?.Invoke();
	}
}

public enum StateType
{
	None,
	Roam,
	Attack
}


public enum ConditionType
{
	None,
	Greater,
	Less,
	Equal,
	And,
	Or,
	Not,
	TargetInView
}

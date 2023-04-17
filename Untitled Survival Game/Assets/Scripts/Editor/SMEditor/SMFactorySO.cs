using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

[CreateAssetMenu(menuName = "StateMachine/Factory")]
public class SMFactorySO : ScriptableObject
{
    public bool GetStatesOnReload;

    public static string[] Conditions
	{
        get { return Instance._conditions; }
	}

    [SerializeField]
    private string[] _conditions;

    private static SMFactorySO _instance;
    public static SMFactorySO Instance
	{
		get
		{
            if (_instance != null)
			{
                return _instance;
			}
			else
			{
                //_instance = FindObjectOfType<SMFactorySO>();

                _instance = Resources.Load<SMFactorySO>("SMFactorySO");

                Debug.Log(_instance._states.Count + " States in list");
			}

            if (_instance == null)
			{
                Debug.Log("Failed to get Instance");
                return null;
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
                    ReloadStates();
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


    public static void ReloadStates()
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


    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptReload()
	{
        Debug.Log("Reloading Scripts");

        if (Instance.GetStatesOnReload)
		{
            ReloadStates();
		}
	}
}

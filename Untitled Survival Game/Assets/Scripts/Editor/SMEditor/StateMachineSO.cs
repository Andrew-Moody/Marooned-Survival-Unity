using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/StateMachineSO")]
public class StateMachineSO : ScriptableObject
{
    [SerializeField]
    private FSMList<EditorState> _statesOnSO;
  
}

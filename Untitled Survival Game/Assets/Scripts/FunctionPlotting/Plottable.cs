using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlottable
{
	public float GetValue();
}


public abstract class Plottable : MonoBehaviour, IPlottable
{
	public abstract float GetValue();
}
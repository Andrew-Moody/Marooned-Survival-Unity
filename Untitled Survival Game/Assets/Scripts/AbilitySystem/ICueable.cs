using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICueable
{
	
}


public interface IAnimCueable : ICueable
{
	public void SetTrigger(string trigger);
}
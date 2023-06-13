using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class NavMeshGenerator : MonoBehaviour
{
	private NavMeshSurface _surface;

	private NavMeshData _navData;

	private List<NavMeshBuildSource> _sources = new List<NavMeshBuildSource>();

	private NavMeshModifierVolume _volume;


	private void Awake()
	{
		_surface = GetComponent<NavMeshSurface>();
	}


	//public void BuildNavMesh()
	//{
	//	List<NavMeshModifier> modifiers;

	//	if (_surface.collectObjects == CollectObjects.Children)
	//	{
	//		modifiers = new List<NavMeshModifier>(_surface.GetComponentsInChildren<NavMeshModifier>());
	//	}
	//	else if(_surface.collectObjects == CollectObjects.Volume)
	//	{
	//		if (!_surface.TryGetComponent(out _volume))
	//		{
	//			Debug.LogError("CollectObjects is set to Volume but no volume modifier was found");
	//		}

	//		// not yet sure how to find modifiers within volume
	//		// get all then check if in volume before adding to markups?
	//		modifiers = NavMeshModifier.activeModifiers;
	//	}
	//	else
	//	{
	//		modifiers = NavMeshModifier.activeModifiers;
	//	}


	//	List<NavMeshBuildMarkup> markups = new List<NavMeshBuildMarkup>();

	//	for(int i = 0; i < modifiers.Count; i++)
	//	{
			
	//		NavMeshModifier modifier = modifiers[i];

	//		bool validLayer = (_surface.layerMask & (1 << modifier.gameObject.layer)) != 0;

	//		bool validAgentType = modifier.AffectsAgentType(_surface.agentTypeID);

			
	//		bool validVolume = true;

	//		if (_surface.collectObjects == CollectObjects.Volume)
	//		{
	//			// Do need to check if the modifier is inside volume but for now inlcude all
	//		}


	//		if (validLayer && validAgentType && validVolume)
	//		{
	//			NavMeshBuildMarkup markup = new NavMeshBuildMarkup()
	//			{
	//				root = modifier.transform,
	//				overrideArea = modifier.overrideArea,
	//				area = modifier.area,
	//				ignoreFromBuild = modifier.ignoreFromBuild
	//			};

	//			markups.Add(markup);
	//		}
	//	}

	//	//Debug.LogError($"Found {markups.Count} Markups");

	//	NavMeshBuilder.CollectSources(_surface.transform, _surface.layerMask, _surface.useGeometry, _surface.defaultArea, markups, _sources);

	//	Debug.LogError($"Collected {_sources.Count} Sources on {gameObject.name}");

	//}


	public void BuildNavMesh()
	{
		NavMeshSurface nav = GetComponent<NavMeshSurface>();

		if (nav != null)
		{
			Debug.LogError("BuildNavMesh");
			List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();

			List<NavMeshBuildMarkup> markups = new List<NavMeshBuildMarkup>();

			NavMeshBuilder.CollectSources(nav.transform, nav.layerMask, nav.useGeometry, nav.defaultArea, markups, sources);

			//Debug.LogError($"Found {sources.Count} Sources on {gameObject.name}");

			Vector3 pos = transform.position;
			Quaternion rot = transform.rotation;

			Bounds localBounds = new Bounds(nav.center, nav.size);

			NavMeshBuildSettings settings = nav.GetBuildSettings();


			//NavMeshData data = NavMeshBuilder.BuildNavMeshData(settings, sources, localBounds, pos, rot);
			//NavMesh.AddNavMeshData(data);

			//NavMeshData navData = new NavMeshData();
			//NavMesh.AddNavMeshData(navData);

			//localBounds = new Bounds(nav.center + transform.position, nav.size);
			//NavMeshBuilder.UpdateNavMeshData(navData, settings, sources, localBounds);

			//Debug.LogError(navData.position);


			// Build using the NavMeshSurface Component
			nav.BuildNavMesh();
		}
	}
}

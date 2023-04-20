using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : NetworkBehaviour
{
    [SerializeField]
    private bool _isSkinnedMesh;

    private IRenderable _renderable;


    public void Initialize(SkinnedMeshRenderer parentRig)
	{
        if (_isSkinnedMesh)
		{
			_renderable = new SkinnedMeshRenderable(parentRig, gameObject);
		}
		else
		{
			_renderable = new MeshFilterRenderable(gameObject);
		}

        
	}


	[ObserversRpc(BufferLast = true)]
    public void ObserversEquipItem(ItemNetData itemNetData)
	{
		ItemSO itemSO = ItemManager.Instance.GetItemSO(itemNetData.ItemID);


		_renderable.SetMesh(itemSO.Mesh);
		_renderable.SetMaterial(itemSO.Material);
	}


	[ObserversRpc(BufferLast = true)]
	public void ObserversClearSlot()
	{
		_renderable.SetMesh(null);
	}
}

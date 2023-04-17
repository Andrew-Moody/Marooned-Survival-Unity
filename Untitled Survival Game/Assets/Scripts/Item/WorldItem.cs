using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : NetworkBehaviour
{
    public ItemNetData ItemNetData;

    public string ItemName;

    private MeshFilter _meshFilter;

    private MeshRenderer _meshRenderer;


    // Start is called before the first frame update
    void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    
    public void SetItem(ItemNetData itemNetData)
	{
        ItemNetData = itemNetData;

        ItemSO itemSO = ItemManager.Instance.GetItemSO(itemNetData.ItemID);

        ItemName = itemSO.ItemName;
        _meshFilter.mesh = itemSO.Mesh;
        _meshRenderer.material = itemSO.Material;
    }


    public ItemNetData GetItemData()
	{
        return ItemNetData;
	}


    [ObserversRpc(BufferLast = true)]
    public void ObserversSetupWorldItem(ItemNetData itemNetData)
	{
        SetItem(itemNetData);
	}

}

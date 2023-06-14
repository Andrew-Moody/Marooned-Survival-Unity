using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDispenser : Interactable
{
	[SerializeField]
	private int _itemID;

	[SerializeField]
	private int _quantity;

	[SerializeField]
	private int _amountToStock;

	[SerializeField]
	private float _restockTime;

	[SerializeField]
	private bool _showOutline;

	[SerializeField]
	private bool _showItem;

	[SerializeField]
	private Transform _spawnLocation;


	private int _amountRemaining;

	private float _timeTillSpawn;

	private Material _outlineMat;
	private Material _itemMat;


	private void Awake()
	{
		enabled = false;
	}


	public override void OnStartClient()
	{
		base.OnStartClient();

		ItemSO itemSO = ItemManager.Instance.GetItemSO(_itemID);

		_outlineMat = _spawnLocation.gameObject.GetComponent<MeshRenderer>().material;


		_interactPrompt = $"Spawn {itemSO.ItemName} x{_quantity}";

		_itemMat = itemSO.Material;
		_spawnLocation.gameObject.GetComponent<MeshFilter>().mesh = itemSO.Mesh;
		
		UpdateDisplay(_itemID, _amountRemaining, _showOutline, _showItem);

		enabled = true;
	}

	public override void Interact(NetworkConnection user)
	{
		base.Interact(user);

		if (_amountRemaining > 0)
		{
			ItemNetData item = new ItemNetData(_itemID, _quantity);

			ItemManager.Instance.SpawnWorldItem(item, _spawnLocation.position);

			_amountRemaining--;

			enabled = true;

			UpdateDisplayORPC(_itemID, _amountRemaining, _showOutline, _showItem);
		}
	}


	[Server]
	public void SetItem(int itemID)
	{
		_itemID = itemID;

		UpdateDisplayORPC(_itemID, _amountRemaining, _showOutline, _showItem);
	}


	[Server]
	public void SetAmountToStock(int amount)
	{
		_amountToStock = amount;

		enabled = _amountRemaining < _amountToStock;
	}


	[Server]
	public void SetAmountRemaining(int amount)
	{
		_amountRemaining = amount;

		enabled = _amountRemaining < _amountToStock;

		UpdateDisplayORPC(_itemID, _amountRemaining, _showOutline, _showItem);
	}


	[ObserversRpc(RunLocally = true, BufferLast = true)]
	private void UpdateDisplayORPC(int itemID, int amount, bool showOutline, bool showItem)
	{
		UpdateDisplay(itemID, amount, showOutline, showItem);
	}


	private void UpdateDisplay(int itemID, int amount, bool showOutline, bool showItem)
	{
		if (itemID != _itemID)
		{
			ItemSO itemSO = ItemManager.Instance.GetItemSO(itemID);

			_spawnLocation.gameObject.GetComponent<MeshFilter>().mesh = itemSO.Mesh;

			_itemMat = itemSO.Material;
		}

		_amountRemaining = amount;

		if (showItem && _amountRemaining != 0)
		{
			_spawnLocation.gameObject.GetComponent<MeshRenderer>().material = _itemMat;
			_spawnLocation.gameObject.SetActive(true);
		}
		else if (showOutline && _amountRemaining == 0)
		{
			_spawnLocation.gameObject.GetComponent<MeshRenderer>().material = _outlineMat;
			_spawnLocation.gameObject.SetActive(true);
		}
		else
		{
			_spawnLocation.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		if (_amountRemaining >= _amountToStock)
		{
			enabled = false;
		}
		else if (_timeTillSpawn <= 0)
		{
			_timeTillSpawn = _restockTime;

			_amountRemaining++;
			UpdateDisplayORPC(_itemID, _amountRemaining, _showOutline, _showItem);
		}
		else
		{
			_timeTillSpawn -= Time.deltaTime;
		}
	}
}

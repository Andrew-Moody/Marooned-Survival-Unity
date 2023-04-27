using FishNet.Managing;
using FishNet.Transporting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddressInput : MonoBehaviour
{
    [SerializeField]
    private NetworkManager _networkManager;

    public void OnInput(string input)
	{
        _networkManager.TransportManager.Transport.SetClientAddress(input);
	}
}

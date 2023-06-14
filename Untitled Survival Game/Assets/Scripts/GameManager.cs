using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	private int _seed;
	public int Seed { get => _seed; set => _seed = value; }

	[SerializeField]
	private NetworkObject _playerPrefab;

	[SerializeField]
	private Camera _loadCamera;

	private NetworkManager _networkManager;


	private void Awake()
	{
		Debug.LogError("GameManager Awake");

		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
		_networkManager = InstanceFinder.NetworkManager;
	}


	private void OnEnable()
	{
		if (_networkManager != null)
		{
			_networkManager.SceneManager.OnClientLoadedStartScenes += OnClientLoadedStartScenes;

			_networkManager.SceneManager.OnQueueStart += OnQueueStart;

			//_networkManager.SceneManager.OnQueueEnd += OnQueueEnd;

			//_networkManager.SceneManager.OnUnloadStart += OnUnloadStart;

			//_networkManager.SceneManager.OnUnloadEnd += OnUnloadEnd;

			_networkManager.SceneManager.OnLoadStart += OnLoadStart;

			_networkManager.SceneManager.OnLoadEnd += OnLoadEnd;

			_networkManager.SceneManager.OnClientPresenceChangeEnd += OnClientPresenceChangeEnd;

			_networkManager.ClientManager.RegisterBroadcast<GameManagerBroadcast>(OnGameManagerBroadcast);

			_networkManager.ClientManager.OnRemoteConnectionState += OnRemoteConnectionState;

			_networkManager.ClientManager.OnClientConnectionState += OnClientConnectionState;
		}

		Debug.LogError("GameManager OnEnable");
	}


	private void OnDisable()
	{
		if (_networkManager != null)
		{
			_networkManager.SceneManager.OnClientLoadedStartScenes -= OnClientLoadedStartScenes;

			_networkManager.SceneManager.OnQueueStart -= OnQueueStart;

			//_networkManager.SceneManager.OnQueueEnd -= OnQueueEnd;

			//_networkManager.SceneManager.OnUnloadStart -= OnUnloadStart;

			//_networkManager.SceneManager.OnUnloadEnd -= OnUnloadEnd;

			_networkManager.SceneManager.OnLoadStart -= OnLoadStart;

			_networkManager.SceneManager.OnLoadEnd -= OnLoadEnd;

			_networkManager.SceneManager.OnClientPresenceChangeEnd -= OnClientPresenceChangeEnd;

			_networkManager.ClientManager.UnregisterBroadcast<GameManagerBroadcast>(OnGameManagerBroadcast);

			_networkManager.ClientManager.OnRemoteConnectionState -= OnRemoteConnectionState;

			_networkManager.ClientManager.OnClientConnectionState -= OnClientConnectionState;
		}

		Debug.LogError("GameManager OnDisable");
	}


	void Start()
	{
		Initialize();

		Debug.LogError("GameManager Start");
	}


	public void OnHostPressed()
	{
		Debug.LogError("Host button clicked");

		if (_networkManager != null)
		{
			_networkManager.ServerManager.StartConnection();
			_networkManager.ClientManager.StartConnection();

			UIManager.HideStackTop(true);

			UIManager.ShowPanel("HostOptionsUI", pushToStack: true);
		}
	}


	public void OnJoinPressed()
	{
		Debug.LogError("Join button clicked");

		if (_networkManager != null)
		{
			_networkManager.ClientManager.StartConnection();

			UIManager.HideStackTop(true);

			UIManager.ShowPanel("JoinOptionsUI", pushToStack: true);
		}
	}


	public void OnStartGamePressed()
	{
		if (_networkManager != null)
		{
			// Unlike before, using an RPC(Runlocally) on another object, the broadcast was recieved on the server after the
			// The scene load callbacks were recieved causing the local client to be unprepared for scene load

			PrepareSceneLoad();

			// Tell clients to prepare for scene load
			GameManagerBroadcast msg = new GameManagerBroadcast() { Message = "PrepareSceneLoad", Seed = _seed };
			_networkManager.ServerManager.Broadcast(msg);


			SceneLoadData sld = new SceneLoadData("SampleScene");
			sld.ReplaceScenes = ReplaceOption.All;

			_networkManager.SceneManager.LoadGlobalScenes(sld);
		}
	}


	public void OnMainMenuPressed()
	{
		if (_networkManager != null)
		{
			if (InstanceFinder.IsClient)
			{
				_networkManager.ClientManager.StopConnection();
			}

			if (InstanceFinder.IsServer)
			{
				_networkManager.ServerManager.StopConnection(true);
			}
		}
	}


	public void PrepareSceneLoad()
	{
		UIManager.HideStackTop(true);
		UIManager.ShowPanel("LoadingUI");

		_loadCamera.gameObject.SetActive(true);
	}


	public void OnLocalPlayerStartClient(GameObject player)
	{
		_loadCamera.gameObject.SetActive(false);

		UIManager.SetPlayer(player);

		CameraController.Instance.SetPlayer(player);

		CameraController.Instance.SetFPSMode(true);

		UIManager.HidePanel("LoadingUI");

		UIManager.ShowPanel("HotbarUI", pushToStack: true);

		if (player.TryGetComponent(out Combatant combatant))
		{
			combatant.OnDeathStartEvent += PlayerDeathHandler;
		}
	}


	// This is only called when the connection is first made and the initial scene is loaded for the server and/or client
	// Typically would spawn the player here but currently waiting till after switch from lobby scene to level scene
	private void OnClientLoadedStartScenes(NetworkConnection connection, bool asServer)
	{
		Debug.LogError("OnClientLoadedStartScenes asServer: " + asServer);
	}


	private void OnQueueStart()
	{
		Debug.LogError($"OnQueueStart");
	}


	private void OnQueueEnd()
	{
		Debug.LogError($"OnQueueEnd");
	}


	private void OnUnloadStart(SceneUnloadStartEventArgs args)
	{
		string sceneName = args.QueueData.SceneUnloadData.SceneLookupDatas[0].Name;

		Debug.LogError($"OnUnloadStart for Scene: {sceneName} asServer: {args.QueueData.AsServer}");
	}


	private void OnUnloadEnd(SceneUnloadEndEventArgs args)
	{
		string sceneName = args.QueueData.SceneUnloadData.SceneLookupDatas[0].Name;

		Debug.LogError($"OnUnloadEnd for Scene: {sceneName} asServer: {args.QueueData.AsServer}");
	}


	// Called when the new scene has started loading
	private void OnLoadStart(SceneLoadStartEventArgs args)
	{
		//string sceneName = args.QueueData.SceneLoadData.GetFirstLookupScene().name;

		string sceneName = args.QueueData.SceneLoadData.SceneLookupDatas[0].Name;

		int length = args.QueueData.SceneLoadData.SceneLookupDatas.Length;

		Debug.LogError($"OnLoadStart for Scene: {sceneName} asServer: {args.QueueData.AsServer} dataLength: {length}");
	}


	// Called when the new scene has finished loading
	private void OnLoadEnd(SceneLoadEndEventArgs args)
	{
		string sceneName = args.QueueData.SceneLoadData.GetFirstLookupScene().name;

		Debug.LogError($"OnLoadEnd for Scene: {sceneName} asServer: {args.QueueData.AsServer}");

		if (!args.QueueData.AsServer)
		{
			WorldGenManager.Instance.GenerateWorld(_seed, InstanceFinder.IsServer);
		}

		// In testing it seemed clients didnt recieve RPCs sent from the server in this callback
	}


	private void OnClientPresenceChangeEnd(ClientPresenceChangeEventArgs args)
	{
		// This may be the earliest the server can know for sure a client has loaded the new scene
		// and the observer status is current. Sending RPCs before this may not be recieved by the client

		Debug.LogError($"ClientId: {args.Connection.ClientId} {(args.Added ? "added to" : "removed from")} scene: {args.Scene.name}");

		if (args.Added && args.Scene.name == "SampleScene")
		{
			SpawnPlayerPrefab(args.Connection);
		}
	}


	private void Initialize()
	{
		UIManager.HideAll();

		UIManager.ShowPanel("MainUI", pushToStack: true);
	}


	private void SpawnPlayerPrefab(NetworkConnection connection)
	{
		if (_playerPrefab == null)
		{
			return;
		}

		NetworkObject nob = Instantiate(_playerPrefab);

		_networkManager.ServerManager.Spawn(nob, connection);
	}


	private void OnGameManagerBroadcast(GameManagerBroadcast msg)
	{
		Debug.LogError(msg.Message);

		if (InstanceFinder.IsClientOnly)
		{
			_seed = msg.Seed;

			PrepareSceneLoad();
		}
	}


	private void PlayerDeathHandler()
	{
		CameraController.Instance.SetFPSMode(false);

		UIManager.ShowPanel("DeathUI", pushToStack: true);
	}


	private void OnRemoteConnectionState(RemoteConnectionStateArgs args)
	{
		//Debug.LogError($"Remote Connection State Changed: {args.ConnectionState}");
	}


	private void OnClientConnectionState(ClientConnectionStateArgs args)
	{
		//Debug.LogError($"Client Connection State Changed: {args.ConnectionState}");

		if (args.ConnectionState == LocalConnectionState.Stopping)
		{
			OnClientDisconnect(args);
		}
	}


	private void OnClientDisconnect(ClientConnectionStateArgs args)
	{
		// Reset GameManager to initial state since it is not destroyed on load
		Initialize();

		UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
	}
}


public struct GameManagerBroadcast : IBroadcast
{
	public string Message;
	public int Seed;
}


public enum BroadcastMethod
{
	None,
	PrepareForSceneLoad,
}
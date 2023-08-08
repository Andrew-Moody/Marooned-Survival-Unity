using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Actors;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	[SerializeField]
	private int _seed;
	public int Seed { get => _seed; set => _seed = value; }

	public int PlayerCount => _players.Count;

	[SerializeField]
	private NetworkObject _playerPrefab;

	[SerializeField]
	private Camera _loadCamera;

	private NetworkManager _networkManager;

	private List<PlayerActor> _players = new List<PlayerActor>();
	private Dictionary<NetworkConnection, PlayerActor> _playersByConnection = new Dictionary<NetworkConnection, PlayerActor>();

	private const string MENU_SCENE = "MainMenuScene";
	private const string GAME_SCENE = "GameScene";

	private const string MAINMENU = "MainUI";
	private const string MOUSE = "MouseUI";
	private const string HOST_OPTIONS = "HostOptionsUI";
	private const string JOIN_OPTIONS = "JoinOptionsUI";
	private const string LOBBY = "LobbyUI";
	private const string LOADING = "LoadingUI";
	private const string HEADSUP = "HeadsupUI";
	private const string DEATH = "DeathUI";
	private const string DEBUG = "DebugUI";

	private bool _startFromGameScene;

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

			// Called on clients when another client connects but only when shareIDs is enabled
			//_networkManager.ClientManager.OnRemoteConnectionState += ClientManager_OnRemoteConnectionState;

			// Called on server when client state changes
			_networkManager.ServerManager.OnRemoteConnectionState += ServerManager_OnRemoteConnectionState;

			_networkManager.ServerManager.OnServerConnectionState += OnServerConnectionState;

			// Called on client when client state changes
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

			//_networkManager.ClientManager.OnRemoteConnectionState -= ClientManager_OnRemoteConnectionState;

			_networkManager.ServerManager.OnRemoteConnectionState -= ServerManager_OnRemoteConnectionState;

			_networkManager.ServerManager.OnServerConnectionState -= OnServerConnectionState;

			_networkManager.ClientManager.OnClientConnectionState -= OnClientConnectionState;
		}

		Debug.LogError("GameManager OnDisable");
	}


	void Start()
	{
		Initialize(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

		Debug.LogError("GameManager Start");
	}


	public PlayerActor GetPlayer(int index)
	{
		return index < _players.Count ? _players[index] : null;
	}


	public PlayerActor GetPlayer(NetworkConnection connection)
	{
		_playersByConnection.TryGetValue(connection, out PlayerActor player);
		return player;
	}


	public void OnHostPressed()
	{
		Debug.LogError("Host button clicked");

		if (_networkManager != null)
		{
			_networkManager.ServerManager.StartConnection();
			_networkManager.ClientManager.StartConnection();

			UIManager.HideStackTop(true);

			UIManager.ShowPanel(HOST_OPTIONS, pushToStack: true);
		}
	}


	public void OnJoinPressed()
	{
		UIManager.HideStackTop(true);

		UIManager.ShowPanel(JOIN_OPTIONS, pushToStack: true);
		
	}


	public void OnJoinServerPressed(string address)
	{
		UIManager.HideStackTop(true);

		UIManager.ShowPanel(LOBBY, pushToStack: true);

		if (_networkManager != null)
		{
			_networkManager.TransportManager.Transport.SetClientAddress(address);

			_networkManager.ClientManager.StartConnection();
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


			SceneLoadData sld = new SceneLoadData(GAME_SCENE);
			sld.ReplaceScenes = ReplaceOption.All;

			_networkManager.SceneManager.LoadGlobalScenes(sld);
		}
	}

	public void OnDebugStartGamePressed()
	{
		_networkManager.ServerManager.StartConnection();
		_networkManager.ClientManager.StartConnection();

		_startFromGameScene = true;
	}


	public void OnMainMenuPressed()
	{
		if (_networkManager != null)
		{
			if (InstanceFinder.IsOffline)
			{
				UIManager.HideAll();
				UIManager.ShowPanel(MOUSE);
				UIManager.ShowPanel(MAINMENU, pushToStack: true);
				return;
			}

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
		UIManager.ShowPanel(LOADING);

		_loadCamera.gameObject.SetActive(true);
	}


	public void OnLocalPlayerStartClient(Actor player)
	{
		_loadCamera.gameObject.SetActive(false);

		UIManager.SetPlayer(player);

		CameraController.Instance.SetPlayer(player);

		UIManager.HideAll();
		UIManager.ShowPanel(MOUSE);
		UIManager.ShowPanel(HEADSUP, pushToStack: true);

		PlayerInput.SetFPSMode(true);

		player.DeathStarted += PlayerDeathStarted;
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

		if (args.Added && args.Scene.name == GAME_SCENE)
		{
			SpawnPlayerPrefab(args.Connection);
		}
	}


	private void Initialize(string scene)
	{
		if (UIManager.Instance == null)
		{
			return;
		}

		if (scene == MENU_SCENE)
		{
			UIManager.HideAll();

			UIManager.ShowPanel(MAINMENU, pushToStack: true);
		}
		else if (scene == GAME_SCENE)
		{
			UIManager.HideAll();

			UIManager.ShowPanel(DEBUG, pushToStack: true);
		}

		UIManager.ShowPanel(MOUSE);
	}


	private void SpawnPlayerPrefab(NetworkConnection connection)
	{
		if (_playerPrefab == null)
		{
			return;
		}

		NetworkObject nob = Instantiate(_playerPrefab);

		_networkManager.ServerManager.Spawn(nob, connection);

		PlayerActor player = nob.GetComponentInChildren<PlayerActor>();

		if (player == null)
		{
			Debug.LogError("PlayerPrefab not setup correctly: missing PlayerActor");
		}

		_players.Add(player);
		_playersByConnection.Add(connection, player);
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


	private void PlayerDeathStarted(IActor playerActor, ActorEventData data)
	{
		PlayerInput.SetFPSMode(false);

		UIManager.ShowPanel(DEATH, pushToStack: true);
	}


	private void OnServerConnectionState(ServerConnectionStateArgs args)
	{
		if (args.ConnectionState == LocalConnectionState.Started)
		{
			if (_startFromGameScene)
			{
				Debug.LogError("DebugStartGame as Server: " + InstanceFinder.IsServer);
				WorldGenManager.Instance.GenerateWorld(_seed, InstanceFinder.IsServer);
			}
		}
	}


	private void ServerManager_OnRemoteConnectionState(NetworkConnection connection, RemoteConnectionStateArgs args)
	{
		Debug.LogError($"Remote Connection State Changed: {args.ConnectionState}");

		if (_playersByConnection.TryGetValue(connection, out PlayerActor player))
		{
			_players.Remove(player);
			_playersByConnection.Remove(connection);
		}
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
		UnityEngine.SceneManagement.SceneManager.LoadScene(MENU_SCENE);

		// Reset GameManager to initial state since it is not destroyed on load
		Initialize(MENU_SCENE);
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
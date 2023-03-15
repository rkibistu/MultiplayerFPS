using Fusion;
using Managers;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// Entitate unica la nivel de aplicatie.
/// Este create de GameLauncher in momentul in care se conecteaza un player in modul HOST. (OnPlayerJoined method)
///             Stim sigur ca va exista un singur player cu acest mod.
/// 
/// 
/// Retine preferinte lobby-ului: harta, game mode, lobby name
/// Ofera posibilitati de a spawna caracterele jucatorilor si obiectul de tip Gameplay (tinand cont de preferintele lobbyului)
/// </summary>
public class GameManager : NetworkBehaviour
{

    // PUBLIC MEMBERS
    public static GameManager Instance { get; private set; }

    public static event Action<GameManager> OnLobbyDetailsUpdated;
    public GameType GameType => ResourceManager.Instance.gameTypes[GameTypeId];
    public string MapName => ResourceManager.Instance.mapDefinitions[MapId].mapName;
    public string ModeName => ResourceManager.Instance.gameTypes[GameTypeId].ModeName;
    [Networked(OnChanged = nameof(OnLobbyDetailsChangedCallback))] public NetworkString<_32> LobbyName { get; set; }
    [Networked(OnChanged = nameof(OnLobbyDetailsChangedCallback))] public int MapId { get; set; }
    [Networked(OnChanged = nameof(OnLobbyDetailsChangedCallback))] public int GameTypeId { get; set; }
    [Networked(OnChanged = nameof(OnLobbyDetailsChangedCallback))] public int MaxUsers { get; set; }

    private static void OnLobbyDetailsChangedCallback(Changed<GameManager> changed) {
        OnLobbyDetailsUpdated?.Invoke(changed.Behaviour);
    }


    // PRIVATE MEMBERS
    private bool _initialized = false;
    public bool IsReady => (Runner.SceneManager.IsReady(Runner) && _initialized);

    
    // MONOBEHAVIOUR INTERFACE
    private void Awake() {
        if (Instance) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    
    // NETWORKBEHAVIOUR INTERFACE
    public override void Spawned() {
        base.Spawned();

        if (Object.HasStateAuthority) {
            LobbyName = ServerInfo.LobbyName;
            MapId = ServerInfo.MapId;
            GameTypeId = ServerInfo.GameMode;
            MaxUsers = ServerInfo.MaxUsers;
        }

        _initialized = true;
    }


    // PUBLIC METHODS

    //Spawn gameplay. Called by GamesceneLoader when scene is ready to be played

    public void SpawnGameplayObjects() {

        if (Context.Instance.Runner.IsServer) {
            //Host-ul spawneaza gameplay obejct + playerAgents

            SpawnGameplay();
            SpawnPlayersAgents();
            AddPlayersToActiveGameplay();
        }
        //toti playeri blocheaza cursorul -> focus pe joc
        RoomPlayer.LocalRoomPlayer.Input.LockCursour();
    }

    public void DespawnGameplayObjects() {

        Debug.Log("Despawn Gameplay Objects");

        RemovePlayersFromActiveGameplay();
        Context.Instance.Gameplay.Timer._onTiemrExpired = null;
        DespawnGameplay();
        DespawnPlayersAgents();
    }


    // RoomPlayer.ActiveAgent se seteaza corect la inceputul unei runde
    // Dar ActiveAgent.Owner nu face asta. Avem nevoie sa apelam aceasta functie pentru a realiza asocierea.
    //      Functie apelata inainte sa se execute operatii folosindu-ne de Owner-ul unui agent, in cazul in care acesta e null
    public void SetOwnersForAllAgents() {

        foreach (var player in RoomPlayer.Players) {

            if (player.ActiveAgent == null) {

                Debug.LogError("Active agent is null. Ai presupus ca asta nu se intampla");
                return;
            }

            player.ActiveAgent.Owner = player;
        }
    }

    // PRIVATE METHODS
    private void SpawnGameplay() {

        if(Runner.IsServer && HasStateAuthority) {

            Debug.Log("Spawn gameplay");
            Gameplay gameplayPrefab = ResourceManager.Instance.gameTypes[this.GameTypeId].GameplayPrefab;
            Context.Instance.Runner.Spawn(gameplayPrefab);
        }
    }
    //Despawn Gameplay.
    private void DespawnGameplay(bool changeToLobby = true) {

        Debug.Log("Despawn gameplay");
        Debug.Log(Context.Instance.Runner == null);
        Debug.Log(Context.Instance.Gameplay == null);
        Debug.Log(Context.Instance.Gameplay.GetComponent<NetworkObject>() == null);

        Context.Instance.Runner.Despawn(Context.Instance.Gameplay.GetComponent<NetworkObject>());

        //LevelManager.LoadMenu();
    }

    // Spawn agents for every player. Called by GamesceneLoader when scene is ready to be played
    private void SpawnPlayersAgents() {

        Debug.Log("Spawn players agents" + RoomPlayer.Players.Count);
        foreach(var player in RoomPlayer.Players) {

            //player.SpawnAgent();
            SpawnPlayerAgent(player);
        }
    }
    // Despawn agents for every player. 
    private void DespawnPlayersAgents() {

        Debug.Log("Despawn players agents");
        foreach(var player in RoomPlayer.Players) {

            DespawnPlayerAgent(player);
        }
    }

    // Add players to active gameplay. Called by GamesceneLoader when scene is ready to be played
    private void AddPlayersToActiveGameplay() {

        Debug.Log("Add players to gameplay " + RoomPlayer.Players.Count);
        foreach (var player in RoomPlayer.Players) {

            AddPlayerToActiveGameplay(player);
        }
    }
    // Remove players from active gameplay.
    private void RemovePlayersFromActiveGameplay() {

        Debug.Log("Remove players from gameplay " + RoomPlayer.Players.Count);
        foreach (var player in RoomPlayer.Players) {

            RemovePlayerFromActiveGameplay(player);
        }
    }
   
    // Soawnewaza/despawnewaza agentul pt player + setari
    private void SpawnPlayerAgent(RoomPlayer player) {

        //despawneaza daca deja exista cumva un agent
        DespawnPlayerAgent(player);

        //spawneaza agent efectiv
        var agent = SpawnAgent(player.Object.InputAuthority, player.AgentPrefab) as AgentStateMachine;
        // coreleaza agentul spawnat cu RoomPlayer caruia ii apartine
        player.AssignAgent(agent);

        //Seteaza NetworkObject asociat
        Runner.SetPlayerObject(player.Object.InputAuthority, player.Object);
    }
    private void DespawnPlayerAgent(RoomPlayer player) {
        if (player.ActiveAgent == null)
            return;

        //despawneaza agentul efectiv
        DespawnAgent(player.ActiveAgent);

        //decoreleaza agentul de RoomPlayerul asociat
        player.ClearAgent();
    }

    // spawneaza/despawneaza efectiv prefab-ul pt agent (nu face nici o alta setare)
    private AgentStateMachine SpawnAgent(PlayerRef inputAuthority, AgentStateMachine agentPrefab) {

        //Transform spawnPoint = RandomSpawnPoint();
        //var agent = Runner.Spawn(agentPrefab, spawnPoint.position, spawnPoint.rotation, inputAuthority);
        var agent = Context.Instance.Runner.Spawn(agentPrefab, agentPrefab.transform.position, agentPrefab.transform.rotation, inputAuthority);
        return agent;
    }
    private void DespawnAgent(AgentStateMachine agent) {
        if (agent == null)
            return;

        Context.Instance.Runner.Despawn(agent.Object);
    }


    // adauga la lista de Playeri pe care o tine obiectul de tip Gameplay
    private void AddPlayerToActiveGameplay(RoomPlayer player) {

        if (Context.Instance.Gameplay != null) {

            Context.Instance.Gameplay.JoinGameplay(player);
        }
        else {
            Debug.LogWarning("Couldn't spawn Agent. Gameplay not spawned yet");
        }
    }
    // scoate din lista de Playeri pe care o tine obiectul de tip Gameplay
    private void RemovePlayerFromActiveGameplay(RoomPlayer player) {

        if (Context.Instance.Gameplay != null) {

            Context.Instance.Gameplay.LeaveGameplay(player);
        }
        else {
            Debug.LogWarning("Couldn't remove player from Gameplay. Gameplay not spawned");
        }
    }
}

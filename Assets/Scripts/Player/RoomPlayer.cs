using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Managers;
using UnityEngine;



/// <summary>
/// 
/// O reprezentare a fiecarui jcuator (nu caracterul efectiv din joc, doar metadata)
/// 
/// Este spawnat de GameLauncher cand se trece de la meniul de lobby la scena jocului efectiv
/// 
/// </summary>
public class RoomPlayer : NetworkBehaviour {
    public enum EGameState {
        Lobby,
        GameCutscene,
        GameReady,
        EndingRound
    }

    public static readonly List<RoomPlayer> Players = new List<RoomPlayer>();

    public static Action<RoomPlayer> PlayerJoined;
    public static Action<RoomPlayer> PlayerLeft;
    public static Action<RoomPlayer> PlayerChanged;

    public static RoomPlayer LocalRoomPlayer;
    public PlayerScore PlayerScore { get => _playerScore; private set { } }

    [Networked(OnChanged = nameof(OnStateChanged))] public NetworkBool IsReady { get; set; }
    [Networked(OnChanged = nameof(OnStateChanged))] public NetworkString<_32> Username { get; set; }
    [Networked] public EGameState GameState { get; set; }
    public bool IsLeader => Object != null && Object.IsValid && Object.HasStateAuthority;

    [Networked(OnChanged = nameof(OnActiveAgentChanged), OnChangedTargets = OnChangedTargets.All), HideInInspector]
    public AgentStateMachine ActiveAgent { get; private set; }
    public PlayerInput Input { get; private set; }
    public AgentStateMachine AgentPrefab => _agentPrefab;

    // PRIVATE MEMBERS

    [SerializeField]
    private AgentStateMachine _agentPrefab;
    private int _lastWeaponSlot;
    private PlayerScore _playerScore = new PlayerScore();


    // Network/monoBEHAVIOUR methods

    protected void Awake() {
        Input = GetComponent<PlayerInput>();
    }

    public override void Spawned() {
        base.Spawned();

        if (Object.HasInputAuthority) {
            LocalRoomPlayer = this;

            PlayerChanged?.Invoke(this);
            RPC_SetPlayerStats(ClientInfo.Username);
        }

        Players.Add(this);
        PlayerJoined?.Invoke(this);

        DontDestroyOnLoad(gameObject);
    }
    public override void Despawned(NetworkRunner runner, bool hasState) {
        if (hasState == false)
            return;

        Debug.Log("Despawned RoomPlayer");
    }

    //public override void FixedUpdateNetwork() {
    //    bool agentValid = ActiveAgent != null && ActiveAgent.Object != null;

    //    Input.InputBlocked = agentValid == false;

    //    if (agentValid == true && HasStateAuthority == true) {
    //        _lastWeaponSlot = ActiveAgent.Weapons.CurrentWeaponSlot;
    //    }
    //}

    // PUBLIC METHODS

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority, InvokeResim = true)]
    private void RPC_SetPlayerStats(NetworkString<_32> username) {
        Username = username;
    }


    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_ChangeReadyState(NetworkBool state) {
        Debug.Log($"Setting {Object.Name} ready state to {state}");
        IsReady = state;
    }

    public void SetAllPlayersReadyState(NetworkBool state) {

        if (!HasStateAuthority) {
            Debug.LogError("This should be called only byan obejct with State Authority");
            return;
        }


        foreach (var player in Players) {

            player.IsReady = state;
        }
    }

    public static void RemovePlayer(NetworkRunner runner, PlayerRef p) {
        var roomPlayer = Players.FirstOrDefault(x => x.Object.InputAuthority == p);
        if (roomPlayer != null) {

            Players.Remove(roomPlayer);
            runner.Despawn(roomPlayer.Object);
        }
    }

    public void AssignAgent(AgentStateMachine agent) {

        //Folosit in Gameplay cand se spawneaza agentul pentru a seta this.ActiveAgent

        agent.Owner = this;
        ActiveAgent = agent;
        ActiveAgent.Owner = this;

        if (HasStateAuthority == true && _lastWeaponSlot != 0) {
            agent.Weapons.SwitchWeapon(_lastWeaponSlot, true);
        }
    }

    public void ClearAgent() {
        if (ActiveAgent == null)
            return;

        ActiveAgent.Owner = null;
        ActiveAgent = null;
    }

    public static void OnActiveAgentChanged(Changed<RoomPlayer> changed) {
        if (changed.Behaviour.ActiveAgent != null) {
            changed.Behaviour.AssignAgent(changed.Behaviour.ActiveAgent);
        }
        else {
            changed.Behaviour.ClearAgent();
        }
    }


    // Apela de gameplay cand se termina runda (expire timer-ul rundei)
    // Deblocheaza cursor, opreste UI local, seteaza starea playerului local
    //          si apeleaza RPC pt server sa anunte ca acest player a finalizat procesul de terminare runda
    public void RoundEnd(ActivateUI activateUI) {

        if (!LocalRoomPlayer.Object.HasInputAuthority)
            return;

        LocalRoomPlayer.Input.UnlockCursour();
        activateUI.Dezactivate();
        LocalRoomPlayer.GameState = EGameState.EndingRound;

        // announce server that process of ending round si ready
        RoundEnded_RPC();
    }

    // Rpc apelat de toti playerii in momentul finalziarii rundei (timer runda expirat)
    // Verifica daca toti playerii au terminat
    //          Daca da, despawneaza obiectele de gameplay (gameplay, agents)
    //                   seteaza stare playeri
    //                   Load next scene -> menu between rounds
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RoundEnded_RPC(RpcInfo info = default) {


        //Vrem sa Despawnam obiectele de gameplay doar daca otti playerii au termiant procesul de finalizare de runda
        foreach (var player in Players) {
            
            if (player.GameState != EGameState.EndingRound) {
                
                if (player.Object.Id == info.Source) {
                    player.GameState = EGameState.EndingRound;
                }
                else {
                    return;
                }
            }
            
        }

        GameManager.Instance.DespawnGameplayObjects();


        // Seteaza starea jucatorilor to lobby deoarece acum ne vom muta in scena d elobby dintre runde
        foreach (var player in Players) {

            player.GameState = EGameState.Lobby;

        }

        LevelManager.LoadTrack(ResourceManager.Instance.AfterRoundMenuScene);
    }

    // PRIVATE METHODS
    private void OnDisable() {
        // OnDestroy does not get called for pooled objects
        PlayerLeft?.Invoke(this);
        Players.Remove(this);


       // ActiveAgent = null; 
    }

    private static void OnStateChanged(Changed<RoomPlayer> changed) => PlayerChanged?.Invoke(changed.Behaviour);

}
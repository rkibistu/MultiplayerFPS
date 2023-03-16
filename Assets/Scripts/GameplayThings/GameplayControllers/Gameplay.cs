using Fusion;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static RoomPlayer;

/// <summary>
/// 
/// Gameplay-ul efectiv. Este o clasa abstracta ce urmeaza sa fie implementata de fiecare tip diferit de gameplay (deathmath, teammatch, etc)
/// Ofera posibilitatea de seta ce sa se intamplee cand:
///     se conectwaza/deconecteaza un player; moare un player; se spawneaza/despawneaza caracterul unui player
/// 
/// Este spawnat de GamesceneLaoder la incarcarea hartii/scenei de lupta
/// Tine un dictionar cu toti playerii conectati -> <PlayerRef, RoomPlayer>
/// 
/// </summary>

[RequireComponent(typeof(GameTimer), typeof(ActivateUI))]
public abstract class Gameplay : NetworkBehaviour {

    // PUBLIC MEMBERS

    [Networked, HideInInspector, Capacity(200)]
    public NetworkDictionary<PlayerRef, RoomPlayer> Players { get; }

    [Tooltip("Durata rundei in secunde")]
    public int _roundDuration;
    public GameTimer Timer => _timer;

    public bool IsPlaying { get; private set; }

    // PRIVATE MEMBERS
    [SerializeField]
    [Tooltip("Time to show Scoreboard at the end of the round before going back to lobby screen")]
    private float _endScoreTableDuration = 3f;

    private SpawnPoint[] _spawnPoints;

    // PROTECTED MEMBERS

    protected GameTimer _timer;
    protected ActivateUI _activateUI;

    // PUBLIC METHODS

    // functie apelata cand un nou player se conecteza la gameplay (doar pe server)
    // adauga la dictionarul cu playeri + spawneaza agent(caracter) pentru player
    public void JoinGameplay(RoomPlayer player) {
        if (HasStateAuthority == false)
            return;

        Debug.Log($"Player {player.Username} joined gameplay");

        AddToPlayerList(player);
        SubscribeTohealthEvents(player);

        // can be override by childs if diffrent gameplays need diffrent actions
        OnPlayerJoin(player);
    }

    //functie apelata cand un player se deconecteaza de la gameplay (doar pe server)
    //  adica se revine la menu de lobby sau se deconecteaza din joc
    public void LeaveGameplay(RoomPlayer player) {
        if (HasStateAuthority == false)
            return;

        Debug.Log($"Player {player.Username} left gameplay");
        RemoveFromPlayerList(player);
        UnsubscribeFromHealthEvents(player);

        // can be override by childs if diffrent gameplays need diffrent actions
        OnPlayerLeft(player);
    }

    // This method should be called after end  of round animations to despawn gameplay objects and move to lobby screen
    //   This method despawn gameplay objects, you have to despawn everything that depends to them for all players
    //      Example: UI is going to have errors if you don't disable it on all players before calling this.
    //      We use end of round animation as a way to let time to all players to despawn UI and other objects that are dependent 
    public void EndRound() {

        //show scoreboard
        UIScreen.Focus(InterfaceManager.Instance.scoreScreen);

        // despawn gameplay objects and go to lobby after some delay
        StartCoroutine(GoBackToLobby_Coroutine());
    }

    // MONOBEHAVIOUR INTERFACE

    // Se apeleaza INAINTE de functiile de Render din Fusion.
    //             DUPA toate functiile de physiscs din Fusion (inclusiv OnLateFixedUpdate din Agent)
    private void Update() {
        if (HasStateAuthority == false)
            return;

        OnUpdate();
    }

    // Se apeleaza dupa Update
    private void LateUpdate() {
        if (HasStateAuthority == false)
            return;

        OnLateUpdate();
    }


    // NETWORK INTERFACE

    public override void Spawned() {
        // Register to context

        Debug.Log("Register Gameplay to context");
        Context.Instance.Gameplay = this;

        //move from DontDestroyOnLoad
        SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetActiveScene());

        //Find all spawn points
        _spawnPoints = GameObject.FindObjectsOfType<SpawnPoint>();
        

        //Get timer and clear OnExpiredTime Action subscriptions
        _timer = GetComponent<GameTimer>();
        _timer._onTiemrExpired = null;

        //activeaza UI specific gameplay
        _activateUI = GetComponent<ActivateUI>();
        _activateUI.Activate();

        // Reset Player Score
        foreach(var player in RoomPlayer.Players) {

            player.PlayerScore.ResetScore();
        }

        //Clear Cache from last round of the match
        // There are destroyed gameobjects that exist in cache when we spawn scenes. We don't want them
        Context.Instance.ObjectCache.ClearAll();

        //accept input
        IsPlaying = true;

        OnSpawned();
    }

    public override void Despawned(NetworkRunner runner, bool hasState) {
        base.Despawned(runner, hasState);

        Debug.Log("Gameplay despawned");
        Context.Instance.Gameplay = null;

        _activateUI.Dezactivate();
    }
    // Gameplay INTERFACE

    protected virtual void OnSpawned() {; }

    // spawn agent
    protected virtual void OnPlayerJoin(RoomPlayer player) {;}
    //despawn agent
    protected virtual void OnPlayerLeft(RoomPlayer player) {;}

    // setari pe agent dupa ce a fost spawnat (pozitia de start, vizibil/invisibil, arma, etc. )
    // acestea difera in functie de tipul de gameplay  (deathmatch, teams, etc.)
    protected virtual void OnPlayerAgentSpawned(AgentStateMachine agent) {; }
    protected virtual void OnPlayerAgentDespawned(AgentStateMachine agent) {; }

    // apelata in Update()
    protected virtual void OnUpdate() {; }

    // apelata in LateUpdate
    protected virtual void OnLateUpdate() {; }

    // functie apelata de fiecare data cand un jucator moare (FatalHitTaken event din Health)
    protected virtual void OnFatalHitTaken(HitData hitData) {
    }

    protected Transform RandomSpawnPoint() {

        int index = Random.Range(0, _spawnPoints.Length);
        return _spawnPoints[index].transform;
    }

    // This method is called by gameplay at the end of the match
    //      it is going to play win/lose animation from GameResultScreenUI
    //      acesta, dupa animatie, va apela EndRound din Gameplay -> despawn objects + go to lobby
    protected void PlayEndOfRoundAnimation() {

        // Seteaza starea jucatorilor to lobby deoarece acum ne vom muta in scena d elobby dintre runde
        foreach (var player in RoomPlayer.Players) {

            player.GameState = EGameState.EndingRound;

        }

        //set gameplay state
        IsPlaying = false;

        //dezactivate UI
        _activateUI.Dezactivate();

        //play win/lose animation
        UIScreen.Focus(InterfaceManager.Instance.resultScreen);

        //aici ne trebuie un if statement, sa stim pe care o bagam
        InterfaceManager.Instance.resultScreen.GetComponent<GameResultScreenUI>().PlayVictoryAnimation();
    }

    private void DespawnGameplayAndGoToLobby() {

        if (!HasStateAuthority)
            return;

        GameManager.Instance.DespawnGameplayObjects();

        // Seteaza starea jucatorilor to lobby deoarece acum ne vom muta in scena d elobby dintre runde
        foreach (var player in RoomPlayer.Players) {

            player.GameState = EGameState.Lobby;
        }

        LevelManager.LoadTrack(ResourceManager.Instance.AfterRoundMenuScene);
    }

    private IEnumerator GoBackToLobby_Coroutine() {

        yield return new WaitForSeconds(_endScoreTableDuration);
        DespawnGameplayAndGoToLobby();
    }

   

    // PRIVATE METHODS

    // adauga/sterge din lista de playeri conectat
    private void AddToPlayerList(RoomPlayer player) {

        var playerRef = player.Object.InputAuthority;

        if (Players.ContainsKey(playerRef) == true) {
            Debug.LogError($"Player {playerRef} already joined");
            return;
        }

        Players.Add(playerRef, player);
    }
    private void RemoveFromPlayerList(RoomPlayer player) {

        if (Players.ContainsKey(player.Object.InputAuthority) == false)
            return;

        Players.Remove(player.Object.InputAuthority);
    }



    // aceasta functie va abona jucatorul la evenimentul FatalHitTaken din clasa Health
    // astfel incat sa executam cod atunci cand un player moare
    private void SubscribeTohealthEvents(RoomPlayer player) {

        AgentStateMachine agent = player.ActiveAgent;
        if (agent == null) {

            Debug.LogError("Sigur se paote Player fara ActiveAgent? Atentie, poate nu apuca sa se initializeze(sa dea Join) la nivel de clasa Player");
            return;
        }

        agent.Health.FatalHitTaken += OnFatalHitTaken;
    }
    // dezabonam 
    private void UnsubscribeFromHealthEvents(RoomPlayer player) {

        AgentStateMachine agent = player.ActiveAgent;
        if (agent == null) {

            Debug.LogError("Sigur se paote Player fara ActiveAgent? Atentie, poate nu apuca sa se initializeze(sa dea Join) la nivel de clasa Player");
            return;
        }

        agent.Health.FatalHitTaken -= OnFatalHitTaken;
    }

 

}

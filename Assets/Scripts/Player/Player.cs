using Fusion;
using UnityEngine;


//[RequireComponent(typeof(PlayerInput))]
//public class RoomPlayer : NetworkBehaviour
//{
//    [Networked(OnChanged = nameof(OnActiveAgentChanged), OnChangedTargets = OnChangedTargets.All), HideInInspector]
//    public AgentStateMachine ActiveAgent { get; private set; }
//    public PlayerInput Input { get; private set; }
//    public AgentStateMachine AgentPrefab => _agentPrefab;
    

//    // PRIVATE MEMBERS

//    [SerializeField]
//    private AgentStateMachine _agentPrefab;


//    private int _lastWeaponSlot;

//    // PUBLIC METHODS

//    public void AssignAgent(AgentStateMachine agent) {
//        ActiveAgent = agent;
//        ActiveAgent.Owner = this;

//        if (HasStateAuthority == true && _lastWeaponSlot != 0) {
//            agent.Weapons.SwitchWeapon(_lastWeaponSlot, true);
//        }
//    }

//    public void ClearAgent() {
//        if (ActiveAgent == null)
//            return;

//        ActiveAgent.Owner = null;
//        ActiveAgent = null;
//    }

//    // NetworkBehaviour INTERFACE
//    public override void Spawned() {

//        if (Context.Instance.Gameplay != null) {

//            // adauga la lista de Playeri pe care o tine obiectul de tip Gameplay
//            // automat, creeaza si Agenntul pentru player si spawneaza-l in lume
//            Context.Instance.Gameplay.Join(this);

            
//        }
//    }
//    public override void FixedUpdateNetwork() {
//        bool agentValid = ActiveAgent != null && ActiveAgent.Object != null;

//        Input.InputBlocked = agentValid == false;

//        if (agentValid == true && HasStateAuthority == true) {
//            _lastWeaponSlot = ActiveAgent.Weapons.CurrentWeaponSlot;
//        }
//    }

//    public override void Despawned(NetworkRunner runner, bool hasState) {
//        if (hasState == false)
//            return;

//        Context.Instance.Gameplay.Leave(this);

//        if (Object.HasStateAuthority == true && ActiveAgent != null) {
//            Runner.Despawn(ActiveAgent.Object);
//        }

//        ActiveAgent = null;


//    }

//    // MONOBEHAVIOUR
//    protected void Awake() {
//        Input = GetComponent<PlayerInput>();
//    }

//    // NETWORK CALLBACKS

//    public static void OnActiveAgentChanged(Changed<RoomPlayer> changed) {
//        if (changed.Behaviour.ActiveAgent != null) {
//            changed.Behaviour.AssignAgent(changed.Behaviour.ActiveAgent);
//        }
//        else {
//            changed.Behaviour.ClearAgent();
//        }
//    }
//}

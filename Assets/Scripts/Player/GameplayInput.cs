using Fusion;
using UnityEngine;

// orice button pui aici, va fi automat luat in calcul mai jos in GamePlayeInput -> NetworkButtons
//              (asta face intern INetworkStruct)
// In PlayerInput -> OnInput, se si pupuleaza valorile de Input
public enum EInputButtons {
    Fire = 0,
    AltFire = 1,
    Jump = 2,
    Reload = 3,
    Test = 4,
    Sprint = 5,
    ZoomWeapon = 6,
}

public struct GameplayInput : INetworkInput {
    public int WeaponSlot => WeaponButton - 1;

    public Vector2 MoveDirection;
    public Vector2 LookRotationDelta;
    public byte WeaponButton;
    public NetworkButtons Buttons;

    //public bool Sprint { get { return Buttons.IsSet(EInputButtons.Sprint); } set { Buttons.Set(EInputButtons.Sprint, value); } }
}

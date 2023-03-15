using UnityEngine;

/// <summary>
/// ScriptableObject
/// 
/// Ar trebui sa exista un scriptableObject pentru fiecare tip de joc:
///             deathmatch, teammatch, etc.
/// 
/// Folosit: in Resources exista un vector de toate obiectele de genul acesta
/// </summary>

[CreateAssetMenu(fileName = "New Game Type", menuName = "Scriptable Object/Game Type")]
public class GameType : ScriptableObject {
    
    public string ModeName;
    public Gameplay GameplayPrefab;

    // aici poti adauga detalii despre mode
    //  exemplu: durata unei runde, nr de runde, durata totala, etc.
}

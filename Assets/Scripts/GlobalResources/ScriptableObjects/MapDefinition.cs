using UnityEngine;

/// <summary>
/// ScriptableObject
/// 
/// Ar trebui sa exista un scriptableObject pentru fiecare harta diferita:
/// 
/// Folosit: in Resources exista un vector de toate obiectele de genul acesta
/// </summary>

[CreateAssetMenu(fileName = "New Map", menuName = "Scriptable Object/Map Definition")]
public class MapDefinition : ScriptableObject
{
    public string mapName; 
    public Sprite mapIcon;
    public int buildIndex; // scene index

}

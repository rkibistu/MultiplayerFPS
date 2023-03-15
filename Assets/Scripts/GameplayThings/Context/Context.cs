using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// 
/// Atasat de un obiect ce exista pe toata durata aplicatie, UNIC.
/// 
/// Folosit pentru a oferii acces global pentru fiecare membru al sau. Membrii sunt unici 
///     (facem asta ca sa nu facem cate un singleton pentru fiecare membru)
/// De fiecare data cand un asftel de mebru este instantiat, acesta trebuie sa populeze aceasta clasa.
///     exemplu: cna dse instantiaza gameplay: Context.Instance.Gameplay = this  (facem asta in Gameplay)
/// </summary>
public class Context : MonoBehaviour
{
    // SINGLETON

    // AICI SCHIMBA -> pune si emmbru _instance, fa intiilaizare in get
    private static Context _instance;
    public static Context Instance {

        get {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<Context>();
            return _instance;
        }
        private set {; }
    }

  
    protected virtual void Awake() {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) {
            Destroy(this);
        }
        else {
            _instance = this;
        }
    }
    
    // PUBLIC MEMBERS

    //membrii de care vrobeam un summary
    public ObjectCache ObjectCache { get; set; }
    public SceneInput SceneInput { get; set; }
    public Gameplay Gameplay { get; set; }
    public NetworkRunner Runner { get; set; }
    public CameraContext CameraContext { get; set; }


}

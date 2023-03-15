using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Trebuie sa exista in fiecare scena de joc (unde se jaoca efetic , nu menu)
/// 
/// Spawneaza gameplay-ul si agentii(caracterele) jucatorilor in momentul incarcarii scenei
/// </summary>

public class GamesceneLoader : MonoBehaviour {


    private void Start() {

        StartCoroutine(SpawnObjects());
    }



  
    public IEnumerator SpawnObjects() {

        //asteapta pana cand runner-ul and Gamemanagerul sunt initalizate
        while(!GameManager.Instance.IsReady)
            yield return null;

        GameManager.Instance.SpawnGameplayObjects();
    }


}
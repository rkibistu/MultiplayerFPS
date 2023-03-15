using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroyOnLoad : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private bool startEnabled = true;
    private void Awake() {
        

        DontDestroyOnLoad(this);
        this.SetActive(startEnabled);
    }
}

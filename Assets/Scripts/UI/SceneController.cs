using System.Collections.Generic;
using UnityEngine;

// De aici practic controlam toate SceneServices
//              de aici se apeleaza functiile pentru SceneServices

public class SceneController : MonoBehaviour
{
    // PUBLIC MEMBERS

    public bool IsActive { get; private set; }

    //PRIVATE MEMBERS
    
    [SerializeField]
    private bool _selfInitialize;

    private bool _isInitialized;
    private List<SceneService> _services = new List<SceneService>();

  

    // PUBLIC METHODS

    //colecteaza serviciile de la obiectele copii si le initializeaza pe toate
    public void Initialize() {

        if (_isInitialized == true)
            return;

        CollectServices();

        OnInitialize();

        _isInitialized = true;
    }
    
    public void Deinitialize() {
        if (_isInitialized == false)
            return;

        Deactivate();

        OnDeinitialize();

        _isInitialized = false;
    }
    public void Activate() {

        if (_isInitialized == false)
            return;

        OnActivate();

        IsActive = true;
    }


    public void Deactivate() {
        if (IsActive == false)
            return;

        OnDeactivate();

        IsActive = false;
    }

    public void Quit() {
        Deinitialize();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
    }


    // MONOBEHAVIOUR METHODS

    // auto initializare daca _selfInitialize == true
    protected void Awake() {
        if (_selfInitialize == true) {
            Initialize();
        }
    }
    // auto activate daca _selfInitialize == true
    protected void Start() {
        if (_isInitialized == false)
            return;

        if (_selfInitialize == true && IsActive == false) {
            // UI cannot be initialized in Awake, Canvas elements need to Awake first

            Activate();
        }
    }

    
    protected virtual void Update() {
        if (IsActive == false)
            return;

        OnTick();
    }
    protected virtual void LateUpdate() {
        if (IsActive == false)
            return;

        OnLateTick();
    }
    protected void OnDestroy() {
        Deinitialize();
    }

    protected void OnApplicationQuit() {
        Deinitialize();
    }

    // PROTECTED METHODS

    protected virtual void CollectServices() {
        var services = GetComponentsInChildren<SceneService>(true);

        foreach (var service in services) {
            AddService(service);
        }
    }

    protected virtual void OnInitialize() {
        for (int i = 0; i < _services.Count; i++) {
            _services[i].Initialize(this);
        }
    }

    protected virtual void OnActivate() {
        for (int i = 0; i < _services.Count; i++) {
            _services[i].Activate();
        }
    }

    protected virtual void OnTick() {
        for (int i = 0, count = _services.Count; i < count; i++) {
            _services[i].Tick();
        }
    }
    protected virtual void OnLateTick() {
        for (int i = 0, count = _services.Count; i < count; i++) {
            _services[i].LateTick();
        }
    }

    protected virtual void OnDeactivate() {
        for (int i = 0; i < _services.Count; i++) {
            _services[i].Deactivate();
        }
    }

    protected virtual void OnDeinitialize() {
        for (int i = 0; i < _services.Count; i++) {
            _services[i].Deinitialize();
        }

        _services.Clear();
    }


    protected void AddService(SceneService service) {
        if (service == null) {
            Debug.LogError($"Missing service");
            return;
        }

        if (_services.Contains(service) == true) {
            Debug.LogError($"Service {service.gameObject.name} already added.");
            return;
        }

        _services.Add(service);

        if (_isInitialized == true) {
            service.Initialize(this);
        }

        if (IsActive == true) {
            service.Activate();
        }
    }
}

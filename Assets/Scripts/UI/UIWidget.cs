using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIWidget : UIBehaviour {

    // PUBLIC MEMBERS

    public bool IsVisible { get; private set; }

    // PROTECTED MEMBERS

    protected SceneUI SceneUI { get; private set; }
    protected bool IsInitalized { get; private set; }
    protected UIWidget Owner { get; private set; }

    // PRIVATE MEMBERS

    private List<UIWidget> _children = new List<UIWidget>(16);

    // PUBLIC METHODS

    public void PlayClickSound() {
        if (SceneUI != null) {
            SceneUI.PlayClickSound();
        }
    }

    public void PlaySound(AudioSetup sound, EForceBehaviour force = EForceBehaviour.None) {
        if (sound == null) {
            Debug.LogWarning($"Missing click sound, parent {name}");
            return;
        }

        if (SceneUI != null) {
            SceneUI.PlaySound(sound, force);
        }
    }

    // INTERNAL METHODS

    //aduna totate UIWidgets de la copii so initalizeaza-le
    internal void Initialize(SceneUI sceneUI, UIWidget owner) {
       

        if (IsInitalized == true)
            return;

        SceneUI = sceneUI;
        Owner = owner;

        _children.Clear();
        GetChildWidgets(transform, _children);

        for (int i = 0; i < _children.Count; i++) {
            _children[i].Initialize(sceneUI, this);
        }

        OnInitialize();

        IsInitalized = true;

        if (gameObject.activeInHierarchy == true) {
            Visible();
        }
    }

    //deinitalizeaza toate UIWidget-urile din lista (de la toti copiii)
    internal void Deinitialize() {
        if (IsInitalized == false)
            return;

        Hidden();

        OnDeinitialize();

        for (int i = 0; i < _children.Count; i++) {
            _children[i].Deinitialize();
        }

        _children.Clear();

        IsInitalized = false;

        SceneUI = null;
        Owner = null;
    }

    //apeleaza Visible pentru toti copii
    internal void Visible() {
        if (IsInitalized == false)
            return;

        if (IsVisible == true)
            return;

        if (gameObject.activeSelf == false)
            return;

        IsVisible = true;

        for (int i = 0; i < _children.Count; i++) {
            _children[i].Visible();
        }

        OnVisible();
    }

    //apeleaza Hidden pentru toti copii
    internal void Hidden() {
        if (IsVisible == false)
            return;

        IsVisible = false;

        OnHidden();

        for (int i = 0; i < _children.Count; i++) {
            _children[i].Hidden();
        }
    }

    //apeleaza tick pentru toi copii
    internal void Tick() {
        if (IsInitalized == false)
            return;

        if (IsVisible == false)
            return;

        OnTick();

        for (int i = 0; i < _children.Count; i++) {
            _children[i].Tick();
        }
    }

    internal void AddChild(UIWidget widget) {
        if (widget == null || widget == this)
            return;

        if (_children.Contains(widget) == true) {
            Debug.LogError($"Widget {widget.name} is already added as child of {name}");
            return;
        }

        _children.Add(widget);

        widget.Initialize(SceneUI, this);
    }

    internal void RemoveChild(UIWidget widget) {
        int childIndex = _children.IndexOf(widget);

        if (childIndex < 0) {
            Debug.LogError($"Widget {widget.name} is not child of {name} and cannot be removed");
            return;
        }

        widget.Deinitialize();

        _children.RemoveAt(childIndex);
    }

    // MONOBEHAVIOR

    protected override void OnEnable() {
        Visible();
    }

    protected override void OnDisable() {
        Hidden();
    }

#if UNITY_EDITOR
    protected override void OnDestroy() {
        if (IsInitalized == true) {
            string ownerName = Owner != null ? Owner.name : "None";
            Debug.LogError($"Widget {name} with owner {ownerName} was not correctly deinitialized");
        }
    }
#endif

    //UIWidget INTERFACE
    //functiile de mai jos sunt apelate in cele de mai sus
    //aceste sunt pentru a fi suprasrcise in copii

    public override bool IsActive() { return true; }

    protected virtual void OnInitialize() { }
    protected virtual void OnDeinitialize() { }
    protected virtual void OnVisible() { }
    protected virtual void OnHidden() { }
    protected virtual void OnTick() { }

    // PRIVATE MEMBERS

    private static void GetChildWidgets(Transform transform, List<UIWidget> widgets) {
        foreach (Transform child in transform) {
            var childWidget = child.GetComponent<UIWidget>();

            if (childWidget != null) {
                widgets.Add(childWidget);
            }
            else {
                // Continue searching deeper in hierarchy
                GetChildWidgets(child, widgets);
            }
        }
    }
}

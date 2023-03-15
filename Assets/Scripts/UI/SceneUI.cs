using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


// aduna toti UIView de la obiectele copil 
// le initilizeaza, activeaza si apeleaza functia Tick pentru ele

public class SceneUI : SceneService
{
    // PUBLIC MEMBERS

    public Canvas Canvas { get; private set; }
    public Camera Camera { get; private set; }

    // PRIVATE MEMBERS

    [SerializeField]
    private UIView[] _defaultViews;
    [SerializeField]
    private AudioEffect[] _audioEffects;
    [SerializeField]
    private AudioSetup _clickSound;

    // SceneUI INTERFACE

    protected UIView[] _views;

    protected virtual void OnInitializeInternal() { }
    protected virtual void OnDeinitializeInternal() { }
    protected virtual void OnTickInternal() { }
    protected virtual void OnViewOpened(UIView view) { }
    protected virtual void OnViewClosed(UIView view) { }

    // PUBLIC METHODS

    public T Get<T>() where T : UIView {
        if (_views == null)
            return null;

        for (int i = 0; i < _views.Length; ++i) {
            T view = _views[i] as T;

            if (view != null)
                return view;
        }

        return null;
    }

    public T Open<T>() where T : UIView {
        if (_views == null)
            return null;

        for (int i = 0; i < _views.Length; ++i) {
            T view = _views[i] as T;
            if (view != null) {
                OpenView(view);
                return view;
            }
        }

        return null;
    }


    // porneste view-ul -> activeaza gameObjectul aferent -> executa functiile din view de la pornire
    public void Open(UIView view) {
        if (_views == null)
            return;

        int index = Array.IndexOf(_views, view);

        if (index < 0) {
            Debug.LogError($"Cannot find view {view.name}");
            return;
        }

        OpenView(view);
    }

    public T Close<T>() where T : UIView {
        if (_views == null)
            return null;

        for (int i = 0; i < _views.Length; ++i) {
            T view = _views[i] as T;
            if (view != null) {
                view.Close();
                return view;
            }
        }

        return null;
    }

    // opreste view-ul -> dezactiveaza gameObjectul aferent
    public void Close(UIView view) {
        if (_views == null)
            return;

        int index = Array.IndexOf(_views, view);

        if (index < 0) {
            Debug.LogError($"Cannot find view {view.name}");
            return;
        }

        CloseView(view);
    }

    public T Toggle<T>() where T : UIView {
        if (_views == null)
            return null;

        for (int i = 0; i < _views.Length; ++i) {
            T view = _views[i] as T;
            if (view != null) {
                if (view.IsOpen == true) {
                    CloseView(view);
                }
                else {
                    OpenView(view);
                }

                return view;
            }
        }

        return null;
    }

    public bool IsOpen<T>() where T : UIView {
        if (_views == null)
            return false;

        for (int i = 0; i < _views.Length; ++i) {
            T view = _views[i] as T;
            if (view != null) {
                return view.IsOpen;
            }
        }

        return false;
    }

    public void CloseAll() {
        if (_views == null)
            return;

        for (int i = 0; i < _views.Length; ++i) {
            CloseView(_views[i]);
        }
    }

    public void GetAll<T>(List<T> list) {
        if (_views == null)
            return;

        for (int i = 0; i < _views.Length; ++i) {
            if (_views[i] is T element) {
                list.Add(element);
            }
        }
    }

    public bool PlaySound(AudioSetup effectSetup, EForceBehaviour force = EForceBehaviour.None) {
        return _audioEffects.PlaySound(effectSetup, force);
    }

    public bool PlayClickSound() {
        return PlaySound(_clickSound);
    }

    // GameService INTERFACE

    // ia toate UIView de la copii, el initlizeaza si porneste (Open)
    protected override sealed void OnInitialize() {
        Canvas = GetComponent<Canvas>();
        Camera = Canvas.worldCamera;
        _views = GetComponentsInChildren<UIView>(true);

        for (int i = 0; i < _views.Length; ++i) {
            UIView view = _views[i];
           
            view.Initialize(this, null);
            view.SetPriority(i);

            view.gameObject.SetActive(false);

            view.Open();
        }

        OnInitializeInternal();

        
    }

    protected override sealed void OnDeinitialize() {
        OnDeinitializeInternal();

        if (_views != null) {
            for (int i = 0; i < _views.Length; ++i) {
                _views[i].Deinitialize();
            }

            _views = null;
        }
    }

    // activeaza defaultViews (cele puse din editor)
    protected override void OnActivate() {
        base.OnActivate();

        Canvas.enabled = true;

        for (int i = 0, count = _defaultViews.SafeCount(); i < count; i++) {
            Open(_defaultViews[i]);
        }
    }

    protected override void OnDeactivate() {
        base.OnDeactivate();

        for (int i = 0, count = _views.SafeCount(); i < count; i++) {
            Close(_views[i]);
        }

        Canvas.enabled = false;
    }

    // apeleaza Tick pentru fiecae UIView
    protected override sealed void OnTick() {
        if (_views != null) {
            for (int i = 0; i < _views.Length; ++i) {
                UIView view = _views[i];
                if (view.IsOpen == true) {
                    view.Tick();
                }
            }
        }

        OnTickInternal();
    }

    // PRIVATE MEMBERS

    private void OpenView(UIView view) {
        if (view == null)
            return;

        if (view.IsOpen == true)
            return;

        view.Open_Internal();

        OnViewOpened(view);
    }

    private void CloseView(UIView view) {
        if (view == null)
            return;

        if (view.IsOpen == false)
            return;

        view.Close_Internal();

        OnViewClosed(view);
    }
}

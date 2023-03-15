using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBreaker : MonoBehaviour
{
    [SerializeField]
    private Transform _screenshootCamera;

    [SerializeField]
    private Transform _explosionSource;
    [SerializeField]
    private float _explosionForce = 100f;
    [SerializeField]
    private float _explosionRadius = 10f;

    [Header("Sound")]
    [SerializeField]
    private Transform _audioEffectsRoot;
    [SerializeField]
    private AudioSetup _audioSetup;
    private AudioEffect[] _audioEffects;

    private bool _broken = false;
    private List<FreeTransform> _initialTransforms;

    private void Awake() {

        if (_audioEffectsRoot != null) {

            _audioEffects = _audioEffectsRoot.GetComponentsInChildren<AudioEffect>();
        }
    }
    private void Start() {
        
        _initialTransforms = new List<FreeTransform>(transform.childCount);
        foreach(Transform child in transform) {

            FreeTransform tr = new FreeTransform();
            tr.position = child.position;
            tr.rotation = child.rotation;

            _initialTransforms.Add(tr);
        }
    }
    private void OnEnable() {

        BreakIt();
    }
    private void OnDisable() {

        Restore();
    }

    private void BreakIt() {

        // transform.position = Context.Instance.CameraContext.Camera.transform.position;
        // transform.rotation = Context.Instance.CameraContext.Camera.transform.rotation;

        foreach(var tr in _audioEffects) {

            tr.GetComponent<AudioSource>().SetActive(true);
            Debug.Log(tr.GetComponent<AudioSource>().enabled);
        }

        _screenshootCamera.transform.position = Camera.main.transform.position;
        _screenshootCamera.transform.rotation = Camera.main.transform.rotation;


        foreach (Transform child in transform) {

            child.GetComponent<Rigidbody>().AddExplosionForce(_explosionForce, _explosionSource.position, _explosionRadius);
        }
        _audioEffects.PlaySound(_audioSetup, EForceBehaviour.ForceAny);
        _broken = true;
    }

    private void Restore() {

        if (!_broken)
            return;

        int i = 0;
        foreach (Transform child in transform) {

            child.position = _initialTransforms[i].position;
            child.rotation = _initialTransforms[i].rotation;
            child.GetComponent<Rigidbody>().velocity = Vector3.zero;
            i++;
        }

        _broken = false;
    }

    private struct FreeTransform {

        public Vector3 position;
        public Quaternion rotation;
    }
}

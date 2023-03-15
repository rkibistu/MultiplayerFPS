using DG.Tweening;
using Fusion;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Atasat de un obiect care cand este distrus,
///     spawneaza pcikable items
/// </summary>

public class LootingBox : NetworkBehaviour {

    [SerializeField]
    [Tooltip("pickable items prefabs that box should spawn")]
    private List<GameObject> _pickablePrefabs;

    [SerializeField]
    [Tooltip("Max number of items to spawn")]
    private int _maxItems = 2;
    [SerializeField]
    private float _spawnRadius = 2f;
    private Vector2 _spawnPaddle = new Vector2(0.4f, 0.4f);

    [SerializeField]
    private Transform _audioEffectRoot;
    [SerializeField]
    private AudioSetup _audioSetup;
    private AudioEffect[] _audioEffects;


    [SerializeField]
    private ParticleSystem _openEffect;

    private int _counterSpawnedItems = 0;
    private Animation _animation;
    private bool _opened = false;

    private void Awake() {
        
        if(_audioEffectRoot != null) {

            _audioEffects = _audioEffectRoot.GetComponentsInChildren<AudioEffect>();
        }
    }

    private void Start() {

        _animation = GetComponent<Animation>();
    }

    public void OpenLootBox() {

        if (_opened)
            return;

        SpawnPickableItems_Rpc();
    }


    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    private void SpawnPickableItems_Rpc() {

        _opened = true;
        _openEffect.Play();
        _animation.Play();

        // Add delay only for host, and no delay for clients if it looks laggy on clients
        if (HasStateAuthority) {

            StartCoroutine(SpawnItemsWithDelay());
        }
    }

    private void SpawnItemsInOrder() {

        foreach (var prefab in _pickablePrefabs) {

            //var obj = Context.Instance.ObjectCache.Get(prefab);
            //var obj = Instantiate(prefab);
            var obj = Context.Instance.Runner.Spawn(prefab);
            JumpEffect(obj.transform);

            _counterSpawnedItems++;
            if (_counterSpawnedItems >= _maxItems)
                break;
        }
    }
    private void SpawnRandomItems() {

        int indexPrefab;
        for (int i = 0; i < _maxItems; i++) {

            indexPrefab = Random.Range(0, _pickablePrefabs.Count);

            var obj = Context.Instance.Runner.Spawn(_pickablePrefabs[indexPrefab]);
            JumpEffect(obj.transform);

            _counterSpawnedItems++;
        }
    }

    private void JumpEffect(Transform obj) {

        Vector2 circle = GenerateSpawnPosition();
        Vector3 hint = new Vector3(circle.x, Random.Range(1f, 3f), circle.y);
        hint = transform.position + hint;
        Vector3 dest = new Vector3(circle.x, 0, circle.y);
        dest = transform.position + dest;
        Vector3[] path = new Vector3[2];
        path[0] = hint;
        path[1] = dest;

        obj.transform.position = transform.position;
        obj.transform.DOPath(path, 0.6f, PathType.CatmullRom);
    }

    private Vector2 GenerateSpawnPosition() {

        // Insert ehre restrictions for direction of spawn fi you want
        
        Vector2 pos = (Random.insideUnitCircle + _spawnPaddle ) * _spawnRadius;
        return pos;
    }

    private IEnumerator SpawnItemsWithDelay() {

        yield return new WaitForSeconds(0.2f);
        SpawnRandomItems();
    }

    //Called by event aniamtions when lootbox starts opening
    private void PlayOpenLootboxSound() {

        Debug.Log("OPEN LOOTBOX SOUND");
        _audioEffects.PlaySound(_audioSetup, EForceBehaviour.ForceAny);
    }
}

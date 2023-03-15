using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameLightRandomizer : MonoBehaviour
{
    [SerializeField]
    private float _minIntensity;
    [SerializeField]
    private float _maxIntensity;
    [SerializeField]
    private float _changeSpeed;
    [SerializeField]
    private float _interval;

    private float _targetIntensity;
    private Light _light;

    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light>();

        _targetIntensity = Random.Range(_minIntensity, _maxIntensity);
        StartCoroutine(ChangeIntensity());
    }

    private IEnumerator ChangeIntensity() {

        while (true) {


            _light.DOIntensity(_targetIntensity, _interval);
            yield return new WaitForSeconds(_interval);



            _targetIntensity = Random.Range(_minIntensity, _maxIntensity);
        }
    }
}

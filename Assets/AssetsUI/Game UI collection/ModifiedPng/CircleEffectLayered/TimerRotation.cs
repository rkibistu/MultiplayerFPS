using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerRotation : MonoBehaviour
{
    public List<GameObject> _leftRotate;
    public List<GameObject> _rightRotate;
    void Start()
    {
        foreach (var left in _leftRotate) {

            left.transform.DORotate(new Vector3(0f, 0f, 360f), 5f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        }
        foreach (var right in _rightRotate) {

            right.transform.DORotate(new Vector3(0f, 0f, -360f), 5f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    private int _empty = Animator.StringToHash("Empty");
    private int _reset = Animator.StringToHash("Reset");
    private int _reloadAndDrop = Animator.StringToHash("ReloadAndDrop");

    public void PlayReloadAnimation() {

        Debug.Log("Play reload anim!");
        _animator.Play(_reloadAndDrop);
    }

    public void StopAllAniamtions() {

        _animator.Play(_reset);
    }
}

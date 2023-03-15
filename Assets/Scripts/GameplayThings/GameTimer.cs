using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class GameTimer : MonoBehaviour
{
    [Networked] public TickTimer _timer { get; set; }
    public Action _onTiemrExpired;

    public float? RemainingTime => _timer.RemainingTime(_runner);

    private NetworkRunner _runner;
    
    void Update()
    {
        if(!_timer.IsRunning)
            return;

        if (_timer.Expired(_runner)) {

            _timer = TickTimer.None;
            _onTiemrExpired?.Invoke();
        }
    }

    public void StartTimer(NetworkRunner runner, int seconds) {

        _runner = runner;
        _timer = TickTimer.CreateFromSeconds(runner, seconds);
    }
}

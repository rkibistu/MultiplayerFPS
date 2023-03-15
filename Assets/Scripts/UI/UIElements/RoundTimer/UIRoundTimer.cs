
using UnityEngine;
using UnityEngine.EventSystems;
using Fusion;
public class UIRoundTimer : UIBehaviour {
    // PRIVATE MEMBERS

    [SerializeField]
    private UIValue _roundTimerValue;
    private float? time;

    // PUBLIC METHODS

    public void UpdateTimerValue(GameTimer timer) {

        time = timer.RemainingTime;
        if(time.HasValue)
            _roundTimerValue.SetValue(time.Value);
    }
}
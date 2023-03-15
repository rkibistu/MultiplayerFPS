using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class UIHitNumber : UIBehaviour {
    // PUBLIC MEMBERS

    public bool IsFinished => _canvasGroup.alpha <= 0f;
    public Vector3 WorldPosition { get; set; }

    // PRIVATE MEMBERS

    [SerializeField]
    private TextMeshProUGUI _text;

    private CanvasGroup _canvasGroup;

    // PUBLIC METHODS

    public void SetNumber(float value) {
        int intValue = Mathf.RoundToInt(value);

        if (intValue == 0 && value != 0f) {
            // Do not show zero if not necessary
            intValue = value > 0f ? 1 : -1;
        }

        _text.text = intValue.ToString();
    }

    // MONOBEHAVIOU METHODS

    protected override void OnEnable() {

        _canvasGroup = GetComponent<CanvasGroup>();
    }
}


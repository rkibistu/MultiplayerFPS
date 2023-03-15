using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// Atasat de  orice obiect ce contine componenta RawIamge
///     -> aplica miscare asupra imaginii (loop, pozitia globala ramane aceeasi)
/// 
/// </summary>

[RequireComponent(typeof(RawImage))]
public class ScrollingRawImage : MonoBehaviour {
    private RawImage rawImage;
    public float xSpeed, ySpeed;
    private float xVal, yVal;

    private void Awake() {
        rawImage = GetComponent<RawImage>();
    }

    private void Update() {
        xVal += Time.deltaTime * xSpeed;
        yVal += Time.deltaTime * ySpeed;
        rawImage.uvRect = new Rect(xVal, yVal, rawImage.uvRect.width, rawImage.uvRect.height);
    }
}

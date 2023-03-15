using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TransformTranslator  : MonoBehaviour{
    

    public  void LerpTo(Transform startPoint, Vector3 endPoint, float speedMovemnt) {

        StartCoroutine(LerpToCoroutine(startPoint, endPoint, speedMovemnt));
    }
    public void MoveTo(Transform startPoint, Vector3 endPoint) {

        StopAllCoroutines();
        startPoint.localPosition = endPoint;
    }

    private IEnumerator LerpToCoroutine(Transform startPoint, Vector3 endPoint, float speedMovement) {

        while (startPoint.localPosition != endPoint) {

            startPoint.localPosition = Vector3.Lerp(startPoint.localPosition, endPoint, speedMovement);
            yield return null;
        }

        Debug.Log("Finish lerp");
        // am ajuns la destiantie
    }
}

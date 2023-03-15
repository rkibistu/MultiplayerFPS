using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveToPosition  : MonoBehaviour{
    public static void MoveTo(Transform startPoint, Transform endPoint, float speedMovemnt) {

        
    }

    IEnumerator LerpTo(Transform startPoint, Transform endPoint, float speedMovement) {

        while (startPoint.position != endPoint.position) {

            Vector3.Lerp(startPoint.position, endPoint.position, speedMovement);
            yield return null;
        }
       
        // am ajuns la destiantie
    }
}

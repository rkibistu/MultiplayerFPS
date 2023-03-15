using UnityEngine;

public class FollowTarget : MonoBehaviour {
    public Transform target;

    // Add this component to an object that you want to follow another object
    // set the target that you want to be followed

    private void Update() {
        if (target) {
            transform.position = target.position;
        }
    }
}

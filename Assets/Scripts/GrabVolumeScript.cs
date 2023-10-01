using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrabVolumeScript : MonoBehaviour
{
    static float HOLD_POSITION_OFFSET_FORWARD = 1;

    Vector3 holdPosition;
    List<Rigidbody> withinVolume;
    Rigidbody grabbed;

    void Start() {
        withinVolume = new();
    }

    void Update() {
        holdPosition = transform.position + transform.up * HOLD_POSITION_OFFSET_FORWARD;
        if (Input.GetMouseButtonDown(0) && withinVolume.Count > 0) {
            int minIndex = withinVolume.Select((item, index) => ((item.position - holdPosition).sqrMagnitude, index)).Min().index;
            grabbed = withinVolume[minIndex];
        }
        if (!Input.GetMouseButton(0)) {
            grabbed = null;
        }
    }
    void FixedUpdate() {
        Debug.DrawLine(holdPosition, holdPosition + new Vector3(0, .1f, 0));
        if (grabbed != null) {
            Debug.Log("Moving?");
            Vector3 v = grabbed.velocity;
            Vector3.SmoothDamp(grabbed.position, holdPosition, ref v, .01f, 999, Time.fixedDeltaTime);
            grabbed.velocity = v;
        }
    }

    private void OnTriggerEnter(Collider other) {
        withinVolume.Add(other.attachedRigidbody);
    }
    private void OnTriggerExit(Collider other) {
        withinVolume.Remove(other.attachedRigidbody);
    }
}

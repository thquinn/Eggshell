using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript_02BallTrigger : MonoBehaviour
{
    public RoomScript roomScript;
    public Collider targetCollider;
    public SpriteRenderer glow;

    bool done;
    float vAlpha;

    private void Update() {
        Color c = glow.color;
        c.a = Mathf.SmoothDamp(c.a, done ? 1 : 0, ref vAlpha, .05f);
        glow.color = c;
    }

    private void OnTriggerEnter(Collider other) {
        if (other == targetCollider) {
            done = true;
            roomScript.OpenDoor();
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other == targetCollider) {
            done = false;
            roomScript.CloseDoor();
        }
    }
}

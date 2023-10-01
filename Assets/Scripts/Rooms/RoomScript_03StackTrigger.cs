using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomScript_03StackTrigger : MonoBehaviour
{
    public RoomScript roomScript;
    public Rigidbody[] cubes;

    float stackTime;

    void FixedUpdate() {
        int minIndex = cubes.Select((item, index) => (item.position.y, index)).Max().index;
        Rigidbody highestCube = cubes[minIndex];
        bool stacked = highestCube.position.y > 2.4f && highestCube.velocity.sqrMagnitude < .01f && highestCube.gameObject.tag != GrabVolumeScript.TAG_GRABBED;
        if (stacked) {
            stackTime += Time.deltaTime;
        } else {
            stackTime = 0;
        }
        if (stackTime >= 2) {
            foreach (Rigidbody rb in cubes) {
                rb.isKinematic = true;
            }
            roomScript.OpenDoor();
            enabled = false;
        }
    }
}

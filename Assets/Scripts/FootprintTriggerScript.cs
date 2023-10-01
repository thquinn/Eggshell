using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootprintTriggerScript : MonoBehaviour
{
    public bool triggered;

    private void OnTriggerStay(Collider other) {
        if (AlienScript.instance.IsVODone()) {
            triggered = true;
        }
    }
}

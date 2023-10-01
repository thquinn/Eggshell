using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootprintTriggerScript : MonoBehaviour
{
    public bool triggered;

    private void OnTriggerEnter(Collider other) {
        triggered = true;
    }
}

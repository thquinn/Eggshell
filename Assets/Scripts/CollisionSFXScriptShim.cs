using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSFXScriptShim : MonoBehaviour
{
    CollisionSFXScript sfx;

    public void Init(CollisionSFXScript sfx) {
        this.sfx = sfx;
    }

    private void OnCollisionEnter(Collision collision) {
        sfx.Collision(collision);
    }
}

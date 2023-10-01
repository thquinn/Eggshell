using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSFXScript : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] clips;

    Rigidbody rb;

    void Start() {
        rb = transform.parent.GetComponent<Rigidbody>();
        transform.parent.gameObject.AddComponent<CollisionSFXScriptShim>().Init(this);
    }

    public void Collision(Collision collision) {
        if (!ShouldPlay(collision)) {
            return;
        }
        float x = collision.relativeVelocity.magnitude * rb.mass;
        float volume = 1 - 1 / (x * .33f + 1);
        source.PlayOneShot(clips[Random.Range(0, clips.Length)], volume);
    }
    bool ShouldPlay(Collision collision) {
        if (!collision.rigidbody) {
            return true;
        }
        if (collision.gameObject.GetComponent<CollisionSFXScriptShim>() == null) {
            return true;
        }
        if (collision.rigidbody.mass > rb.mass) {
            return false;
        }
        if (collision.rigidbody.mass == rb.mass && collision.gameObject.GetInstanceID() > transform.parent.gameObject.GetInstanceID()) {
            return false;
        }
        return true;
    }
}

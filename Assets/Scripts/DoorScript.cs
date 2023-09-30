using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    static float OPENCLOSE_TIME = .5f;

    public GameObject doorCollision;

    Vector3 closedPos, openPos;
    bool opening;
    Vector3 v;

    void Start() {
        closedPos = transform.localPosition;
        openPos = closedPos + new Vector3(0, 2.9f, 0);
    }
    public void Open() {
        opening = true;
        doorCollision.SetActive(false);
    }
    public void Close() {
        opening = false;
        doorCollision.SetActive(true);
    }

    void Update() {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, opening ? openPos : closedPos, ref v, OPENCLOSE_TIME);
    }
}

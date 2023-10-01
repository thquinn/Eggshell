using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    static float MOVE_SPEED = 10;
    static float MOVE_INERTIA = .9f;
    static float STOP_INERTIA = .9f;
    static float LOOK_SENSITIVITY = 2;
    static float JUMP_SPEED = 5f;
    static float COYOTE_TIME = .33f;
    static float JUMP_DELAY = .2f;

    public Rigidbody rb;
    public Camera cam;

    bool isGrounded, jumped;
    float groundTimer;
    bool intro;
    float introTimer;
    Vector3 vIntroTheta;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        intro = true;
        rb.isKinematic = true;
        transform.localRotation = Quaternion.Euler(new Vector3(-90, 20, 0));
    }

    void Update() {
        if (intro) {
            UpdateIntro();
            return;
        }
        UpdateLook();
        UpdateControls();
    }
    void FixedUpdate() {
        UpdateVelocity();
    }

    void UpdateLook() {
        float x = Input.GetAxis("Mouse X") * LOOK_SENSITIVITY;
        float y = Input.GetAxis("Mouse Y") * LOOK_SENSITIVITY;
        transform.Rotate(0, x, 0);
        float thetaX = cam.transform.localRotation.eulerAngles.x;
        if (thetaX > 180) {
            thetaX -= 360;
        }
        thetaX = Mathf.Clamp(thetaX - y, -90, 90);
        cam.transform.localRotation = Quaternion.Euler(thetaX, 0, 0);
    }
    void UpdateControls() {
        isGrounded = Util.IsOnGround(gameObject, 8, .5f, .1f);
        if (groundTimer < 0) {
            groundTimer = Mathf.Min(0, groundTimer + Time.deltaTime);
        } else if (isGrounded) {
            groundTimer = COYOTE_TIME;
        } else {
            groundTimer = Mathf.Max(0, groundTimer - Time.deltaTime);
        }
        bool jumpButton = Input.GetButtonDown("Jump");
        if (groundTimer > 0 && !jumped && jumpButton) {
            jumped = true;
            groundTimer = -JUMP_DELAY;
            //sfxJumps[Random.Range(0, sfxJumps.Length)].Play();
        }
    }
    void UpdateVelocity() {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (input.sqrMagnitude > 1) {
            input.Normalize();
        }
        Vector3 inputVelocity = transform.forward * input.y * MOVE_SPEED + transform.right * input.x * MOVE_SPEED;
        Vector3 moveVelocity;
        moveVelocity = new Vector3(
            Mathf.Lerp(rb.velocity.x, inputVelocity.x, inputVelocity.x == 0 ? STOP_INERTIA : MOVE_INERTIA),
            0,
            Mathf.Lerp(rb.velocity.z, inputVelocity.z, inputVelocity.z == 0 ? STOP_INERTIA : MOVE_INERTIA)
        );

        float moveMagnitude = moveVelocity.magnitude;
        float yVelocity = rb.velocity.y;
         if (jumped) {
            yVelocity = JUMP_SPEED;
        }
        jumped = false;
        rb.velocity = new Vector3(moveVelocity.x, yVelocity, moveVelocity.z);
    }

    void UpdateIntro() {
        introTimer += Time.deltaTime;
        if (introTimer > 2) {
            transform.localRotation = Util.SmoothDampQuaternion(transform.localRotation, Quaternion.identity, ref vIntroTheta, 1);
        }
        if (Mathf.Abs(transform.localRotation.eulerAngles.x) < 1) {
            transform.localRotation = Quaternion.identity;
            rb.isKinematic = false;
            intro = false;
        }
    }
}

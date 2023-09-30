using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code
{
    public static class Util
    {
        static LayerMask layerMaskTerrain;
        static Util() {
            layerMaskTerrain = LayerMask.GetMask("Terrain");
        }

        public static Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime) {
            Vector3 c = current.eulerAngles;
            Vector3 t = target.eulerAngles;
            return Quaternion.Euler(
              Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime),
              Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime),
              Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime)
            );
        }

        public static float SampleRangeVector(Vector2 v) {
            return Mathf.Lerp(v.x, v.y, UnityEngine.Random.value);
        }

        public static bool IsOnGround(GameObject go, int numChecks, float radius, float height) {
            Vector3 position = go.transform.position;
            position.y += height * .5f;
            for (int i = -1; i < numChecks; i++) {
                RaycastHit hitInfo;
                if (i == -1) {
                    Physics.Raycast(position, Vector3.down, out hitInfo, height, layerMaskTerrain);
                }
                else {
                    float theta = 2 * Mathf.PI / numChecks * i;
                    position.x += Mathf.Cos(theta) * radius;
                    position.z += Mathf.Sin(theta) * radius;
                    Physics.Raycast(position, Vector3.down, out hitInfo, height, layerMaskTerrain);
                }
                if (hitInfo.collider != null && Vector3.Dot(hitInfo.normal, Vector3.up) > .5f) {
                    return true;
                }
            }
            return false;
        }
    }
}

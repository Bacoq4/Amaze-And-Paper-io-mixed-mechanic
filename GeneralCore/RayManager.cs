using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GeneralCore
{
    /// <summary>
    /// Related all about raycast
    /// </summary>
    public class RayManager : MonoBehaviour
    {
#if UNITY_EDITOR
        
        public static bool IsThereObjectAtHit(out RaycastHit hit, SceneView sceneView, Event editorEvent)
        {
            Ray ray = GetRayFromCameraToWorld(sceneView, editorEvent);

            if (Physics.Raycast(ray, out hit, 10000))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        
        public static bool IsThereObjectAtHitWithLayermask(string layerMask, SceneView sceneView, Event editorEvent)
        {
            Ray ray = GetRayFromCameraToWorld(sceneView, editorEvent);

            if (Physics.Raycast(ray, out RaycastHit hit, 10000, LayerMask.GetMask(layerMask)))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsThereObjectAtHitWithLayermask(string layerMask, out RaycastHit hit, SceneView sceneView,  Event editorEvent)
        {
            Ray ray = GetRayFromCameraToWorld(sceneView, editorEvent);

            if (Physics.Raycast(ray, out hit, 10000, LayerMask.GetMask(layerMask)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public static Ray GetRayFromCameraToWorld(SceneView scene, Event editorEvent)
        {
            var mousePos = GetMousePos(scene, editorEvent);
            Ray ray = scene.camera.ScreenPointToRay(mousePos);
            return ray;
        }
        
        public static Vector3 GetMousePos(SceneView scene, Event e)
        {
            Vector3 mousePos = e.mousePosition;
            float ppp = EditorGUIUtility.pixelsPerPoint;
            mousePos.y = scene.camera.pixelHeight - mousePos.y * ppp;
            mousePos.x *= ppp;
            return mousePos;
        }
#endif
        public static Ray GetRayFromCameraToWorld()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return ray;
        }
        
        public static Ray GetRay(Vector3 startingPoint, Vector3 dir)
        {
            Ray ray = new Ray(startingPoint, dir);
            return ray;
        }

        public static void ThrowRay(Vector3 startingPoint, Vector3 dir, out RaycastHit hit ,string layerMask, bool showDebug, float duration)
        {
           
            Ray ray = GetRay(startingPoint, dir);
            Physics.Raycast(ray, out hit, 10000, LayerMask.GetMask(layerMask));
            if (showDebug)
            {
                Debug.DrawRay(ray.origin,ray.direction * 10000,Color.red, duration);
            }
        }
    }

}

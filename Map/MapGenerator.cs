using System;
using NaughtyAttributes;
using UnityEngine;
using GeneralCore;
using PlayerRelated;

#if UNITY_EDITOR
using UnityEditor;

namespace MapGeneration
{
    /// <summary>
    /// Map generator for constructing grid based maps during only editor phase
    /// </summary>
    [ExecuteInEditMode]
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] 
        private bool letEditorScripting;
        [Header("---MapParent---")]
        private Transform spawnParent;

        // Spawn type can be made interchangeable
        private enum SpawnObjType
        {
            WalkableCube,
            Wall
        }
        
        [SerializeField]
        [Header("---MapObjType---")] 
        private SpawnObjType spawnObjType;

        [Header("---MapObjects---")]
        [SerializeField] private BaseMapObject currentSpawningPrefab;
        [SerializeField] private BaseMapObject[] spawnPrefabs;

        [Header("---Editor---")] 
        private SceneView sceneView;
        private Event editorEvent;

        [Header("---Player---")] 
        [SerializeField] private Player playerPrefab;
        [SerializeField] private Player currentPlayer;

        [Header("---KeyCodes---")] 
        [SerializeField] private KeyCode SpawnCode;

        [SerializeField] private KeyCode DeleteCode;
        [SerializeField] private KeyCode PlayerSpawnCode;

        [Header("---KeyBools---")] 
        [SerializeField] private bool SpawnKeyPressed = false;
        [SerializeField] private bool DeleteKeyPressed = false;

        [Button("Restart Map (use with caution!)")]
        private void RestartMap()
        {
            int childCount = transform.childCount;
            Transform[] transforms = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
            {
                transforms[i] = transform.GetChild(i);
            }

            for (int i = 0; i < childCount; i++)
            {
                DestroyImmediate(transforms[i].gameObject, true);
            }

            spawnParent = new GameObject("SpawnParent").transform;
            spawnParent.transform.SetParent(transform);
        }

        [Button("Change Spawn Type")]
        private void changeSpawnType()
        {
            switch (spawnObjType)
            {
                case SpawnObjType.Wall:
                    spawnObjType = SpawnObjType.WalkableCube;
                    break;
                case SpawnObjType.WalkableCube:
                    spawnObjType = SpawnObjType.Wall;
                    break;
            }
        }

        private void OnEnable()
        {
            if (!Application.isEditor)
            {
                Destroy(this);
            }

            SceneView.duringSceneGui += OnScene;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnScene;
        }

        void OnScene(SceneView scene)
        {
            sceneView = scene;
            editorEvent = Event.current;

            if (!letEditorScripting) return;

            if (editorEvent.type == EventType.KeyDown && editorEvent.keyCode == SpawnCode)
            {
                SpawnKeyPressed = true;
            }

            if (editorEvent.type == EventType.KeyUp && editorEvent.keyCode == SpawnCode)
            {
                SpawnKeyPressed = false;
            }

            if (editorEvent.type == EventType.KeyDown && editorEvent.keyCode == DeleteCode)
            {
                DeleteKeyPressed = true;
            }
            if (editorEvent.type == EventType.KeyUp && editorEvent.keyCode == DeleteCode)
            {
                DeleteKeyPressed = false;
            }
            if (editorEvent.type == EventType.KeyDown && editorEvent.keyCode == PlayerSpawnCode)
            {
                HandlePlayerSpawn();
            }
            else if (SpawnKeyPressed)
            {
                SpawnMapObject();
            }
            else if (DeleteKeyPressed)
            {
                DeleteObject();
            }
        }

        private void HandlePlayerSpawn()
        {
            bool isThereWall = RayManager.IsThereObjectAtHitWithLayermask(SpawnObjType.Wall.ToString(), out RaycastHit hitWall, sceneView, editorEvent);
            bool isThereWalkArea = RayManager.IsThereObjectAtHitWithLayermask(SpawnObjType.WalkableCube.ToString(), out RaycastHit hitWalk, sceneView, editorEvent);
            Debug.Log(isThereWall + "<-wall - walk->" + isThereWalkArea);

            if (isThereWall)
            {
                return;
            }

            if (!isThereWalkArea)
            {
                return;
            }

            hitWalk.point += Vector3.up / 2;

            if (!currentPlayer)
            {
                Player player = SpawnFromPrefab(TruncateVectorCoords(hitWalk.point), playerPrefab);
                currentPlayer = player;
                Debug.Log("Spawn Player");
            }
            else
            {
                if (currentPlayer != playerPrefab)
                {
                    DestroyImmediate(currentPlayer.gameObject);
                    Player player = SpawnFromPrefab(TruncateVectorCoords(hitWalk.point), playerPrefab);
                    currentPlayer = player;
                    Debug.Log("Player Spawned");
                }

                MoveTransform(currentPlayer.transform, TruncateVectorCoords(hitWalk.point));
                Debug.Log("Move Player");
            }
        }

        private void DeleteObject()
        {
            if (RayManager.IsThereObjectAtHit(out RaycastHit hit, sceneView, editorEvent)) ;
            {
                BaseMapObject mapObject = hit.transform.GetComponent<BaseMapObject>();

                if (mapObject != null)
                {
                    DestroyImmediate(hit.transform.gameObject);
                }
            }
        }

        private void SpawnMapObject()
        {
            var ray = RayManager.GetRayFromCameraToWorld(sceneView, editorEvent);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;

            if (plane.Raycast(ray, out rayDistance))
            {
                Vector3 hitPos = ray.origin + ray.direction * rayDistance;

                switch (spawnObjType)
                {
                    case SpawnObjType.WalkableCube:
                        currentSpawningPrefab = spawnPrefabs[0]; 
                        break;
                    case SpawnObjType.Wall:
                        hitPos += Vector3.up;
                        currentSpawningPrefab = spawnPrefabs[1];
                        break;
                }

                if (RayManager.IsThereObjectAtHitWithLayermask(spawnObjType.ToString(), sceneView, editorEvent))
                {
                    hitPos = TruncateVectorCoords(hitPos);
                    SpawnFromPrefab(hitPos, currentSpawningPrefab);
                }
            }
        }

        private GameObject GeneratePrimitive(Vector3 hitPos, PrimitiveType primitiveType)
        {
            GameObject go = GameObject.CreatePrimitive(primitiveType);
            hitPos = TruncateVectorCoords(hitPos);
            go.transform.position = hitPos;
            go.transform.SetParent(spawnParent);
            return go;
        }

        private BaseMapObject SpawnFromPrefab(Vector3 hitPos, BaseMapObject prefab)
        {
            BaseMapObject mapObject = Instantiate(prefab, hitPos, Quaternion.identity);
            mapObject.transform.SetParent(spawnParent);
            return mapObject;
        }

        private Player SpawnFromPrefab(Vector3 hitPos, Player prefab)
        {
            Player player = Instantiate(prefab, hitPos, Quaternion.identity);
            player.transform.SetParent(spawnParent);
            return player;
        }

        // can be move into math related class in the future
        private Vector3 TruncateVectorCoords(Vector3 vec)
        {
            Vector3 newVec = new Vector3(0, vec.y, 0);
            newVec.x = Mathf.RoundToInt(vec.x);
            newVec.z = Mathf.RoundToInt(vec.z);
            return newVec;
        }
        
        private void MoveTransform(Transform tr, Vector3 newPos)
        {
            tr.position = newPos;
        }
    }
}
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using GeneralCore;
using NaughtyAttributes;


namespace PlayerRelated
{
    /// <summary>
    /// Execute Player Movement
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        [Expandable]
        [SerializeField] private ScriptableInputData InputData;
        [SerializeField] private float speed = 10;
        [SerializeField] private PlayerCubeRelation cubeRelation;
        private bool isMoving;
        void Update()
        {
            if (!isMoving && ( Mathf.Abs(InputData.Horizontal) > 0.01f || Mathf.Abs(InputData.Vertical) > 0.01f) )
            {
                isMoving = true;
                Vector3 dir = new Vector3(InputData.Horizontal, 0, InputData.Vertical);
                Move(dir);
            }
        }

        void Move(Vector3 dir)
        {
            RayManager.ThrowRay(transform.position, dir, out RaycastHit hit, "Wall", true, 5);
            if (!hit.transform) { return; }
            
            Vector3 lastVec = getLastOffset(dir.x, dir.z);
            
            hit.point += lastVec;

            float dist = Vector3.Distance(transform.position, hit.point);
            float time = dist / speed;
                
            transform.DOMove(hit.point, time).SetEase(Ease.Linear).OnComplete(() =>
            {
                isMoving = false;
                cubeRelation.checkCorners();
                bool isGameFinished = cubeRelation.isEveryCubeColored();
                print(isGameFinished);
                if(isGameFinished) { GameManager.Instance.LevelCompleted(); }
            });
        }

        Vector3 getLastOffset(float hor, float ver)
        {
            Vector3 lastOffset = Vector3.zero;
            if (hor > 0.1f)
            {
                lastOffset += Vector3.left / 2;
            }
            else if (hor < -0.1f)
            {
                lastOffset += Vector3.right / 2;
            }
            else if (ver > 0.1f)
            {
                lastOffset += Vector3.back / 2;
            }
            else if (ver < -0.1f)
            {
                lastOffset += Vector3.forward / 2;
            }

            return lastOffset;
        }
    }

}

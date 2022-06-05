using System;
using System.Collections;
using System.Collections.Generic;
using MapGeneration;
using UnityEngine;

namespace PlayerRelated
{
    /// <summary>
    /// player trigger, working when passing over walkable cubes
    /// </summary>
    public class PlayerTrigger : MonoBehaviour
    {
        [SerializeField] private PlayerCubeRelation cubeRelation;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("walkableCube"))
            {
                cubeRelation.changeColliderMaterial(other);
            }
        }

        
    }
}


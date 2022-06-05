using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MapGeneration;
using Unity.VisualScripting;
using UnityEngine;

namespace PlayerRelated
{
    /// <summary>
    /// Handling cube coloring processes with player
    /// </summary>
    public class PlayerCubeRelation : MonoBehaviour
    {
        [SerializeField] private MeshRenderer renderer;
        public MeshRenderer playerRenderer { get => renderer; }
        private WalkableCube[] cubes;

        [SerializeField] private CubeColorController[] colorControllers;
        
        private void Awake()
        {
            cubes = FindObjectsOfType<WalkableCube>();
            foreach (var colorController in colorControllers)
            {
                foreach (var cornerCube in colorController.cornerCubes)
                {
                    cornerCube.isCornerCube = true;
                }
            }
        }

        public bool isEveryCubeColored()
        {
            bool b = true;
            foreach (var cube in cubes)
            {
                if (!cube.isColored)
                {
                    b = false;
                }
            }

            return b;
        }
        
        public void changeColliderMaterial(Collider other)
        {
            // works in player ontriggerenter 
            WalkableCube cube = other.GetComponent<WalkableCube>();
            other.GetComponent<MeshRenderer>().material.DOColor(renderer.material.color,2);
            cube.isColored = true;
        }
        
        private void changeColors(WalkableCube[] cubes)
        {
            foreach (var cube in cubes)
            {
                cube.GetComponent<MeshRenderer>().material.DOColor(renderer.material.color,2);
                cube.isColored = true;
            }
        }

        public void checkCorners()
        {
            foreach (var colorController in colorControllers)
            {
                bool isFullCornersColored = true;

                foreach (var cornerCube in colorController.cornerCubes)
                {
                    if (!cornerCube.isColored)
                    {
                        isFullCornersColored = false;
                    }
                }

                if (isFullCornersColored)
                {
                    changeColors(colorController.colorCubes);
                }
            }
        }
        
        [System.Serializable]
        struct CubeColorController
        {
            public WalkableCube[] cornerCubes;
            public WalkableCube[] colorCubes;

        }
    }
    
    
}
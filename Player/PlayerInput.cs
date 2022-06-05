using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace PlayerRelated
{
    /// <summary>
    /// Setting Player Input
    /// </summary>
    public class PlayerInput : MonoBehaviour
    {
        [Expandable]
        [SerializeField] private ScriptableInputData InputData;

        private void Start()
        {
            InputData.Horizontal = 0;
            InputData.Vertical = 0;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                InputData.Horizontal = 1;
                InputData.Vertical = 0;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                InputData.Horizontal = -1;
                InputData.Vertical = 0;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                InputData.Horizontal = 0;
                InputData.Vertical = 1;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                InputData.Horizontal = 0;
                InputData.Vertical = -1;
            }
            else if (!Input.anyKey)
            {
                InputData.Horizontal = 0;
                InputData.Vertical = 0;
            }
            //InputData.Horizontal = Input.GetAxisRaw("Horizontal");
            //InputData.Vertical = Input.GetAxisRaw("Vertical");
        }
    }

}

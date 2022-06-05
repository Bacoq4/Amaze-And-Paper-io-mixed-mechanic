using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerRelated
{
    /// <summary>
    /// Player input datas, to move across objects
    /// </summary>
    [CreateAssetMenu(menuName = "InputData")]
    public class ScriptableInputData : ScriptableObject
    {
        [SerializeField] private float horizontal;
        [SerializeField] private float vertical;

        public float Horizontal
        {
            get => horizontal;
            set => horizontal = value;
        }

        public float Vertical
        {
            get => vertical;
            set => vertical = value;
        }
    }

}



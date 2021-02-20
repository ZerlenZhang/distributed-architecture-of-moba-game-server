using UnityEngine;

namespace PurificationPioneer.Script
{
    public class MyCube : MonoBehaviour
    {
        [SerializeField] private bool _isTrigger;
        [SerializeField] private Material _cubeMaterial;
        [SerializeField] private PhysicMaterial _cubePhysicsMaterial;

        [HideInInspector] public GameObject[] faces = new GameObject[6];

        public const int TopIndex = 0;
        public const int BottomIndex = 1;
        public const int ForwardIndex = 2;
        public const int BackIndex = 3;
        public const int LeftIndex = 4;
        public const int RightIndex = 5;

        public GameObject Right
        {
            get => faces[RightIndex];
            set => faces[RightIndex] = value;
        }
        public GameObject Left
        {
            get => faces[LeftIndex];
            set => faces[LeftIndex] = value;
        }
        
        public GameObject Back
        {
            get => faces[BackIndex];
            set => faces[BackIndex] = value;
        }
        
        public GameObject Forward
        {
            get => faces[ForwardIndex];
            set => faces[ForwardIndex] = value;
        }
        
        public GameObject Top
        {
            get => faces[TopIndex];
            set => faces[TopIndex] = value;
        }

        public GameObject Bottom
        {
            get => faces[BottomIndex];
            set => faces[BottomIndex] = value;
        }
    }
}
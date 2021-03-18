using System;
using UnityEngine;

namespace PurificationPioneer
{
    public class Walker : MonoBehaviour
    {
        public float speed;

        private void Update()
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
        }
    }
}
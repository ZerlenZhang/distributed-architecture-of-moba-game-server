using System;
using Cinemachine;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Common;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    public class LocalCameraHelper : MonoSingleton<LocalCameraHelper>
    {
        [SerializeField]private Camera usedCamera;

        public Camera ActivateCamera
        {
            get
            {
                if (usedCamera == null)
                {
                    usedCamera=Camera.main;
                    if (!usedCamera)
                        throw new Exception($"[LocalCameraHelper] usedCamera is null and Camera.main is null");
                }

                return usedCamera;
            }
        }
        public CinemachineFreeLook vcam;
        private AndroidInputAxis androidInputAxis;

        /// <summary>
        /// 初始化本地相机参数
        /// </summary>
        /// <param name="follow"></param>
        /// <param name="lookAt"></param>
        /// <param name="inputProvider"></param>
        public void Init(Transform follow, Transform lookAt)
        {
            Assert.IsTrue(vcam);
            vcam.Follow = follow;
            vcam.LookAt = lookAt;
#if UNITY_EDITOR
            if (GameSettings.Instance.WorkAsAndroid)
            {
                androidInputAxis = new AndroidInputAxis();
                vcam.m_XAxis.SetInputAxisProvider(AndroidInputAxis.X, androidInputAxis);
                vcam.m_YAxis.SetInputAxisProvider(AndroidInputAxis.Y, androidInputAxis);   
            }
#elif UNITY_ANDROID
            androidInputAxis = new AndroidInputAxis();
            vcam.m_XAxis.SetInputAxisProvider(AndroidInputAxis.X, androidInputAxis);
            vcam.m_YAxis.SetInputAxisProvider(AndroidInputAxis.Y, androidInputAxis); 
#endif
        }

        public Vector2 GetCameraDirectionXZ()
        {
            var cameraForward = ActivateCamera.transform.forward;
            var expectedForward = new Vector2(
                cameraForward.x, cameraForward.z).normalized;
            return expectedForward;
        }
        
        
        private void Update()
        {
            androidInputAxis?.Update();
        }

        /// <summary>
        /// 安卓手机获得输入轴
        /// </summary>
        private class AndroidInputAxis : AxisState.IInputAxisProvider
        {
            public const int X = 0;
            public const int Y = 1;

            private Vector3 lastInput;
            private Vector2 value;

            public Vector2 Value => value;

            public AndroidInputAxis()
            {
                lastInput = Input.mousePosition;
            }

            private bool inputVaild = false;
            public void Update()
            {
                if (Input.GetMouseButton(0))
                {
                    var input = Input.mousePosition;
                    if (input.x > Screen.width / 2f)
                    {
                        inputVaild = true;
                        value.x = (input.x - lastInput.x)/Screen.width/Time.deltaTime;
                        value.y = (input.y - lastInput.y)/Screen.height/Time.deltaTime;
                        lastInput = input;                        
                    }

                }

                if (Input.GetMouseButtonUp(0) && inputVaild)
                {
                    inputVaild = false;
                    lastInput = Input.mousePosition;
                    value=Vector2.zero;
                }
            }
            
            public float GetAxisValue(int axis)
            {
                switch (axis)
                {
                    case X:
                        return value.x;
                    case Y:
                        return value.y;
                }
                throw new Exception($"Unexpected axis: {axis}");
            }
        }        
        
    }
}
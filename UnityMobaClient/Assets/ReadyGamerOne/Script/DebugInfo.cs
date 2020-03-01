using UnityEngine;

namespace ReadyGamerOne.Script
{
    public class DebugInfo:MonoBehaviour
    {
        public int targetFps = 60;
        /// <summary>
        /// 固定时间间隔
        /// </summary>
        private float time_delta = 0.5f;
        /// <summary>
        /// 上次统计FPS的时间
        /// </summary>
        private float prevTime = 0.0f;
        /// <summary>
        /// 计算出的FPS值
        /// </summary>
        private float fps = 0.0f;
        /// <summary>
        /// 累计刷新帧数
        /// </summary>
        private float i_frames=0;

        private GUIStyle _style;

        private void Awake()
        {
            //Application.targetFrameRate = this.targetFps;
        }

        private void Start()
        {
            this.prevTime = Time.realtimeSinceStartup;
            this._style=new GUIStyle
            {
                fontSize = 15,
            };
            _style.normal.textColor=Color.black;
        }

        private void Update()
        {
            this.i_frames++;
            if (Time.realtimeSinceStartup >= this.prevTime + this.time_delta)
            {
                this.fps = this.i_frames / (Time.realtimeSinceStartup - this.prevTime);
                this.prevTime = Time.realtimeSinceStartup;
                this.i_frames = 0;
            }
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(0, 0, 200, 200), "FPS:" + this.fps);
        }
    }
}
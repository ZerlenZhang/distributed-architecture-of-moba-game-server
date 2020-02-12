using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ReadyGamerOne.Script
{
    public abstract class AbstractCharacterController : MonoBehaviour
    {
        #region Properties

        #region 环境类型设定

        /// <summary>
        /// 移动原理，运动学还是动力学
        /// </summary>
        public enum MoveType
        {
            Physical,
            Transform
        }

        public MoveType moveType = MoveType.Transform;

        public Space spaceType=Space.World;
        
        /// <summary>
        /// 2D世界还是 3D世界
        /// </summary>
        public enum WorldType
        {
            _2D,
            _3D
        }

        public WorldType worldType = WorldType._2D;

        /// <summary>
        /// 输入类型
        /// </summary>
        public enum InputType
        {
            Input_GetAxis,
            Input_GetKey
        }

        public InputType inputType = InputType.Input_GetAxis;

        public string horizontalAxis = "Horizontal";
        public string verticalAxis = "Vertical";

        public KeyCode leftKey = KeyCode.A;
        public KeyCode rightKey = KeyCode.D;
        public KeyCode upKey = KeyCode.W;
        public KeyCode downKey = KeyCode.S;

        [HideInInspector] public bool enableSimulator = false;


        /// <summary>
        /// 移动方向
        /// </summary>
        public enum DirType
        {
            X,
            Y,
            X_Y,
            X_Z
        }

        public DirType dirType = DirType.X;

        #endregion

        [FormerlySerializedAs("autoFitAnimation")] public bool FitDirByLocalScale = true;

        #region StateProperties

        #region IdleState

        public string idleAniName;
        [SerializeField] private int idleAniIndex;

        private Action _onIdle;
        public virtual Action onIdle
        {
            get
            {
                if (_onIdle == null)
                    _onIdle = ToIdle;
                return _onIdle;
            }
            set
            {
                _onIdle = value;
            }
        }

        #endregion


        #region Walk

        public bool enableWalk = true;
        public abstract Func<Vector2, bool> CanWalk { get; }
        private Action<Vector2, Vector2> _onWalk;
        public virtual Action<Vector2,Vector2> onWalk
        {
            get
            {
                if (_onWalk == null)
                    _onWalk = ToWalk;
                return _onWalk;
            }
            set
            {
                _onWalk = value;
            }
        }

        public Vector2 walkScaler;
        public string walkAniName;

        [SerializeField] private int walkAniIndex;

        #endregion


        #region Run

        public bool enableRun = false;
        public virtual Func<Vector2, bool> CanRun => null;

        private Action<Vector2, Vector2> _onRun;
        public virtual Action<Vector2, Vector2> onRun
        {
            get
            {
                if (_onRun == null)
                    _onRun = ToRun;
                return _onRun;
            }
            set
            {
                _onRun = value;
            }
        }
        public Vector2 runScaler;
        public string runAniName;

        [SerializeField] private int runAniIndex;

        #endregion


        #region Dash
        
        public enum LerpType
        {
            Line,
            Sphere
        }

        public bool enableDash = false;
        public virtual Func<Vector2, bool> CanDash => null;
        public virtual Action<Vector2> onStartDash => ToDash;
        public string dashAniName;

        [SerializeField] private int dashAniIndex;

        public KeyCode dashKey = KeyCode.LeftShift;
        public float dashDistance;
        [FormerlySerializedAs("dashArg")] public float dashTime = 0.3f;
        public LerpType lerpType = LerpType.Sphere;

        #endregion


        #region Squat


        public KeyCode squatKey=KeyCode.LeftControl;
        public bool enableSquat = false;
        public virtual Func<Vector2, bool> CanSquat => null;
        public virtual Action<Vector2> onSquat => ToSquat;
        public string squatAniName;
        [SerializeField] private int squatAniIndex;

        #endregion        


        #endregion


        #endregion
        

        #region Virtual

        #region MonoBehavior


        // Start is called before the first frame update
        protected virtual void Start()
        {
            //如果物理系统驱动，需要初始化刚体组件
            if (moveType == MoveType.Physical)
            {
                switch (worldType)
                {
                    case WorldType._2D:
                        _rigidbody2D = GetComponent<Rigidbody2D>();
                        if (_rigidbody2D == null)
                            throw new Exception("获取Rigidbody2D失败");
                        break;
                    case WorldType._3D:
                        _rigidbody = GetComponent<Rigidbody>();
                        if (_rigidbody == null)
                            throw new Exception("获取RigidBody失败");
                        break;
                }
            }
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (enableSimulator)
                return;

            if (moveType == MoveType.Transform)
            {
                var input = GetInput();

                #region Fit_Dir

                if (FitDirByLocalScale)
                {
                    AutoFitDirByLocalScale(input);
                }

                #endregion


                #region Squat
                
                if (enableSquat && !LockState && CanSquat != null && CanSquat(input))
                {
                    onSquat(input);
                    if (EnableAni)
                        ani.Play(Animator.StringToHash(squatAniName));
                }

                #endregion
                
                #region Dash

                else if (enableDash && !LockState && CanDash != null && CanDash(input))
                {
                    if (IfCanMove(input))
                    {
                        onStartDash(input);
                    }

                    if(EnableAni)
                        ani.Play(Animator.StringToHash(dashAniName));
                }

                #endregion

                #region Run

                else if (enableRun && !LockState && CanRun != null && CanRun(input))
                {
                    if (IfCanMove(input))
                    {
                        onRun(input,runScaler);
                    }

                    if(EnableAni)
                        ani.Play(Animator.StringToHash(runAniName));
                }

                #endregion

                #region Walk

                else if (enableWalk && !LockState && CanWalk != null && CanWalk(input))
                {
                    if (IfCanMove(input))
                    {
                        onWalk(input,walkScaler);
                    }

                    if (EnableAni)
                        ani.Play(Animator.StringToHash(walkAniName));
                }

                #endregion

                #region IdleState

                else if (!LockState)
                {
                    onIdle();
                }

                #endregion
            }
        }


        protected virtual void FixedUpdate()
        {
            if (moveType == MoveType.Physical)
            {
                var input = GetInput();
            }
        }        

        #endregion

        #region CommonLogic

        /// <summary>
        /// 获取用户输入
        /// </summary>
        /// <returns></returns>
        public virtual Vector2 GetInput()
        {
            Vector2 input = Vector2.zero;
            switch (inputType)
            {
                case InputType.Input_GetAxis:
                    input = new Vector2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis));
                    break;
                case InputType.Input_GetKey:
                    if (Input.GetKey(leftKey))
                        input.x = -1;
                    else if (Input.GetKey(rightKey))
                        input.x = 1;
                    if (Input.GetKey(upKey))
                        input.y = 1;
                    else if (Input.GetKey(downKey))
                        input.y = -1;
                    break;
            }

            return input;
        }

        /// <summary>
        /// 人物面向的方向
        /// </summary>
        protected virtual int Dir => transform.localScale.x > 0 ? 1 : -1;

        protected virtual Func<Vector2, bool> IfCanMove => (input) => true;


        #endregion

        #endregion

        #region Public

        public enum StateType
        {
            Idle,
            Walk,
            Run,
            Dash
        }

        /// <summary>
        /// 模拟用户输入后的效果
        /// </summary>
        /// <param CharacterName="type"></param>
        /// <param CharacterName="input"></param>
        public void Simulator(StateType type,Vector2 input)
        {
            AutoFitDirByLocalScale(input);
            switch (type)
            {
                case StateType.Idle:
                    ToIdle();
                    break;
                case StateType.Walk:
                    onWalk(input, walkScaler);
                    if (EnableAni)
                        ani.Play(walkAniName);
                    break;
                case StateType.Run:
                    onRun(input, runScaler);
                    if (EnableAni)
                        ani.Play(runAniName);
                    break;
                case StateType.Dash:
                    onStartDash(input);
                    if (EnableAni)
                        ani.Play(dashAniName);
                    break;
            }
        }

        #endregion

        #region Private

        #region Fields

        private Rigidbody _rigidbody;
        private Rigidbody2D _rigidbody2D;
        private Animator ani => GetComponent<Animator>();

        private bool LockState = false;

        #endregion

        #region Methods

        private int dirBefore = 1;

        private void TransformWithScaler(Vector3 input, Vector3 scale)
        {
            transform.Translate(
                Time.deltaTime * new Vector3(input.x * scale.x, input.y*scale.y, input.z * scale.z),
                spaceType);
        }




        #endregion
        
        #region StatesLogic

        /// <summary>
        /// 开始跑步，持续触发
        /// </summary>
        /// <param CharacterName="input"></param>
        private void ToWalk(Vector2 input,Vector2 walkScaler)
        {
            switch (dirType)
            {
                case DirType.X:
                    TransformWithScaler(new Vector2(input.x, 0), walkScaler);
                    break;
                case DirType.Y:
                    TransformWithScaler(new Vector2(0, input.y), walkScaler);
                    break;
                case DirType.X_Y:
                    TransformWithScaler(input, walkScaler);
                    break;
                case DirType.X_Z:
                    TransformWithScaler(new Vector3(input.x,0,input.y), new Vector3(walkScaler.x,0,walkScaler.y));
                    break;
            }
        }

        /// <summary>
        /// 开始跑步，持续调用
        /// </summary>
        /// <param CharacterName="input"></param>
        private void ToRun(Vector2 input,Vector2 runScaler)
        {
            switch (dirType)
            {
                case DirType.X:
                    TransformWithScaler(new Vector2(input.x, 0), runScaler);
                    break;
                case DirType.Y:
                    TransformWithScaler(new Vector2(0, input.y), runScaler);
                    break;
                case DirType.X_Y:
                    TransformWithScaler(input, runScaler);
                    break;
                case DirType.X_Z:
                    TransformWithScaler(new Vector3(input.x,0,input.y), new Vector3(runScaler.x,0,runScaler.y));
                    break;
            }
        }

        /// <summary>
        /// 开始冲刺，只调用一次！！
        /// </summary>
        /// <param CharacterName="input"></param>
        private void ToDash(Vector2 input)
        {
            LockState = true;
            
            var targetPos = transform.position + new Vector3(Dir * dashDistance, 0, 0);

            //球形插值
            var a = dashTime;
            var b = dashDistance * 4 / (Mathf.PI * a);
            var timer = 0f;
            
            //线性插值
            var v = dashDistance * 2 / dashTime;

            MainLoop.Instance.ExecuteUntilTrue(() =>
            {
                if (enableDash)
                {
                    if (IfCanMove(input))
                    {
                        var speed = 0f;
                        switch (lerpType)
                        {
                            case LerpType.Line:
                                speed=(-v/dashTime)*timer + v;
                                break;
                            case LerpType.Sphere:
                                speed=b * Mathf.Sqrt(1 - timer * timer / (a * a));
                                break;
                        }
                        
                        if (float.IsNaN(speed)||speed<=0)
                            return true;
                        
                        transform.Translate(new Vector3(Dir * speed * Time.deltaTime, 0, 0), spaceType);
                        
                        timer += Time.deltaTime;
                        
                        if(EnableAni)
                            ani.Play(Animator.StringToHash(dashAniName));
                        
                    }
                    else
                        return true;

                    return Vector3.Distance(targetPos, transform.position) <= 0.01f;
                }

                return true;
            }, () => LockState = false);
        }

        /// <summary>
        /// 开始下蹲，持续调用
        /// </summary>
        /// <param CharacterName="input"></param>
        private void ToSquat(Vector2 input)
        {
            
        }        

        /// <summary>
        /// Idle状态，持续触发
        /// </summary>
        private void ToIdle()
        {
            //IdleState
            if (EnableAni)
                ani.Play(Animator.StringToHash(idleAniName));
        }
        

        private void AutoFitDirByLocalScale(Vector2 input)
        {
            var localScale = this.transform.localScale;
            var dir = 1;
            if (FitDirByLocalScale && Mathf.Abs(input.x) > 0.01f)
            {
                dir = input.x > 0 ? 1 : -1;
            }
            else
            {
                if (dir != dirBefore)
                    dir = dirBefore;
            }

            dirBefore = dir;
            this.transform.localScale = new Vector3(dir * Mathf.Abs(localScale.x), localScale.y, localScale.z);
        }

        #endregion

        #endregion

        public bool EnableAni => ani != null && ani.enabled;
    }
}
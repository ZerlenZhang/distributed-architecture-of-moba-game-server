using System;
using ReadyGamerOne.Common;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace ReadyGamerOne.Rougelike.Person
{
    /// <summary>
    /// 人物信息接口
    /// </summary>
    public interface IPerson :
        ITakeDamageablePerson<AbstractPerson>,
        IResourcableObject
    {
        /// <summary>
        /// 角色名
        /// </summary>
        string CharacterName { get; }
        
        AbstractPersonController Controller { get; }

        #region 战斗必须

        /// <summary>
        /// 角色是否活着
        /// </summary>
        bool IsAlive { get; }
        
        /// <summary>
        /// 当前生命值
        /// </summary>
        int Hp { get; }
        
        /// <summary>
        /// 最大生命值
        /// </summary>
        int MaxHp { get; }
        
        /// <summary>
        /// 基础攻击力
        /// </summary>
        int Attack { get; }
        
        /// <summary>
        /// 杀死当前角色
        /// </summary>
        void Kill();        

        #endregion
        
        #region Events
        event Action onUpdate;

        #endregion

    }
    
    
    
    /// <summary>
    /// 必须有一个统一的基类来做统一处理
    /// </summary>
    public abstract class AbstractPerson:
        IPerson
    {
        #region Static

        public static event Action<AbstractPerson> OnPersonEnable;
        public static event Action<AbstractPerson> OnPersonDisable;        

        #endregion
        
        #region 内部类，用于方便GameObject拿到PersonSystem引用


        public class PersonIdentity : MonoBehaviour
        {
            public AbstractPerson abstractPerson;

            public event Action onUpdate;

            private void Update()
            {
                onUpdate?.Invoke();
            }
        }        

        #endregion
        
        #region Constructors

        /// <summary>
        /// 构造函数里完成实例化，添加Identity组件，并设置不激活
        /// </summary>
        protected AbstractPerson()
        {
            this.OnInstanciateObject();
        }        

        #endregion

        #region Fields

        protected int _hp;
        protected int _maxHp;
        protected int _attack;
        
        public event Action onUpdate;
        public event Action<AbstractPerson> onBeforeKill;

        protected void TickOnKillEventAndClearEvent()
        {
            Debug.Log("Tick onkill");
            onBeforeKill?.Invoke(this);
            
            onUpdate = null;
            onTakeDamage = null;
            onCauseDamage = null;
            onBeforeKill = null;
        }

        protected GameObject _gameObject;

        private AbstractPersonController _controller;
        
        #endregion
        
        #region ITakeDamageablePerson<T>

        public event Action<AbstractPerson, int> onCauseDamage;
        public event Action<AbstractPerson, int> onTakeDamage;

        public virtual void OnTakeDamage(AbstractPerson takeDamageFrom, int damage)
        {
            Debug.Log($"{CharacterName}收到来自{takeDamageFrom.CharacterName}的{damage}伤害");

            onTakeDamage?.Invoke(takeDamageFrom, damage);
            _hp -= damage;
            if (_hp <= 0)
            {
                Debug.Log(CharacterName+"该死！");
                Kill();
            }
        }

        public virtual void OnCauseDamage(AbstractPerson causeDamageTo, int damage)
        {
            onCauseDamage?.Invoke(causeDamageTo, damage);
//            Debug.Log($"{CharacterName}对{causeDamageTo.CharacterName}造成{damage}伤害");
        }     


        #endregion
        
        #region IPerson
        
        /// <summary>
        /// 角色名字
        /// </summary>
        public string CharacterName
        {
            get
            {
                if (!_gameObject)
                    return "物体被销毁";
                return _gameObject.name;
            }
        }

        public AbstractPersonController Controller => _controller;

        
        #region IResourcableObject
        
        #region IUnityGameObject

        /// <summary>
        /// 本类对应GameObject的索引
        /// </summary>
        public GameObject gameObject => _gameObject;
        
        /// <summary>
        /// 获取物体Transform
        /// </summary>
        public Transform transform => gameObject.transform;
        
        /// <summary>
        /// 获取和设置物体坐标
        /// </summary>
        public Vector3 position
        {
            get
            {
                Assert.IsNotNull(transform);
                return transform.position;
            }
            set
            {
                Assert.IsNotNull(transform);
                transform.position = value;
            }
        }


        public Vector3 localPosition 
        {
            get
            {
                Assert.IsNotNull(transform);
                return transform.localPosition;
            }
            set
            {
                Assert.IsNotNull(transform);
                transform.localPosition = value;
            }
        }

        #endregion

        /// <summary>
        /// 资预制体路径
        /// </summary>
        public abstract string ResPath { get; }

        /// <summary>
        /// 实例化角色预制体时创建
        /// </summary>
        public virtual void OnInstanciateObject()
        {            
            _gameObject = ResourceMgr.InstantiateGameObject(ResPath);

            gameObject.name = GetType().Name;

            //自动添加PersonIdentity
            
            var id = gameObject.GetOrAddComponent<PersonIdentity>();
            id.abstractPerson = this;
            id.onUpdate += Update;
            gameObject.SetActive(false);

            //反射添加人物控制脚本
            var controllerTypeAttribute = GetType().GetAttribute<UsePersonControllerAttribute>();
            if (controllerTypeAttribute != null)
            {
                _controller = (AbstractPersonController)gameObject.GetOrAddComponent(controllerTypeAttribute.controllerType);

//                Debug.Log(controllerTypeAttribute.controllerType);

                _controller.InitController(this);
            }

            Assert.IsTrue(_controller);
        }             
        
        /// <summary>
        /// 真正销毁物体
        /// </summary>
        public void DestroyObject()
        {
            Object.Destroy(gameObject);
        }

        /// <summary>
        /// 启动角色
        /// </summary>
        public virtual void EnableObject()
        {
            if(gameObject==null)
                OnInstanciateObject();
            OnPersonEnable?.Invoke(this);
            gameObject.SetActive(true);
            onBeforeKill = null;
        }

        /// <summary>
        /// 关闭角色
        /// </summary>
        public virtual void DisableObject()
        {
            OnPersonDisable?.Invoke(this);
            if (gameObject)
                gameObject.SetActive(false);
        }
        
        #endregion

        #region 战斗必须
        
        public virtual int Hp => _hp;
        public virtual int MaxHp => _maxHp;
        public virtual int Attack => _attack;

        /// <summary>
        /// 是否活着
        /// </summary>
        public virtual bool IsAlive => gameObject != null && _hp > 0;
        
        /// <summary>
        /// 调用此函数杀死角色
        /// </summary>
        public virtual void Kill()
        {
            //处理消息
            TickOnKillEventAndClearEvent();
            //这里就直接销毁物体
            DestroyObject();
        }            

        #endregion
        
        #endregion
        
        #region Functions

        /// <summary>
        /// 更新函数，角色活着并且激活的时候每帧调用
        /// </summary>
        protected virtual void Update()
        {
            this.onUpdate?.Invoke();
        }
   

        #endregion
    }
}
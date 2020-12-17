using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GlobalVar = ReadyGamerOne.Global.GlobalVar;

namespace ReadyGamerOne.Script
{
    public class Joystick : MonoBehaviour,IDragHandler,IEndDragHandler
    {
        public Transform EnableRect;
        public Transform stick;
        public float maxR;
        public bool showGizmosR = false;
        private bool dragging = false;
        private Vector3 initPosition;

        private PointerEventData _currentPointerData;

        /// <summary>
        /// 触摸方向
        /// </summary>
        private Vector2 touchDir;
        public Vector2 TouchDir => touchDir;

        private void LogicUpdate()
        {
            if (this == null || this.gameObject == null)
                MainLoop.Instance.RemoveFixedUpdateFunc(LogicUpdate);
            
            if (!dragging)
            {
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W) ||
                    Input.GetKeyDown(KeyCode.S))
                {
                    if(EnableRect)
                        this.gameObject.SetActive(true);
                }
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) ||
                    Input.GetKey(KeyCode.S))
                {
                    if(!this.gameObject.activeSelf && EnableRect)
                        this.gameObject.SetActive(true);
                    
                    var simulatorInput=new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                    Simulator(simulatorInput*maxR);
                }
                else
                {
                    if(EnableRect)
                        this.gameObject.SetActive(false);
                    Simulator(Vector2.zero);
                }
            }
        }

        private void Start()
        {
            this.initPosition = transform.position;
            this.stick.localPosition=Vector3.zero;

            MainLoop.Instance.AddFixedUpdateFunc(LogicUpdate);

            if (EnableRect)
            {
                stick.GetComponent<Image>().raycastTarget = false;
                var uiinputer = EnableRect.GetOrAddComponent<UIInputer>();
                uiinputer.eventOnBeginDrag+=evt=>SetJoyStickState(true, evt);
                uiinputer.eventOnDrag += evt => _currentPointerData = evt;
                uiinputer.eventOnEndDrag += evt => SetJoyStickState(false, evt);
                this.gameObject.SetActive(false);
            }
        }

        private void SetJoyStickState(bool enable,PointerEventData data)
        {
            _currentPointerData = data;
            if (enable)
            {
                dragging = true;
                transform.position = data.position;
                if(EnableRect)
                    this.gameObject.SetActive(true);
                MainLoop.Instance.AddFixedUpdateFunc(InternalDrag);
            }
            else
            {
                dragging = false;
                if(EnableRect)
                    this.gameObject.SetActive(false);
                transform.position = initPosition;
                MainLoop.Instance.RemoveFixedUpdateFunc(InternalDrag);
                OnEndDrag(data);
            }
        }


        private void InternalDrag()
        {
            OnDrag(_currentPointerData);
        }


        private void Simulator(Vector2 input)
        {
            var len = input.magnitude;
            
            if (len > maxR)
            {
                input = input.normalized * maxR;
            }

            this.touchDir = new Vector2(input.x / maxR, input.y / maxR);
            
            this.stick.localPosition = input;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Assert.IsNotNull(GlobalVar.CanvasComponent);
            dragging = true;
            Vector2 pos=Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                this.transform as RectTransform,
                eventData.position,
                GlobalVar.CanvasComponent.worldCamera, out pos);

            Simulator(pos);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            dragging = false;
            Simulator(Vector2.zero);
        }

        private void OnDrawGizmos()
        {
            if(showGizmosR)
                Gizmos.DrawWireSphere(transform.position,maxR);
        }
    }
}

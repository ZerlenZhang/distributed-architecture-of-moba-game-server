using ReadyGamerOne.Script;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ReadyGamerOne.Scripts
{
    public class SuperBloodBar : MonoBehaviour
    {

        #region Transition

        [Range(0,1f)] public float transitionTimeScaler=1.0f;
        public Image transitionImage;
        [SerializeField] private Color transitionColor;

        private Coroutine coroutine;
        
        public Color TransitionColor
        {
            set
            {
                if (transitionImage == null)
                {
                    Debug.LogError("transitionImage is null");
                    return;
                }
                transitionImage.color = value;
            }
            get
            {

                if (transitionImage == null)
                {
                    Debug.LogError("transitionImage is null");
                    return Color.black;
                }

                return transitionImage.color;

            }
        }
        private RectTransform TransitionRect
        {
            get
            {
                if (transitionImage == null)
                {
                    Debug.LogError("TransitionImage is null");
                    return null;
                }
                return transitionImage.GetComponent<RectTransform>();
            }
        }        

        #endregion


        #region Fill

        public Image fillImage;
        [SerializeField] private Color fillColor;

        public Color FillColor
        {
            get
            {
                if (fillImage == null)
                {
                    Debug.LogError("FillImage is null");
                    return Color.black;
                }

                return fillImage.color;
            }
            set
            {
                if (fillImage == null)
                {
                    
                    Debug.LogError("FillImage is null");
                    return;
                }

                fillImage.color = value;
            }
        }
        private RectTransform FillRect
        {
            get
            {
                if (fillImage == null)
                {
                    Debug.LogError("fillImage is null");
                    return null;
                }

                return fillImage.GetComponent<RectTransform>();
            }
        }        

        #endregion




        #region BackGround

        public Image backGroundImage;
        [SerializeField] private Color backGroundColor;

        public Color BackGroundColor
        {
            get
            {
                if (backGroundImage == null)
                {
                    Debug.LogError("BackGroundImage is null");
                    return Color.black;
                }

                return backGroundImage.color;
            }
            set
            {
                if (backGroundImage == null)
                {
                    Debug.LogError("BackGroundImage is null");
                    return ;
                }

                backGroundImage.color = value;
            }
        }

        private Vector3[] corners=new Vector3[4];
        private float SizeDeltaX
        {
            get
            {
                if (backGroundImage == null)
                {
                    Debug.LogError("backGroundImage is null");
                    return -1;
                }
                backGroundImage.GetComponent<RectTransform>().GetLocalCorners(corners);
                return (corners[2].x - corners[1].x);
            }
        }        

        #endregion
        
        
        

        
        

        public float maxValue = 100.0f;
        [SerializeField] private float value;
        private float transitionValue;
        private float timer = 0;

        /// <summary>
        /// 实际值
        /// </summary>
        public float Value
        {
            get { return value; }
            set
            {
                
                ValueChangeTransition(this.value, value);
                
                
                this.value = value;

                var fillRect = FillRect;
                if (fillRect == null)
                    return;
                fillRect.sizeDelta=new Vector2(
                    -(1-this.value/maxValue) *Mathf.Abs(SizeDeltaX),
                    fillRect.sizeDelta.y);


                onValueChange?.Invoke(value);
            }
        }

        public UnityEvent<float> onValueChange;
        
        /// <summary>
        /// 过渡值
        /// </summary>
        private float TransitionValue
        {
            get { return transitionValue; }
            set
            {
                if (this == null)
                    return ;
                transitionValue = value;
                
                var transitionRect = TransitionRect;
                transitionRect.sizeDelta = new Vector2(
                    -(1-this.transitionValue/maxValue) * Mathf.Abs(SizeDeltaX),
                    transitionRect.sizeDelta.y);

            }
        }

        private float speed;


        protected void Start()
        {
            TransitionValue = value;
            Value = value;
        }

        public void InitValue(float value)
        {
            this.maxValue = value;
            this.Value = value;
        }
        
        private void ValueChangeTransition(float oldValue, float newValue)
        {
            if(!Application.isPlaying)
                return;
//            Debug.Log("??????");
            timer = 0;
            if (coroutine != null)
                MainLoop.Instance.StopCoroutine(coroutine);
            coroutine= MainLoop.Instance.UpdateForSeconds(() =>
            {
                timer += Time.deltaTime;
                TransitionValue =
                    Mathf.SmoothDamp(TransitionValue, newValue, ref speed,
                        transitionTimeScaler*0.2f); //(oldValue, newValue, timer / transitionTime);
            }, transitionTimeScaler);
        }

    }
}
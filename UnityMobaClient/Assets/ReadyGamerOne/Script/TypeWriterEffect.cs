using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyGamerOne.Script
{
    /// <summary>
    /// 打字机效果
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class TypeWriterEffect : MonoBehaviour
    {
        private Text thisText;

        private string fullWords;

        private Coroutine writerCoroutine;

        private Func<bool> pushFunc;

        private event Action onFinally;

        private void Update()
        {
            if (writerCoroutine != null)
            {
                if (pushFunc != null && pushFunc())
                {
                    print("打字机被打断");
                    StopCoroutine(writerCoroutine);
                    this.thisText.text = this.fullWords;
                    Finally();
                }
            }
        }

        private void Init()
        {
            if (thisText == null)
                this.thisText = GetComponent<Text>();
            if (string.IsNullOrEmpty(fullWords))
            {
                this.fullWords = thisText.text;
            }

            this.thisText.text = "";
        }


        public void StartWriter( float timeToEnd, Action endCall = null, Func<bool> pushFunc = null,
            bool affectedByTimeScale = true)
        {
            this.pushFunc = pushFunc;
            onFinally += endCall;
            Init();
            this.writerCoroutine = StartCoroutine(TypeWriter(this.fullWords.Length / timeToEnd, affectedByTimeScale));
        }

        private IEnumerator TypeWriter(float charPerSecond, bool affectedByTimeScale = true)
        {
            for (int i = 0; i < this.fullWords.Length; i++)
            {
                this.thisText.text = this.fullWords.Substring(0, i + 1);
                if (affectedByTimeScale)
                    yield return new WaitForSeconds(1 / charPerSecond);
                else
                {
                    for (var j = 0; j < 30 / charPerSecond; j++)
                        yield return 0;
                }
            }

            Finally();
        }

        private void Finally()
        {
            this.writerCoroutine = null;
            this.fullWords = null;
            onFinally?.Invoke();
            onFinally = null;
        }
    }
}
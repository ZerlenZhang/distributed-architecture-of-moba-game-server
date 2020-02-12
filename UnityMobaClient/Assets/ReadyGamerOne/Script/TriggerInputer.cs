using System;
using UnityEngine;

namespace ReadyGamerOne.Script
{
    public class TriggerInputer : MonoBehaviour
    {
        public event Action<Collider> onTriggerEnterEvent; 
        public event Action<Collider> onTriggerStayEvent; 
        public event Action<Collider> onTriggerExitEvent; 

        public event Action<Collider2D> onTriggerEnterEvent2D; //如果TriggerEnter会调用这个event
        public event Action<Collider2D> onTriggerStayEvent2D; //如果TriggerStay会调用这个event
        public event Action<Collider2D> onTriggerExitEvent2D; //同理

        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnterEvent?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            onTriggerStayEvent?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            onTriggerExitEvent?.Invoke(other);
        }


        private void OnTriggerEnter2D(Collider2D col)
        {
            onTriggerEnterEvent2D?.Invoke(col);
        }


        private void OnTriggerStay2D(Collider2D col)
        {
            onTriggerStayEvent2D?.Invoke(col);
        }


        private void OnTriggerExit2D(Collider2D col)
        {
            onTriggerExitEvent2D?.Invoke(col);
        }


        public void Clear()
        {
            onTriggerEnterEvent = null;
            onTriggerStayEvent= null;
            onTriggerExitEvent= null;
            onTriggerEnterEvent2D= null;
            onTriggerStayEvent2D= null;
            onTriggerExitEvent2D= null;
            
        }
    }
}

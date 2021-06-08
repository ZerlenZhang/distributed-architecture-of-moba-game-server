using System;
using ReadyGamerOne.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ReadyGamerOne.Rougelike.Item
{
    public abstract class Slot<T,DataType> : 
        MonoBehaviour, 
        IPointerEnterHandler, 
        IPointerExitHandler,
        IPointerClickHandler
    where T:Slot<T,DataType>
    where DataType :CsvMgr
    {
        public DataType ItemData;
        public Image icon;
        public Text countText;
        public event Action<T> onPointerEnter;
        public event Action<T> onPointerExit;
        public event Action<T> onPointerClick;

        private Action<string> onDelete;

        public int Count => int.Parse(countText.text);
        
        public void Init(string itemId, int count = 1,Action<string> ondelete=null)
        {
            this.onDelete = ondelete;
            
            ItemData = CsvMgr.GetData<DataType>(itemId);
            
            countText.text = count.ToString();
            
            UseItemData();
        }


        protected abstract void UseItemData();

        public void Add(int count)
        {
            countText.text = (Count + count).ToString();
        }

        public int Remove(int amount)
        {
            var count = Count;
            if (count <= amount)
            {
                onDelete?.Invoke(ItemData.ID);
                Destroy(gameObject);
                return count;
            }
            
            countText.text = (Count - amount).ToString();

            return amount;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnter?.Invoke(this as T);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit?.Invoke(this as T);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onPointerClick?.Invoke(this as T);
        }
    }
}
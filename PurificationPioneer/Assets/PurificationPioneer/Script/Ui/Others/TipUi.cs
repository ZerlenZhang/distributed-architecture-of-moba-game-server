using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    public class TipUi:MonoBehaviour
    {
        public Text m_TipText;
        public CanvasGroup m_CanvasGroup;
        public void Show(string tip)
        {
            m_TipText.text = tip;
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
        }
    }
}
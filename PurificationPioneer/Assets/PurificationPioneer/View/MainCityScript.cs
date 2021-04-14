using System;
using PurificationPioneer.Script;
using PurificationPioneer.Scriptable;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.View
{
    public class MainCityScript : MonoBehaviour
    {
        public MainCityOptionUi battleOptionUi;
        public Button optionBtn;


        private void Update()
        {
            if (Input.GetKeyDown(GameSettings.Instance.BattleOptionKey))
            {
                battleOptionUi.SetVisible(true);
            }
        }

        public void SetActive(bool state)
        {
            optionBtn.interactable = state;
            if (!state && battleOptionUi.gameObject.activeSelf)
            {
                battleOptionUi.SetVisible(false);
            }
        }

        private void OnEnable()
        {
            DialogSystem.Scripts.DialogSystem.onStartDialog += OnStartDialog;
            DialogSystem.Scripts.DialogSystem.onEndDialog += OnEndDialog;
        }

        private void OnDisable()
        {
            DialogSystem.Scripts.DialogSystem.onStartDialog -= OnStartDialog;
            DialogSystem.Scripts.DialogSystem.onEndDialog -= OnEndDialog;
        }


        private void OnEndDialog()
        {
            SetActive(true);
        }

        private void OnStartDialog()
        {
            SetActive(false);
        }
    }
}
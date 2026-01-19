using System.Collections.Generic;
using TMPro;
using UnityEngine;



namespace kfutils.rpg.ui {

    public class TabbedPanel : MonoBehaviour
    {
        [SerializeField] GameObject tabButtonPrefab;
        [SerializeField] TabbedSubpanel[] subpanels;
        [SerializeField] GameObject buttonPanel;
        [SerializeField] GameObject contentPanel;
        [SerializeField] TMP_Text currentTabLabel;


        private TabButton[] tabButtons;
        private Dictionary<string, TabbedSubpanel> subpanelMap = null;


        private void OnEnable()
        {
            if(subpanelMap == null) Init();
            ShowSubpanel(subpanels[0]);
        }


        private void Init()
        { 
            if(subpanels.Length > 1) {
                subpanelMap = new();
                tabButtons = new TabButton[subpanels.Length];
                for(int i = 0; i < subpanels.Length; i++)
                {
                    tabButtons[i] = Instantiate(tabButtonPrefab, buttonPanel.transform).GetComponent<TabButton>();
                    tabButtons[i].ButtonText.text = subpanels[i].TabName;
                    tabButtons[i].SetController(this);
                    subpanels[i].SetController(this);
                    subpanelMap.Add(subpanels[i].TabName, subpanels[i]);
                }
            }
            else
            {
                subpanelMap = new();
                for(int i = 0; i < subpanels.Length; i++)
                {
                    subpanels[i].SetController(this);
                    subpanelMap.Add(subpanels[i].TabName, subpanels[i]);
                }
            }
        }


        private void ShowSubpanel(TabbedSubpanel subpanel)
        {
            foreach(TabbedSubpanel sub in subpanels)
            {
                sub.gameObject.SetActive(false);
            }
            RectTransform subRect = subpanel.GetComponent<RectTransform>();
            subRect.anchorMin = new Vector2(0, 0);
            subRect.anchorMax = new Vector2(1, 1);
            subRect.anchoredPosition = Vector2.zero;
            subRect.sizeDelta = Vector2.zero;
            subpanel.gameObject.SetActive(true);
            currentTabLabel.text = subpanel.TabName;
        }


        public void ShowSubpanel(string tabName)
        {
            GameManager.Instance.UI.PlayShortClick();
            for(int i = 0; i < subpanels.Length; i++) subpanels[i].gameObject.SetActive(false);
            ShowSubpanel(subpanelMap[tabName]);
        }



    }

}   

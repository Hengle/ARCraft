﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class ExpandableButton : MonoBehaviour {

    public Button[] choices;
    public GameObject[] icons;

    [System.Serializable]
    public class OnSelectEvent : UnityEvent<int> {}
    public OnSelectEvent OnSelect;

    public int currentChoice = 0;

    private GameObject currentIcon;
    private bool expanded = false;

    void Start() {
        for (int i = 0; i < choices.Length; i++) {
            int k = i;
            choices[i].onClick.AddListener(delegate { SelectAction(k); });
            choices[i].gameObject.SetActive(false);
        }
        currentIcon = Instantiate(icons[currentChoice]);
        currentIcon.transform.SetParent(transform, false);
        GetComponent<Button>().onClick.AddListener(ClickAction);
    }

    public void ClickAction() {
        expanded = !expanded;
        for (int i = 0; i < choices.Length; i++) {
            choices[i].gameObject.SetActive(expanded);
        }
    }

    public void SelectAction(int index) {
        expanded = false;
        for (int i = 0; i < choices.Length; i++) {
            choices[i].gameObject.SetActive(false);
        }
        currentChoice = index;
        Destroy(currentIcon);
        currentIcon = Instantiate(icons[currentChoice]);
        currentIcon.transform.SetParent(transform, false);
        OnSelect.Invoke(index);
    }

}
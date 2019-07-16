using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSelectFlagWidget : MonoBehaviour, IPointerClickHandler {
    public Text label;
    public GameObject selectGO;
    public GameObject flagGO;

    public int index { get; private set; }

    public bool isFlagged { get { return flagGO.activeSelf; } set { flagGO.SetActive(value); } }

    public bool isSelected { get { return selectGO.activeSelf; } set { selectGO.SetActive(value); } }

    public string text { get { return label.text; } set { label.text = value; } }

    public event System.Action<int> clickCallback;

    public void Setup(int ind) {
        index = ind;
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
        clickCallback?.Invoke(index);
    }
}

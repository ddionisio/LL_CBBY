using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractiveObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    [Header("Data")]
    public InteractiveMode mode; //which mode to be active

    [Header("Display")]
    public GameObject activeGO;
    public GameObject highlightGO;
    public MaterialHighlight highlightControl;

    [Header("FSM")]
    public PlayMakerFSM fsm;
    public string fsmEventClick = "Click";

    public bool isActive { get { return mode == GameData.instance.currentInteractMode; } }

    private Collider mColl;

    void OnEnable() {
        ApplyActive();

        GameData.instance.interactModeChanged += OnModeChanged;
    }

    void OnDisable() {
        SetHighlight(false);

        GameData.instance.interactModeChanged -= OnModeChanged;
    }

    void Awake() {
        mColl = GetComponent<Collider>();
    }

    void OnModeChanged() {
        ApplyActive();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
        if(isActive)
            SetHighlight(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
        if(isActive)
            SetHighlight(false);
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
        if(fsm)
            fsm.SendEvent(fsmEventClick);
    }

    private void SetHighlight(bool isHighlight) {
        if(highlightControl)
            highlightControl.SetActive(isHighlight);
    }

    private void ApplyActive() {
        var _isActive = isActive;

        mColl.enabled = _isActive;

        if(activeGO) activeGO.SetActive(_isActive);

        if(!_isActive)
            SetHighlight(false);

        /*
        if(isActive) {
            
        }
        else {
            
        }*/
    }
}

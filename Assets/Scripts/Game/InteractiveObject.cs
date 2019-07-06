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
    public bool isLocked {
        get { return mIsLocked; }
        set {
            if(mIsLocked != value) {
                mIsLocked = value;
                ApplyActive();
            }
        }
    }

    private Collider mColl;
    private bool mIsLocked;

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
        if(isActive && !mIsLocked)
            SetHighlight(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
        if(isActive)
            SetHighlight(false);
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
        if(!isActive || mIsLocked)
            return;

        if(fsm)
            fsm.SendEvent(fsmEventClick);
    }

    private void SetHighlight(bool isHighlight) {
        if(highlightControl)
            highlightControl.isActive = isHighlight;
    }

    private void ApplyActive() {
        var _isActive = isActive;

        mColl.enabled = _isActive && !mIsLocked;

        if(activeGO) activeGO.SetActive(_isActive && !mIsLocked);

        if(!_isActive || mIsLocked)
            SetHighlight(false);

        /*
        if(isActive) {
            
        }
        else {
            
        }*/
    }
}

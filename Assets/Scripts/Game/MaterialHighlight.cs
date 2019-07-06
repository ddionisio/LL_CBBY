using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialHighlight : MonoBehaviour {
    public Renderer[] targets;

    public Material material;

    public bool isActive {
        get { return mIsActive; }
        set {
            if(mIsActive != value) {
                mIsActive = value;
                ApplyActive();
            }
        }
    }

    private Material[] mDefaultMats; //corresponds to targets

    private bool mIsActive;

    void OnDisable() {
        mIsActive = false;
        ApplyActive();
    }

    void Awake() {
        mDefaultMats = new Material[targets.Length];
        for(int i = 0; i < targets.Length; i++)
            mDefaultMats[i] = targets[i].sharedMaterial;
    }

    private void ApplyActive() {
        for(int i = 0; i < targets.Length; i++) {
            if(targets[i]) {
                targets[i].sharedMaterial = mIsActive ? material : mDefaultMats[i];
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialHighlight : MonoBehaviour {
    public GameObject target;

    public string varName;
    public float valActive = 1f;
    public float valInactive = 0f;

    private class Item {
        public Material[] materials;

        public Item(Renderer r) {
            materials = r.materials;
        }

        public void Apply(int varId, float val) {
            for(int i = 0; i < materials.Length; i++) {
                var mat = materials[i];
                if(mat)
                    mat.SetFloat(varId, val);
            }
        }

        public void Deinit() {
            if(materials != null) {
                for(int i = 0; i < materials.Length; i++) {
                    if(materials[i])
                        DestroyImmediate(materials[i]);
                }
            }
        }
    }

    private int mVarId;
    private Item[] mItems;

    public void SetActive(bool aActive) {
        if(mItems != null) {
            var val = aActive ? valActive : valInactive;

            for(int i = 0; i < mItems.Length; i++) {
                var itm = mItems[i];
                itm.Apply(mVarId, val);
            }
        }
    }

    void OnDestroy() {
        if(mItems != null) {
            for(int i = 0; i < mItems.Length; i++) {
                var itm = mItems[i];
                itm.Deinit();
            }
        }
    }

    void Awake() {
        mVarId = Shader.PropertyToID(varName);

        var meshRenders = target.GetComponentsInChildren<MeshRenderer>();

        mItems = new Item[meshRenders.Length];

        for(int i = 0; i < mItems.Length; i++) {
            var r = meshRenders[i];

            var itm = new Item(r);
            mItems[i] = itm;
        }
    }
}

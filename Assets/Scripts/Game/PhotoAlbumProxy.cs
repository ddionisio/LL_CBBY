using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoAlbumProxy : MonoBehaviour {
    public string modal = "photoAlbum";
    public bool isSpecial;
    public bool isAll;

    private M8.GenericParams mParms = new M8.GenericParams();

    public void Invoke() {
        mParms[PhotoAlbumModal.parmIsSpecial] = isSpecial;

        M8.ModalManager.main.Open(modal, mParms);
    }
}

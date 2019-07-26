using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoAlbumItemWidget : MonoBehaviour {
    public RawImage image;

    public void Apply(Texture txt) {
        if(txt) {
            image.texture = txt;
            image.gameObject.SetActive(true);
        }
        else
            image.gameObject.SetActive(false);
    }
}

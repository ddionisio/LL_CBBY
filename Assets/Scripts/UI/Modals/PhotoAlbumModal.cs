using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoAlbumModal : M8.ModalController, M8.IModalPush {
    public const string parmIsSpecial = "special";
    public const string parmIsAll = "all";

    [Header("UI")]
    public PhotoAlbumItemWidget templateItem;
        
    public Transform contentRoot;
    public ScrollRect scroller;

    private PhotoAlbumItemWidget[] mImages; //screenshot count + special screenshot

    void M8.IModalPush.Push(M8.GenericParams parms) {
        bool isSpecial = false;
        bool isAll = false;

        if(parms != null) {
            if(parms.ContainsKey(parmIsSpecial))
                isSpecial = parms.GetValue<bool>(parmIsSpecial);

            if(parms.ContainsKey(parmIsAll))
                isAll = parms.GetValue<bool>(parmIsAll);
        }

        //allocate album
        if(mImages == null) {
            mImages = new PhotoAlbumItemWidget[GameData.instance.captureCount + 1];

            for(int i = 0; i < mImages.Length; i++) {
                var img = Instantiate(templateItem);

                img.transform.SetParent(contentRoot, false);

                img.Apply(null);

                img.gameObject.SetActive(true);

                mImages[i] = img;
            }
        }

        //setup captured images
        var captures = GameData.instance.captureInfos;
        for(int i = 0; i < captures.Length; i++) {
            mImages[i].Apply(captures[i].texture);
        }

        if(isSpecial || isAll) {
            mImages[mImages.Length - 1].Apply(GameData.instance.captureScreenTexture);
            mImages[mImages.Length - 1].gameObject.SetActive(true);
        }
        else {
            mImages[mImages.Length - 1].gameObject.SetActive(false);
        }

        if(isSpecial)
            scroller.normalizedPosition = new Vector2(1f, 1f);
        else
            scroller.normalizedPosition = new Vector2(0f, 1f);
    }

    void Awake() {
        templateItem.gameObject.SetActive(false);
    }
}

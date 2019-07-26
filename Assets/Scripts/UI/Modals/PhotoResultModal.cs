using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoResultModal : M8.ModalController, M8.IModalPush {
    [Header("UI")]
    public PhotoAlbumItemWidget templateItem;

    public Transform contentRoot;

    public Text percentText;
    public Text scoreText;

    private PhotoAlbumItemWidget[] mImages;

    void M8.IModalPush.Push(M8.GenericParams parms) {
        //allocate album
        if(mImages == null) {
            mImages = new PhotoAlbumItemWidget[GameData.instance.captureCount];

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

        //compute and update capture score
        GameData.instance.UpdateCaptureScore();

        percentText.text = Mathf.RoundToInt(GameData.instance.capturePercent * 100f).ToString() + '%';
        scoreText.text = GameData.instance.captureScore.ToString();
    }

    void Awake() {
        templateItem.gameObject.SetActive(false);
    }
}

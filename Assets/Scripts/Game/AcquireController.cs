using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcquireController : MonoBehaviour {
    public enum Type {
        Monitor,
        Mouse,
        Desktop,
        Keyboard,

        Stapler,
        DeskFan,
        Photo
    }

    [System.Serializable]
    public class DeviceItem {
        public AcquisitionItemData[] acquisitionItems;
        public GameObject displayGO;
    }

    public GameObject rootGO;

    [Header("Acquisitions")]
    public DeviceItem[] acquisitions; //corresponds to Type

    [Header("Animation")]
    public M8.Animator.Animate animator;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeEnter;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeExit;

    [Header("Modal")]
    public string modalAcquire = "acquire";

    public bool isAcquiring { get { return mRout != null; } }

    private Coroutine mRout;

    private M8.GenericParams mModalParms = new M8.GenericParams();

    public void Acquire(Type type) {
        int ind = (int)type;
        if(ind >= 0 && ind < acquisitions.Length) {
            var itm = acquisitions[ind];

            for(int i = 0; i < itm.acquisitionItems.Length; i++)
                GameData.instance.AcquireDevice(itm.acquisitionItems[i]);

            mRout = StartCoroutine(DoAcquire(itm));
        }
    }

    void OnEnable() {
        if(rootGO) rootGO.SetActive(false);

        for(int i = 0; i < acquisitions.Length; i++) {
            var itm = acquisitions[i];

            if(itm.displayGO)
                itm.displayGO.SetActive(false);
        }
    }

    void OnDisable() {
        mRout = null;
    }

    IEnumerator DoAcquire(DeviceItem item) {
        HUD.instance.Hide();

        if(rootGO) rootGO.SetActive(true);

        if(item.displayGO)
            item.displayGO.SetActive(true);

        if(animator && !string.IsNullOrEmpty(takeEnter))
            yield return animator.PlayWait(takeEnter);

        //show modal
        mModalParms[AcquisitionModal.parmItems] = item.acquisitionItems;

        M8.ModalManager.main.Open(modalAcquire, mModalParms);

        //wait for modal to close
        while(M8.ModalManager.main.isBusy || M8.ModalManager.main.IsInStack(modalAcquire))
            yield return null;

        if(animator && !string.IsNullOrEmpty(takeExit))
            yield return animator.PlayWait(takeExit);

        if(item.displayGO)
            item.displayGO.SetActive(false);

        if(rootGO) rootGO.SetActive(false);

        HUD.instance.Show();

        mRout = null;
    }
}

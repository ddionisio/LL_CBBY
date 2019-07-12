using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageCloneWidget : MonoBehaviour {
    [Header("Data")]
    public ImageDriveData imageData;
    [M8.Localize]
    public string sourceTitleRef;
    [M8.Localize]
    public string progressTitleRef;
    public string modal = "progress";
    public float delay = 2f;

    [Header("UI")]
    public Text sourceNameText;
    public Text imageFilenameText;
    public Button proceedButton;
    public GameObject completeGO;

    [Header("Signal Invoke")]
    public M8.Signal signalInvokeProceed;

    private M8.GenericParams mModalParms = new M8.GenericParams();

    void Awake() {
        sourceNameText.text = M8.Localize.Get(sourceTitleRef);
        imageFilenameText.text = imageData.filename;

        completeGO.SetActive(false);

        proceedButton.onClick.AddListener(OnProceedClick);
    }

    private void OnProceedClick() {
        proceedButton.interactable = false;

        StartCoroutine(DoProceed());
    }

    IEnumerator DoProceed() {
        mModalParms[ProgressModal.parmTitleString] = string.Format(M8.Localize.Get(progressTitleRef), M8.Localize.Get(sourceTitleRef), imageData.filename);
        mModalParms[ProgressModal.parmDelay] = delay;

        M8.ModalManager.main.Open(modal, mModalParms);

        while(M8.ModalManager.main.isBusy || M8.ModalManager.main.IsInStack(modal))
            yield return null;

        completeGO.SetActive(true);

        signalInvokeProceed.Invoke();
    }
}

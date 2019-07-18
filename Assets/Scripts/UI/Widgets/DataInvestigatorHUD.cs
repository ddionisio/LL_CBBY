using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInvestigatorHUD : MonoBehaviour {
    [Header("File Inspect")]
    [M8.Localize]
    public string fileInspectSearchTitleRef;
    
    public void FileInspectClick() {
        StartCoroutine(DoFileInspect());
    }

    IEnumerator DoFileInspect() {
        yield return null;
    }
}

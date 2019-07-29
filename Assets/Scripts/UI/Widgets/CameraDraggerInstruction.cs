using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDraggerInstruction : MonoBehaviour {
    public DragToGuideWidget dragGuide;
    public Transform pt1;
    public Transform pt2;

    void OnEnable() {
        StartCoroutine(DoAction());
    }

    IEnumerator DoAction() {
        while(true) {
            yield return DoShow(pt1, pt2);

            yield return DoShow(pt2, pt1);
        }
    }

    IEnumerator DoShow(Transform aPt1, Transform aPt2) {
        dragGuide.Show(false, aPt1.position, aPt2.position);

        var delay = dragGuide.cursorFadeDelay * 5f + dragGuide.cursorFadeDelay * 2f + dragGuide.cursorMoveDelay;

        yield return new WaitForSeconds(delay);
    }
}

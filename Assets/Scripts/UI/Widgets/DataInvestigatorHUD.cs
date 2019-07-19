using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInvestigatorHUD : MonoBehaviour {
    [Header("Search")]
    public string searchModal = "search";
    public string searchResultModal = "searchResult";
    public float searchProgressDelay = 0.3f;
    [M8.Localize]
    public string searchProgressTitleFormatRef;

    public string progressModal = "progress";

    [Header("File Inspect")]
    [M8.Localize]
    public string fileInspectSearchTitleRef;
    public string fileInspectModal = "fileInspector";

    [Header("Network Log Inspect")]
    public string networkLogModal = "networkLog";

    private SearchKeywordData mSearchKeyword;
    private int mSearchResultIndex;

    private M8.GenericParams mSearchModalParms = new M8.GenericParams();
    private M8.GenericParams mSearchResultParms = new M8.GenericParams();
    private M8.GenericParams mProgressParms = new M8.GenericParams();
    private M8.GenericParams mFileInspectParms = new M8.GenericParams();

    public void FileInspectClick() {
        StartCoroutine(DoFileInspect());
    }

    public void NetworkLogInspectClick() {
        StartCoroutine(DoNetworkLogInspect());
    }

    IEnumerator DoFileInspect() {
        //some dialog

        //open search
        mSearchKeyword = null;

        mSearchModalParms[SearchModal.parmTitle] = M8.Localize.Get(fileInspectSearchTitleRef);
        mSearchModalParms[SearchModal.parmCallback] = (SearchModal.CommitCallback)OnSearchProceed;

        M8.ModalManager.main.Open(searchModal, mSearchModalParms);

        //wait for search
        while(M8.ModalManager.main.isBusy || M8.ModalManager.main.IsInStack(searchModal))
            yield return null;

        if(mSearchKeyword) {
            //do progress
            yield return StartCoroutine(DoSearchProgress(mSearchKeyword.key, searchProgressDelay));

            //show search result
            mSearchResultIndex = -1;

            mSearchResultParms[SearchResultModal.parmSearchType] = SearchType.File;
            mSearchResultParms[SearchResultModal.parmSearchKeywordData] = mSearchKeyword;
            mSearchResultParms[SearchResultModal.parmProceedCallback] = (SearchResultModal.ProceedCallback)OnSearchResultProceed;

            M8.ModalManager.main.Open(searchResultModal, mSearchResultParms);

            //wait for search result
            while(M8.ModalManager.main.isBusy || M8.ModalManager.main.IsInStack(searchResultModal))
                yield return null;

            if(mSearchResultIndex != -1) {
                //open file inspect modal
                mFileInspectParms[FileInspectModal.parmSearchKeywordData] = mSearchKeyword;
                mFileInspectParms[FileInspectModal.parmSearchKeywordResultIndex] = mSearchResultIndex;

                M8.ModalManager.main.Open(fileInspectModal, mFileInspectParms);

                while(M8.ModalManager.main.isBusy || M8.ModalManager.main.IsInStack(fileInspectModal))
                    yield return null;

                //some dialog
            }

            //return to search
            StartCoroutine(DoFileInspect());
        }
    }

    IEnumerator DoNetworkLogInspect() {
        //some dialog

        yield return null;

        //show modal
        var parms = new M8.GenericParams();
        parms[NetworkLogModal.parmIsInspection] = true;

        M8.ModalManager.main.Open(networkLogModal, parms);

        //dialog
    }

    IEnumerator DoSearchProgress(string title, float delay) {
        mProgressParms[ProgressModal.parmTitleString] = string.Format(M8.Localize.Get(searchProgressTitleFormatRef), title);
        mProgressParms[ProgressModal.parmDelay] = delay;

        M8.ModalManager.main.Open(progressModal, mProgressParms);

        while(M8.ModalManager.main.isBusy || M8.ModalManager.main.IsInStack(progressModal))
            yield return null;
    }

    void OnSearchProceed(SearchKeywordData keyword) {
        mSearchKeyword = keyword;
    }

    void OnSearchResultProceed(int ind) {
        mSearchResultIndex = ind;
    }
}

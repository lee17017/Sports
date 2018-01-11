﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [SerializeField]
    private Transform _lifePanel;

    [SerializeField]
    private GameObject _heartPrefab;

    [SerializeField]
    private GameObject _loadingScreen;

    [SerializeField]
    private GameObject _winScreen;

    [SerializeField]
    private Text _gameReadyTextBox;

    public void SetupUI(int maxLife) {
        //setup life display
        for(int i=0; i<maxLife; i++) {
            GameObject tmp = (GameObject)Instantiate(_heartPrefab);
            tmp.transform.SetParent(_lifePanel, false);
        }
        _loadingScreen.SetActive(false);
        _winScreen.SetActive(false);
    }

    public void ActivateLoadingScreen() {
        _loadingScreen.SetActive(true);
    }

    public void DeactivateLoadingScreen() {
        _loadingScreen.SetActive(false);
    }

    public void UpdateLife(int life) {
        Destroy(_lifePanel.GetChild(_lifePanel.childCount - 1));
    }

    public void UpdateGameReadyTime(float time) {
        _gameReadyTextBox.text = time.ToString("F2");
    }

    public void ShowWinScreen() {
        _winScreen.SetActive(true);
    }

}

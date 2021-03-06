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
    private GameObject _menuScreen;

    [SerializeField]
    private GameObject _menuScreenRestart;

    [SerializeField]
    private GameObject _menuScreenNext;

    [SerializeField]
    private Text _gameReadyTextBox;

    [SerializeField]
    private Text _levelTitleTextBox;


    [SerializeField]
    private GameObject _pointer;

    public void SetupUI(int maxLife) {
        //setup life display
        for(int i=0; i<maxLife; i++) {
            GameObject tmp = (GameObject)Instantiate(_heartPrefab);
            tmp.transform.SetParent(_lifePanel, false);
        }
        _loadingScreen.SetActive(false);
        _winScreen.SetActive(false);
        _menuScreen.SetActive(false);
    }

    public void ActivateLoadingScreen() {
        _loadingScreen.SetActive(true);
    }

    public void DeactivateLoadingScreen() {
        _loadingScreen.SetActive(false);
    }

    public void ActivateMenuScreen(bool dieded)
    {
        GameObject temp = Instantiate(_pointer);
        temp.transform.position = new Vector3(0, 0, 20f);

        _menuScreen.SetActive(true);
        if (dieded)
        {
            _menuScreenRestart.SetActive(true);
            _menuScreenNext.SetActive(false);
        }
        else
        {
            _menuScreenNext.SetActive(true);
            _menuScreenRestart.SetActive(false);
        }
    }

    public void UpdateLife(int life) {
        int diff = _lifePanel.childCount - life;
        for (int i = 0; i < diff; i++)
        {
            Destroy(_lifePanel.GetChild(i).gameObject);
        }
    }

    public void UpdateGameReadyTime(float time) {
        _gameReadyTextBox.text = time.ToString("F2");
    }

    public void UpdateLevelTitle(string title) {
        _levelTitleTextBox.text = title;
    }

    public void ShowWinScreen() {
        _winScreen.SetActive(true);
    }

}

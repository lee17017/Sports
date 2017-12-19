using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [SerializeField]
    private Transform _lifePanel;

    [SerializeField]
    private GameObject _heartPrefab;

    public void SetupUI(int maxLife) {
        //setup life display
        for(int i=0; i<maxLife; i++) {
            GameObject tmp = (GameObject)Instantiate(_heartPrefab);
            tmp.transform.SetParent(_lifePanel, false);
        }

    }

    public void UpdateLife(int life) {

    }

}

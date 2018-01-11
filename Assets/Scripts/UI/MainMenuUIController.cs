using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour {

    [SerializeField]
    private Transform _levelSelectionPanel;

    [SerializeField]
    private Button _levelSelectionButton;

    [SerializeField]
    private GameManager _gameManager;

	// Use this for initialization
	void Start () {
        InitializeLevelSelection(_gameManager.MaxLevel, 2);
	}

    //maxLevel = biggest level index, unlockedLevel = biggest unlocked level index
    private void InitializeLevelSelection(int maxLevel, int unlockedLevel) {
        for(int i=1; i<=unlockedLevel; i++) {
            Button temp = Instantiate(_levelSelectionButton, _levelSelectionPanel, false);
            temp.GetComponent<LevelSelectionButton>().Initialize(_gameManager, i);
        }
        for(int i=unlockedLevel+1; i< maxLevel; i++) {
            Button temp = Instantiate(_levelSelectionButton, _levelSelectionPanel, false);
            temp.interactable = false;
        }
    }

}

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
        InitializeLevelSelection(_gameManager.MaxLevel, 3);
	}

    private void InitializeLevelSelection(int maxLevels, int unlockedLevels) {
        for(int i=0; i<unlockedLevels; i++) {
            Button temp = Instantiate(_levelSelectionButton, _levelSelectionPanel, false);
            temp.GetComponent<LevelSelectionButton>().Initialize(_gameManager, i);
        }
        for(int i=unlockedLevels; i< maxLevels; i++) {
            Button temp = Instantiate(_levelSelectionButton, _levelSelectionPanel, false);
            temp.interactable = false;
        }
    }

}

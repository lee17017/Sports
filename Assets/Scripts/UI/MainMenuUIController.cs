using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour {

    [SerializeField]
    private Transform _levelSelectionPanel;

    [SerializeField]
    private Button _levelSelectionButton;

	// Use this for initialization
	void Start () {
        InitializeLevelSelection(GameManager.Instance.MaxLevel, GameManager.Instance.UnlockedLevel);
	}

    //maxLevel = biggest level index, unlockedLevel = biggest unlocked level index
    private void InitializeLevelSelection(int maxLevel, int unlockedLevel) {
        for(int i=1; i<=unlockedLevel; i++) {
            Button temp = Instantiate(_levelSelectionButton, _levelSelectionPanel, false);
            temp.GetComponent<LevelSelectionButton>().Initialize(GameManager.Instance, i);
        }
        for(int i=unlockedLevel+1; i< maxLevel; i++) {
            Button temp = Instantiate(_levelSelectionButton, _levelSelectionPanel, false);
            temp.interactable = false;
        }
    }

}

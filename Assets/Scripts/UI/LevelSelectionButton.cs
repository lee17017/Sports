using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionButton : MonoBehaviour{

    private int _level;
    private GameManager _gameManager;

    public void Initialize(GameManager gameManager, int level) {
        _level = level;
        _gameManager = gameManager;
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick() {
        _gameManager.LoadLevel(_level);
    }
}

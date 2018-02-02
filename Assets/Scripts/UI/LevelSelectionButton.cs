using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionButton : MonoBehaviour{

    [SerializeField]
    private Text _text;

    private int _level;
    private GameManager _gameManager;

    public void Initialize(GameManager gameManager, int level) {
        _level = level;
        _gameManager = gameManager;

        _text.text = "Level " + level;

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick() {
        _gameManager.LoadLevel(_level);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    private const string LevelPrefix = "Level";

    private const float LevelLoadingTime = 2f;

    private static GameManager _instance = null;
    public static GameManager Instance { get { return _instance; } }

    [SerializeField]
    private UIController _uiController;

    [SerializeField]
    private LevelSettings _levelSettings;

    [Header("IMPORTANT: will eventually crash game if not correct")]
    [SerializeField]
    private int _maxLevel = 0;
    public int MaxLevel { get { return _maxLevel; } }

    private int _life;

    private int _currentLevel;

    private bool _isLoadingLevel = false;

    private void Awake() {
        if(_instance == null) {
            _instance = this;
        }else if(_instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        if(_maxLevel == 0) {
            _maxLevel = SceneManager.sceneCountInBuildSettings - 1;
        }
    }

    private void UpdateReferences() {
        //require some scripts and objects in scene
        _uiController = FindObjectOfType<UIController>();
        if (_uiController == null) {
            throw new System.Exception("No UiController in scene! ");
        }

        _levelSettings = FindObjectOfType<LevelSettings>();
        if (_levelSettings == null) {
            throw new System.Exception("No LevelSettings in scene! ");
        }
    }

    //called whenever a level is loaded
    private void InitializeLevel() {
        _life = _levelSettings.Life;
        _uiController.SetupUI(_levelSettings.Life);

        if (FindObjectOfType<Player>()) {
            FindObjectOfType<Player>().Activate();
        } else {
            throw new System.Exception("No Player in scene! ");
        }
    }

    public void OnDamagePlayer(int damage) {
        _life -= damage;
        _uiController.UpdateLife(_life);
        if (_life <= 0) {
            Die();
        }
    }

    public void UpdatePlayerPosition(float x) {
        if(x > _levelSettings.LevelEndX) {
            Win();
        }
    }

    private void Die() {
        Debug.Log("Dieded");
    }

    private void Win() {
        Debug.Log("Win!");
        if(_currentLevel+1 <= _maxLevel) {
            LoadLevel(_currentLevel+1);
        } else {
            Debug.Log("Congratulations! You beat the game!");
            GameFinished();
        }
    }

    private void GameFinished() {
        _uiController.ShowWinScreen();

        if (FindObjectOfType<Player>()) {
            FindObjectOfType<Player>().Deactivate();
        }
    }

    public void LoadLevel(int level) {
        if (!_isLoadingLevel && level <= _maxLevel) {
            _isLoadingLevel = true;

            if(_uiController != null) {
                _uiController.ActivateLoadingScreen();
            }

            StartCoroutine(LoadSceneAsync(level));
        } else if(!_isLoadingLevel) {
            Debug.LogWarning("Already loading a Level, wait until done!");
        } else {
            Debug.LogWarning("Level " + level+ " does not exist!");
        }
    }

    IEnumerator LoadSceneAsync(int scene) {
        Debug.Log("Starting to load " + LevelPrefix + scene);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(LevelPrefix + scene);

        while (!asyncLoad.isDone) {
            yield return null;
        }

        UpdateReferences();

        for (float i = LevelLoadingTime; i > 0; i -= .01f) {
            _uiController.UpdateGameReadyTime(i);
            yield return new WaitForSecondsRealtime(.01f);
        }

        InitializeLevel();

        _isLoadingLevel = false;
        _currentLevel++;
        Debug.Log("Done loading " + LevelPrefix + scene);
        yield return null;
    }

}

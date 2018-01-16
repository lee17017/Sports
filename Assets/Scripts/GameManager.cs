using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [Header("IMPORTANT: will eventually crash game if not correct")]
    [SerializeField]
    private int _maxLevel = 0;
    public int MaxLevel { get { return _maxLevel; } }

    private const string LevelPrefix = "Level";

    private const float LevelLoadingTime = 2f;

    private static GameManager _instance = null;
    public static GameManager Instance { get { return _instance; } }

    private UIController _uiController;
    private LevelSettings _levelSettings;
    private Player _player;

    private int _life;

    private int _currentLevel;

    private bool _isLoadingLevel = false;

    private float[] _checkpoints;
    private int _lastCheckpoint = -1;
    private float _loadWithCheckpoint = -1f;

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

    //called whenever a level is loaded
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

        _player = FindObjectOfType<Player>();
        if (_player == null) {
            throw new System.Exception("No Player in scene! ");
        }
    }

    //called whenever a level is loaded
    private void InitializeLevel() {
        _life = _levelSettings.Life;
        _uiController.SetupUI(_levelSettings.Life);
        _checkpoints = _levelSettings.Checkpoints;

        _player.Activate();
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
        if(_checkpoints.Length > _lastCheckpoint + 1 && x > _checkpoints[_lastCheckpoint + 1]){
            _lastCheckpoint++;
        }
    }

    private void Die() {
        Debug.Log("Dieded");
        if(_lastCheckpoint != -1) {
            _loadWithCheckpoint = _checkpoints[_lastCheckpoint];
        }
        LoadLevel(_currentLevel);
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
        //AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(LevelPrefix + scene);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
        while (!asyncLoad.isDone) {
            yield return null;
        }

        UpdateReferences();

        _uiController.UpdateLevelTitle("Level " + scene);

        if(_loadWithCheckpoint != -1) {
            _player.transform.Translate(new Vector3(_loadWithCheckpoint, 0, 0));
        }

        for (float i = LevelLoadingTime; i > 0; i -= .01f) {
            _uiController.UpdateGameReadyTime(i);
            yield return new WaitForSecondsRealtime(.01f);
        }

        InitializeLevel();

        _lastCheckpoint = -1;
        _loadWithCheckpoint = -1f;

        _isLoadingLevel = false;
        _currentLevel = scene;
        Debug.Log("Done loading " + LevelPrefix + scene);
        yield return null;
    }

}

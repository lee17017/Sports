using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [Header("IMPORTANT: will eventually crash game if not correct")]
    [SerializeField]
    private int _maxLevel = 0;
    public int MaxLevel { get { return _maxLevel; } }

    private int _unlockedLevel = 1;
    public int UnlockedLevel { get { return _unlockedLevel; } }

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

        //From Hendrik:
        //Activating the CameraBox on Camera
        Camera cam = Camera.main;
        for (int i = 0; i < cam.transform.childCount; i++)
        {
            cam.transform.GetChild(i).gameObject.GetComponent<SphereCollider>().enabled = true;
        }
    }

    public void OnDamagePlayer(int damage) {
        _life -= damage;
        _uiController.UpdateLife(System.Math.Max(_life,0));
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
        Time.timeScale = 0;
        _uiController.ActivateMenuScreen(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        if (_lastCheckpoint != -1)
        {
            _loadWithCheckpoint = _checkpoints[_lastCheckpoint];
        }
        LoadLevel(_currentLevel);
    }
    public void LoadNextLevel()
    {
        Time.timeScale = 1;
        LoadLevel(_currentLevel + 1);
    }

    private void Win() {
        Debug.Log("Win!");
        Time.timeScale = 0;
        if (_currentLevel+1 <= _maxLevel) {
            _unlockedLevel++;
            _uiController.ActivateMenuScreen(false);
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

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void LoadLevel(int level) {
        
        if (!_isLoadingLevel && level <= _maxLevel) {
            _isLoadingLevel = true;

            Time.timeScale = 1;

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
        if (_uiController != null)
        {
            _uiController.ActivateLoadingScreen();
        }

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
        GestureHandler.Instance.reset();
        Debug.Log("Done loading " + LevelPrefix + scene);
        yield return null;
    }

    // From Hendrik: Developer TOols for Keyboard (for cheating and such)
    private void LateUpdate()
    {
        for (int i = 0; i < numKeys.Length; i++)
        {
            if (Input.GetKeyDown(numKeys[i]))
            {
                int numberPressed = i;
                LoadLevel(numberPressed);
                break;
            }
        }
    }
    private KeyCode[] numKeys = {
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6,
         KeyCode.Alpha7,
         KeyCode.Alpha8,
         KeyCode.Alpha9,
         KeyCode.Alpha0,
     };
}

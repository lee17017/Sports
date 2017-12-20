using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    private static GameManager _instance = null;
    public static GameManager Instance { get { return _instance; } }

    [SerializeField]
    private UIController _uiController;

    [SerializeField]
    private LevelSettings _levelSettings;

    [Header("IMPORTANT: will eventually crash game if not correct")]
    [SerializeField]
    private int _maxLevel = 0;

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

    //called whenever a level is loaded
    private void OnLevelLoaded() {
        //require some scripts and objects in scene
        _uiController = FindObjectOfType<UIController>();
        if (_uiController == null) {
            throw new System.Exception("No UiController in scene! ");
        }

        _levelSettings = FindObjectOfType<LevelSettings>();
        if(_levelSettings == null) {
            throw new System.Exception("No LevelSettings in scene! ");
        }

        _life = _levelSettings.Life;
        _uiController.SetupUI(_levelSettings.Life);
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

    }

    private void Win() {

    }

    public void LoadLevel(int level) {
        if (!_isLoadingLevel) {
            _isLoadingLevel = true;

            _uiController.ActivateLoadingScreen();

            StartCoroutine(LoadSceneAsync(level));
        } else {
            Debug.LogWarning("Already loading a Level, wait until done!");
        }
    }

    IEnumerator LoadSceneAsync(int scene) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Scene"+scene);

        while (!asyncLoad.isDone) {
            OnLevelLoaded();
            _isLoadingLevel = false;
            yield return null;
        }
    }

}

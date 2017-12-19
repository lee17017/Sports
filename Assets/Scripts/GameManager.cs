using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private static GameManager _instance = null;
    public static GameManager Instance { get { return _instance; } }

    [SerializeField]
    private UIController _uiController;

    [SerializeField]
    private int _maxLife;

    private int _life;

    private void Awake() {
        if(_instance == null) {
            _instance = this;
        }else if(_instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
        _life = _maxLife;

        _uiController.SetupUI(_maxLife);
    }
	
    private void SetupUI() {

    }

    public void OnDamagePlayer(int damage) {
        _life -= damage;
        _uiController.UpdateLife(_life);
        if (_life <= 0) {
            Die();
        }
    }

    private void Die() {

    }

	// Update is called once per frame
	void Update () {
        
    }
}

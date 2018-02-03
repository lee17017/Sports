using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossEnemy : MonoBehaviour
{
    private static bool _collisionHack = false;

    [SerializeField]
    private int _health;
    private int _curHealth; //Wieviele Leben der Gegner hat
    // Wenn -1, ist der Gegner unverwundbar und kann im Moment NIE sterben.

    [SerializeField]
    private float _timeToLive; //Sagt aus, wielange dieser Gegner aktiv überleben kann (0 = Infinity)
    [SerializeField]
    private int _collisionDamage; // Sagt aus, wieviel Schaden der Spieler bei Kontakt erleidet

    private int _score = 0;

    public Text scoreText;

    private float maxY = 4.5f;
    private float minY = -2.5f;

    //------------------------
    [Header("MOVEMENT")]
    #region movementSettings
    [SerializeField]
    private float _movementSpeed;
    [SerializeField]
    private float _updateBeaconPeriod;
    [SerializeField]
    private GameObject _beacon;

    private Vector3 _direction = Vector3.up;
    #endregion

    //------------------------
    [Header("SHOOTING")]
    #region shootingSettings
    [SerializeField]
    private float _gunOffsetFactor;
    [SerializeField]
    private float _gunMinCD; //Kleinster Cooldown zwischen Schüssen
    [SerializeField]
    private float _gunMaxCD; //Größert Cooldown (nimmt random wert zwischen min und max)
    [SerializeField]
    private float _gunStartCD; //CD, wenn der Gegner aktiviert wird
    [SerializeField]
    private float _accuracy; //1 = Zielt genau auf Spieler, <1 und >0 ist ungenauigkeitswahrscheinlichkeit
    [SerializeField]
    private float _projectileSpeed; //0 = double movespeed als geschwindigkeit
    [SerializeField]
    private int _projectileDamage; //0 = double movespeed als geschwindigkeit
    [SerializeField]
    private GameObject _enemyProjectile; // Das Projektil Prefab

    private bool _gunOnCooldown = false;
    #endregion

    //------------------------
    private bool _isActive = false;
    private GameObject _player;

    [SerializeField]
    private GameObject _staticEnemy;
    private bool _spawnTop;

    [Header("BUFFS")]
    [SerializeField]
    private float _maxMove;
    [SerializeField]
    private float _maxMinCD;
    [SerializeField]
    private float _maxMaxCD;
    [SerializeField]
    private float _maxProjectileSpeed;
    private bool _changeDirection = false;
    [Header("Probabilities")]
    [SerializeField]
    private int _buffSpeed;
    [SerializeField]
    private int _buffMinCD;
    [SerializeField]
    private int _buffMaxCD;
    [SerializeField]
    private int _buffProjectile;
    [SerializeField]
    private int _buffSpecial;
    // ----------------------------


    // Use this for initialization
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        _player = GameObject.FindGameObjectWithTag("Player");

    }

    void LookDirection(Vector3 direction)
    {
        float zRot;
        if (direction.x == 0)
            zRot = 90 + (direction.y <= 0 ? 0 : 180f);
        else
            zRot = Mathf.Atan(direction.y / direction.x) / Mathf.PI * 180 + (direction.x <= 0 ? 0 : 180f);

        transform.rotation = Quaternion.Euler(0, 0, zRot);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isActive)
        {
            Move();
            if (!_gunOnCooldown)
            {
                Shoot();
            }
            LookDirection(_player.transform.position - transform.position);
        }
    }

    // When the boss dies, the level end must trigger
    void TriggerWin()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        gameManager.GetComponent<GameManager>().Win();
    }

    public void LooseHealth(int damage)
    {
        if (_curHealth != -1)
        {
            _curHealth -= damage;
            if (_curHealth <= 0)
            {
                powerUP();
                _curHealth = 4;
            }
        }
    }

    // Makes the boss vulnerable and he moves to the other side
    void powerUP()
    {
        _curHealth = _health;
        // Gains a bonus effect:
        int rand = Random.Range(1, 101);

        if (rand <= _buffSpeed)
        {
            rand = 0;
        }
        else if (rand <= _buffSpeed + _buffMinCD)
        {
            rand = 1;
        }
        else if (rand <= _buffSpeed + _buffMinCD + _buffMaxCD)
        {
            rand = 2;
        }
        else if (rand <= _buffSpeed + _buffMinCD + _buffMaxCD + _buffProjectile)
        {
            rand = 3;
        }
        else
        {
            rand = 4;
        }

        switch (rand)
        {
            case 0:
                _movementSpeed += (_maxMove - _movementSpeed) * 0.4f;
                break;
            case 1:
                _gunMinCD += (_maxMinCD - _gunMinCD) * 0.4f;
                break;
            case 2:
                _gunMaxCD += (_maxMaxCD - _gunMaxCD) * 0.4f;
                break;
            case 3:
                _projectileSpeed += (_maxProjectileSpeed - _projectileSpeed) * 0.4f;
                break;
            case 4:
                if (_changeDirection == false)
                {
                    _changeDirection = true;
                }
                else
                {
                    // Spawn Enemies:
                    Vector3 spawnLocation = Vector3.up;
                    Vector3 rotation;
                    if (_spawnTop)
                    {
                        spawnLocation = new Vector3(transform.position.x + 6f, 5.3f, 0);
                        rotation = new Vector3(0, 0, 0);

                    }
                    else
                    {
                        spawnLocation = new Vector3(transform.position.x + 6f, -3.5f, 0);
                        rotation = new Vector3(0, 0, 180);
                    }
                    _spawnTop = !_spawnTop;
                    GameObject spawned = Instantiate(_staticEnemy, spawnLocation, Quaternion.identity);
                    spawned.transform.Rotate(rotation);
                }
                break;
        }

    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "PlayerProjectile")
        {
            LooseHealth(1);
            _score++;
            scoreText.text = "Score: " + _score;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "CameraBox" && !_isActive)
        {
            _isActive = true;
        }
    }

    // Der Bereich, der für die Bewegung verantwortlich ist
    #region Movement
    void Move()
    {
        //Bewegung
        float timeFactor = _movementSpeed * Time.deltaTime;
        Vector3 nextStep = new Vector3(transform.position.x + (_direction.x * timeFactor), transform.position.y + (_direction.y * timeFactor), transform.position.z + (_direction.z * timeFactor));
        transform.position = nextStep;

        if (transform.position.y >= maxY)
        {
            SwitchDirection(2);
        }
        else if (transform.position.y <= minY)
        {
            SwitchDirection(1);
        }
        else if (_changeDirection && Random.Range(0, 999) == 0)
        {
            SwitchDirection(0);
        }
    }

    void SwitchDirection(int switchTo)
    {
        switch (switchTo)
        {
            case 0:
                if (_direction.Equals(Vector3.up))
                {
                    _direction = Vector3.down;
                }
                else
                {
                    _direction = Vector3.up;
                }
                break;
            case 1:
                _direction = Vector3.up;
                break;
            case 2:
                _direction = Vector3.down;
                break;
        }
        //_direction = _beacon.transform.position - this.transform.position;
        _direction = _direction.normalized;

    }
    #endregion

    // Code, der das Schießen umsetzt
    #region Shooting
    void Shoot()
    {
        if (!_gunOnCooldown)
        {
            _gunOnCooldown = true;

            //really shoots, TBD
            Vector3 shootDirection = new Vector3(-1, 0, 0);
            shootDirection = _player.transform.position - this.transform.position;

            // Accuracy Berechnung:
            float curAccY = Random.Range(1, 1 + (1 - _accuracy));

            shootDirection = new Vector3(shootDirection.x, shootDirection.y * curAccY, 0);
            shootDirection = shootDirection.normalized;

            // Start Position bestimmen:
            Vector3 startPosition = this.transform.position + _gunOffsetFactor * shootDirection;

            GameObject projc = Instantiate(_enemyProjectile, startPosition, Quaternion.identity);

            projc.GetComponent<EnemyProjectile>().movementSpeed = _projectileSpeed;
            if (_projectileSpeed == 0)
            {
                projc.GetComponent<EnemyProjectile>().movementSpeed = this._movementSpeed * 2f;
            }
            projc.GetComponent<EnemyProjectile>().direction = shootDirection;
            projc.GetComponent<EnemyProjectile>().damage = this._projectileDamage;

            float cd = (Random.value * _gunMaxCD) + _gunMinCD;
            StartCoroutine(GunCooldown(cd));
        }
    }

    //Wartet bis der Gegner wieder schießen kann
    IEnumerator GunCooldown(float cd)
    {
        _gunOnCooldown = true;
        yield return new WaitForSeconds(cd);
        _gunOnCooldown = false;
        //if (_isActive)
        //{
        //    Shoot();
        //}
    }
    #endregion

}

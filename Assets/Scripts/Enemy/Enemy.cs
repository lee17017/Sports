using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{

    private static bool _collisionHack = false;

    [SerializeField]
    private int _health; //Wieviele Leben der Gegner hat
    // Wenn -1, ist der Gegner unverwundbar und kann im Moment NIE sterben.

    [SerializeField]
    private float _timeToLive; //Sagt aus, wielange dieser Gegner aktiv überleben kann (0 = Infinity)
    [SerializeField]
    private int _collisionDamage; // Sagt aus, wieviel Schaden der Spieler bei Kontakt erleidet
    [SerializeField]
    private bool _doesNotRotate; // Rotiert er automatisch?

    //------------------------
    [Header("MOVEMENT")]
    [SerializeField]
    private bool _canMove; // true = kann sich bewegen
    #region movementSettings
    [SerializeField] // Einstellungen für die Bewegung
    private bool _moveToPlayer;
    [SerializeField]
    private bool _notMoveBehindPlayer; // does not update movement when being left to the player
    [SerializeField]
    private float _movementSpeed;
    [Tooltip("0 means no updates")]
    [SerializeField]
    private float _updateBeaconPeriod;
    [SerializeField]
    private GameObject _beacon;

    private Vector3 _direction;
    #endregion

    //------------------------
    [Header("SHOOTING")]
    [SerializeField]
    private bool _canShoot; // Einstellungen für schießen (<- false == kann nicht schießen)
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
    private bool _notShootBehindPlayer;
    [SerializeField]
    private shootingTarget _gunTarget; // Hier wird festgelegt, in welche Richtung der Gegner schießt (auf den Spieler oder in feste Richtungen)
    //[SerializeField]
    //private shootingTarget _gunTargetModifier2; // Richtungsmodifier
    [SerializeField]
    private GameObject _enemyProjectile; // Das Projektil Prefab

    private bool _gunOnCooldown = false;

    enum shootingTarget
    {
        Player, Beacon, Left, Right, Down, Up
    }
    #endregion

    //------------------------
    private bool _isActive = false;
    private GameObject _player;


    // Use this for initialization
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        _player = GameObject.FindGameObjectWithTag("Player");

        if (_canShoot || _canMove)
        _beacon.transform.position = new Vector3(_beacon.transform.position.x, _beacon.transform.position.y, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isActive)
        {
            if (_canMove)
            {
                Move();
            }
            if (_canShoot && !_gunOnCooldown)
            {
                Shoot();
            }

            if (_startHack && !_doesNotRotate)
            {
                // Update LookAt() - Rotation:
                if (_canShoot && _gunTarget == shootingTarget.Player)
                {
                    LookDirection(_player.transform.position - transform.position);
                }
                else if (_canMove)
                {
                    LookDirection(_direction);
                }
            }
        } 
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

    void InitialiseEnemy()
    {
        GetMovingDirection(); //erste Mal _direction setzen

        if (_canShoot && !_gunOnCooldown)
        {
            StartCoroutine(GunCooldown(_gunStartCD));
        }

        StartCoroutine(startCD()); // Hack
    }

    // Small Hack
    private bool _startHack = false;
    IEnumerator startCD()
    {
        yield return new WaitForSeconds(0.8f);
        _startHack = true;
    }
    // Hack End

    // Hier kommen alle Kollisionsabfragen rein
    void OnTriggerEnter(Collider collision)
    {
        //Wenn er mit dem Spieler kollidiert, kassiert dieser ein Schaden und der Gegner stirbt
        if (collision.gameObject.tag == "Player")
        {
            _player.GetComponent<Player>().Damage(this._collisionDamage);
            Die();
        }
        // Kollision mit Gegn. Projektil erhöht ihre Bewegungsgeschwindigkeit
        else if (collision.gameObject.tag == "EnemyProjectile")
        {
            //LooseHealth(1);
            //_movementSpeed += 0.4f;
            Destroy(collision.gameObject);
        }
        // Aktivierung der Gegner:
        else if (collision.gameObject.tag == "CameraBox" && !_isActive)
        {
            InitialiseEnemy();
            _isActive = true;
        }
        // Kollision mit Gegner stoppt diesen hier kurzzeitig:
        else if (collision.gameObject.tag == "Enemy")
        {
            _collisionHack = !_collisionHack;
            if (_collisionHack)
            {
                StartCoroutine(CollisionDelay());
            }
        }
    }
    void OnTriggerExit(Collider collision)
    {
        if (_isActive & collision.gameObject.tag == "CameraBox")
        {
            _isActive = false;
            StartCoroutine(TimeToDie());
        }
    }

    // Leben verlieren
    public void LooseHealth(int damage)
    {
        if (_health != -1)
        {
            _health -= damage;
            if (_health <= 0)
            {
                Die();
            }
        }
    }

    //Was der Gegner beim Sterben macht
    void Die()
    {
        Destroy(gameObject);
    }

    // Wird aufgerufen, wenn der Gegner die sichtbarkeit verlässt, er stirbt dann irgendwann
    IEnumerator TimeToDie()
    {
        yield return new WaitForSeconds(_timeToLive);
        Destroy(this.gameObject);
    }

    // Der Bereich, der für die Bewegung verantwortlich ist
    #region Movement
    void Move()
    {
        //Bewegung
        float timeFactor = _movementSpeed * Time.deltaTime;
        Vector3 nextStep = new Vector3(transform.position.x + (_direction.x * timeFactor), transform.position.y + (_direction.y * timeFactor), transform.position.z + (_direction.z * timeFactor));
        transform.position = nextStep;
    }

    void GetMovingDirection()
    {
        if (!_notMoveBehindPlayer || !IsBehindPlayer())
        {
            if (_moveToPlayer)
            {
                _beacon.transform.position = _player.transform.position;
                if (_updateBeaconPeriod > 0)
                {
                    StartCoroutine(UpdateBeacon());
                }
            }

            _direction = _beacon.transform.position - this.transform.position;
            _direction = _direction.normalized;
        }
    }

    //Wartet und lässt die Richtung aktualisieren
    IEnumerator UpdateBeacon()
    {
        yield return new WaitForSeconds(_updateBeaconPeriod);
        GetMovingDirection();
    }
    #endregion

    // Code, der das Schießen umsetzt
    #region Shooting
    void Shoot()
    {
        if (!_gunOnCooldown && (!_notShootBehindPlayer || !IsBehindPlayer()))
        {
            _gunOnCooldown = true;

            //really shoots, TBD
            Vector3 shootDirection = new Vector3(-1, 0, 0);
            switch (_gunTarget)
            {
                case shootingTarget.Down:
                    shootDirection = new Vector3(0, -1, 0);
                    break;
                case shootingTarget.Up:
                    shootDirection = new Vector3(0, 1, 0);
                    break;
                case shootingTarget.Left:
                    shootDirection = new Vector3(-1, 0, 0);
                    break;
                case shootingTarget.Right:
                    shootDirection = new Vector3(1, 0, 0);
                    break;
                case shootingTarget.Player:
                    shootDirection = _player.transform.position - this.transform.position;
                    break;
                case shootingTarget.Beacon:
                    shootDirection = _beacon.transform.position - this.transform.position;
                    break;
            }

            // Accuracy Berechnung:
            float curAccX = Random.Range(1, 1 + (1 - _accuracy));
            //shootDirection.x *= curAccX;
            //float curAccY = Random.Range(1 - (1 - _accuracy), 1 + (1 - _accuracy));
            //shootDirection.y *= curAccY;
            shootDirection = new Vector3(shootDirection.x * curAccX, shootDirection.y, 0);
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


    bool IsBehindPlayer()
    {
        // wenn der gegner min. 1 x koordinate weiter links ist.
        return (this.transform.position.x + 1 < _player.transform.position.x);
    }
    
    IEnumerator CollisionDelay()
    {
        _isActive = false;
        yield return new WaitForSeconds(1f);
        _isActive = true;
    }

    public bool getActive()
    {
        return _isActive;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{

    [SerializeField]
    private int _health; //Wieviele Leben der Gegner hat

    [SerializeField]
    private float _timeToLive; //Sagt aus, wielange dieser Gegner aktiv überleben kann (0 = Infinity)

    //------------------------
    [SerializeField]
    private bool _canMove; // true = kann sich bewegen
    #region movementSettings
    [SerializeField] // Einstellungen für die Bewegung
    private bool _moveToPlayer;
    [SerializeField]
    private float _movementSpeed;
    [SerializeField]
    private float _updateBeaconPeriod;
    [SerializeField]
    private GameObject _beacon;

    private Vector3 _direction;
    #endregion

    //------------------------
    [SerializeField]
    private bool _canShoot; // Einstellungen für schießen (<- false == kann nicht schießen)
    #region shootingSettings
    [SerializeField]
    private float _gunMinCD; //Kleinster Cooldown zwischen Schüssen
    [SerializeField]
    private float _gunMaxCD; //Größert Cooldown (nimmt random wert zwischen min und max)
    [SerializeField]
    private float _gunStartCD; //CD, wenn der Gegner aktiviert wird
    [SerializeField]
    private float _accuracy; //1 = Zielt genau auf Spieler, <1 und >0 ist ungenauigkeitswahrscheinlichkeit
    [SerializeField]
    private shootingTarget _gunTarget; // Hier wird festgelegt, in welche Richtung der Gegner schießt (auf den Spieler oder in feste Richtungen)
    //[SerializeField]
    //private shootingTarget _gunTargetModifier2; // Richtungsmodifier
    [SerializeField]
    private GameObject _enemyProjectile; // Das Projektil Prefab

    private bool _onCooldown = false;

    enum shootingTarget
    {
        Player, Left, Right, Down, Up
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
        _beacon.transform.position = new Vector3(_beacon.transform.position.x, _beacon.transform.position.y, 0f);
    }

    // Update is called once per frame
    void Update()
    {

        if (_canMove && _isActive)
        {
            Move();
        }
        //if (_canShoot)
        //{
        //    Shoot();
        //}
    }

    void InitialiseEnemy()
    {
        GetMovingDirection(); //erste Mal _direction setzen

        if (_canShoot)
        {
            StartCoroutine(GunCooldown(_gunStartCD));
        }
    }

    // Hier kommen alle Kollisionsabfragen rein
    void OnTriggerEnter(Collider collision)
    {
        //Wenn er mit dem Spieler kollidiert, kassiert dieser ein Schaden und der Gegner stirbt
        if (collision.gameObject.tag == "Player")
        {
            _player.GetComponent<Player>().Damage(1);
            Die();
        }
        // Beim Aufprall eines Spieler-projektils verlieren sie per se 1 Leben.
        //else if (collision.gameObject.tag == "PlayerProjectile")
        //{
        //    LooseHealth(1);
        //}
        // Kollision mit Gegn. Projektil auch 1 Schaden, aber ihre Bewegungsgeschwindigkeit erhöht sich.
        /*
        else if (collision.gameObject.tag == "EnemyProjectile")
        {
            LooseHealth(1);
            _movementSpeed *= 1.5f;
        }
         */
        else if (collision.gameObject.tag == "CameraBox")
        {
            if (_isActive)
            {
                _isActive = false;
                StartCoroutine(TimeToDie());
            }
            else
            {
                InitialiseEnemy();
                _isActive = true;
            }
        }
    }

    // Leben verlieren
    public void LooseHealth(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            Die();
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

    /* Alter OnBecame(In-)Visible Code
    void OnBecameInvisible()
    {
        //Wenn ein Gegner nicht mehr in der kamera sichtbar ist, verschwindet es nach kurzer Zeit
        if (_isActive)
        {
            _isActive = false;
            StartCoroutine(TimeToDie());
        }
    }
    void OnBecameVisible()
    {
        if (!_isActive)
        {
            InitialiseEnemy();
            _isActive = true;
        }
    }
     */

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
        if (_canShoot)
        {
            _canShoot = false;

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
            }
            shootDirection = shootDirection.normalized;
            GameObject projc = Instantiate(_enemyProjectile, this.transform.position, Quaternion.identity);
            projc.GetComponent<EnemyProjectile>().movementSpeed = this._movementSpeed * 2f;
            projc.GetComponent<EnemyProjectile>().direction = shootDirection;

            float cd = (Random.value * _gunMaxCD) + _gunMinCD;
            StartCoroutine(GunCooldown(cd));
        }
    }

    //Wartet bis der Gegner wieder schießen kann
    IEnumerator GunCooldown(float cd)
    {
        _canShoot = false;
        yield return new WaitForSeconds(cd);
        _canShoot = true;
        Shoot();
    }
    #endregion

}

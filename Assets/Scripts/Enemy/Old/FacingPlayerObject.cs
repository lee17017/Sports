using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingPlayerObject : Enemy
{
    //public Zielposition
    [SerializeField]
    private GameObject _player;

    [SerializeField]
    private float _movementSpeed;

    [SerializeField]
    private float _updateBeaconPeriod;

    private Vector3 _direction;


    protected override void Act()
    {
        //Bewegung
        float timeFactor = _movementSpeed * Time.deltaTime;
        Vector3 nextStep = new Vector3(transform.position.x + (_direction.x * timeFactor), transform.position.y + (_direction.y * timeFactor), transform.position.z + (_direction.z * timeFactor));
        transform.position = nextStep;
    }

    protected override void Collide(Player player)
    {
        //player.DealDamage(1);
        LooseHealth(1);
    }

    protected override void Die()
    {
        Destroy(gameObject);
    }

    protected override void LooseHealth(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            Die();
        }
    }

    protected override void OnEnemyHit(GameObject collision)
    {
        //do nothing
    }

    protected override void OnProjectileHit(GameObject collision)
    {
        LooseHealth(1);
    }

    // Berechnet die Richtung, in die sich das Objekt bewegt (normalisiert)
    // In diesem Fall auf den Spieler zu.
    private void GetMovingDirection()
    {
        _direction = _player.transform.position - this.transform.position;
        _direction = _direction.normalized;
        if (_updateBeaconPeriod >= 0)
        {
            StartCoroutine(UpdateBeacon());
        }
    }

    //Wartet und lässt die Richtung aktualisieren
    IEnumerator UpdateBeacon()
    {
        new WaitForSeconds(_updateBeaconPeriod);
        GetMovingDirection();
        yield return null;
    }

    // Use this for initialization
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        GetMovingDirection();
    }

    // Update is called once per frame
    void Update()
    {
        Act();
    }
}



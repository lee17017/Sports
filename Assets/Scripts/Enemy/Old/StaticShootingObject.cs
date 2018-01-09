using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticShootingObject : Enemy
{

    [SerializeField]
    private float gunMinCD; //Kleinster Cooldown zwischen Schüssen
    [SerializeField]
    private float gunMaxCD; //Größert Cooldown (nimmt random wert zwischen min und max)
    [SerializeField]
    private float accuracy; //1 = Zielt genau auf Spieler, <1 und >0 ist ungenauigkeitswahrscheinlichkeit



    protected override void Act()
    {
        //Does absolutely nothing
    }

    protected override void Collide(Player player)
    {
        //player.DealDamage(1); // Fügt dem Spieler einen Schaden zu.
        Die();
    }

    protected override void Die()
    {
        Destroy(this.gameObject);
    }

    protected override void OnProjectileHit(GameObject collision)
    {
        LooseHealth(1);
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
        // does nothing
    }

    void Shoot()
    {

    }

    // Use this for initialization
    void Start()
    {

    }

    
}

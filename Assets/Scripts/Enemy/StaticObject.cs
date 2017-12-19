using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObject : Enemy
{

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

    // Use this for initialization
    void Start()
    {

    }

    
}

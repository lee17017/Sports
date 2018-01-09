using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyShoot : MonoBehaviour {

    [SerializeField]
    private float _gunMinCD; //Kleinster Cooldown zwischen Schüssen
    [SerializeField]
    private float _gunMaxCD; //Größert Cooldown (nimmt random wert zwischen min und max)
    [SerializeField]
    private float _accuracy; //1 = Zielt genau auf Target, <1 und >0 ist ungenauigkeitswahrscheinlichkeit
    [SerializeField]
    private shootingTarget _target; // Auf wen geschossen wird

    [SerializeField]
    private GameObject _enemyProjectile;


    private bool _onCooldown = false;

    enum shootingTarget
    {
        Player, Left, Right, Down, Up
    }

	// Use this for initialization
	void Start () {

        Enemy enemy = GetComponent<Enemy>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Enemy.isActive == true)
        {

        }
	}

    void Shoot()
    {

    }
}

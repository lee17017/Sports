using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour {

    [SerializeField]
    protected int _health; //Wieviele Leben der Gegner hat
    //public static int zPosition = 0; //In welcher Ebene sich alle Gegner befinden werden!

    public bool isActive = true;

	// Use this for initialization
	void Start () {
        //transform.position = new Vector3(transform.position.x, transform.position.y, zPosition);
	}
	
	// Update is called once per frame
	void Update () {

        if(isActive) //Wird später erst nach Levelweite aktiviert
        {
            Act();
        }
	}

    // Hier kommen alle Kollisionsabfragen rein
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Collide(collision.gameObject.GetComponent<Player>());
        } else if (collision.gameObject.tag == "Projectile")
        {
            OnProjectileHit(collision.gameObject);
        } else if (collision.gameObject.tag == "Enemy")
        {
            OnEnemyHit(collision.gameObject);
        }
    }

    //Wenn der Gegner mit dem Spieler kollidiert
    protected abstract void Collide(Player player);

    //Was der Gegner macht (jeden Frame aufruf)
    protected abstract void Act();

    //Was der Gegner vor dem Sterben macht
    protected abstract void Die();

    //Was der Gegner macht, wenn er getroffen wird.
    protected abstract void OnProjectileHit(GameObject collision);

    //Was der Gegner macht, sollte er mit einem anderen Gegner kollidieren
    protected abstract void OnEnemyHit(GameObject collision);

    protected abstract void LooseHealth(int damage);

}

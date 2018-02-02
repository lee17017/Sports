using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{

    public Vector3 direction;
    public float movementSpeed;
	public int damage;

    public static float _timeToLive = 4f; //Sekunden anzahl, bevor das Projektil verschwindet

    // Use this for initialization
    void Start()
    {
		if (damage == 0) {
			damage = 1;
		}
    }

    // Update is called once per frame
    void Update()
    {

        //this.transform.LookAt(transform.position + direction);
        float zRot;
        if (direction.x == 0)
            zRot = 90 + (direction.y <= 0 ? 0 : 180f);
        else
            zRot = Mathf.Atan(direction.y / direction.x) / Mathf.PI * 180 + (direction.x <= 0 ? 0 : 180f);
        
        transform.rotation = Quaternion.Euler(0, 0, zRot);


        float timeFactor = movementSpeed * Time.deltaTime;
        Vector3 nextStep = new Vector3(transform.position.x + (direction.x * timeFactor), transform.position.y + (direction.y * timeFactor), transform.position.z + (direction.z * timeFactor));
        transform.position = nextStep;
    }

    public EnemyProjectile(Vector3 direction, float speed, Vector3 scale)
    {
        direction = direction.normalized;
        movementSpeed = speed;
        this.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
    }

    IEnumerator WaitBeforeDie()
    {
        yield return new WaitForSeconds(_timeToLive);
        Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider collision)
    {
        //Wenn er mit dem Spieler kollidiert, kassiert dieser ein Schaden und der Gegner stirbt
        if (collision.gameObject.tag == "Player")
        {
			collision.GetComponent<Player>().Damage(this.damage);
            Destroy(this.gameObject);
        }
    }

    // Wenn ein Projektil das Spiefeld verlässt, wird es nach X Sekunden entfernt
    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "CameraBox")
        {
            StartCoroutine(WaitBeforeDie());
        }
    }
}

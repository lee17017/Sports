using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{

    public Vector3 direction;
    public float movementSpeed;

    public static float _timeToLive = 4f; //Sekunden anzahl, bevor das Projektil verschwindet

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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
            collision.GetComponent<Player>().Damage(1);
            Destroy(this.gameObject);
        }
        else if (collision.gameObject.tag == "CameraBox")
        {
            StartCoroutine(WaitBeforeDie());
        }
    }

    /*
    void OnBecameInvisible()
    {
        //Wenn ein projetil nicht mehr in der kamera sichtbar ist, verschwindet es nach kurzer Zeit
        StartCoroutine(WaitBeforeDie());
    }
     */
}

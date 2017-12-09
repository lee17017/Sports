using UnityEngine;

public abstract class BaseObject: MonoBehaviour {

    private void OnCollisionEnter(Collision collision){
        if(collision.gameObject.GetComponent<Player>()){
            NotifyPlayerCollision(collision.gameObject.GetComponent<Player>());
        }
    }

    protected abstract void NotifyPlayerCollision(Player player);

}

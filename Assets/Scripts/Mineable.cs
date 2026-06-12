using UnityEngine;

public class Mineable : MonoBehaviour
{
    public int health;
    public int damage;
    
    public void Mine()
    {
        health -= damage ;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}

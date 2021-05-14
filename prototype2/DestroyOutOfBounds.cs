using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{
    public float topBounds = 30.0f;
    public float bottomBounds = -30.0f;
    
    // Update is called once per frame
    void Update()
    {
        if(transform.position.z > topBounds)
        {
            Destroy(gameObject);
        }
        else if (transform.position.z < bottomBounds)
        {
            Debug.Log("Game Over :(");
            Destroy(gameObject);
        }
    }
}

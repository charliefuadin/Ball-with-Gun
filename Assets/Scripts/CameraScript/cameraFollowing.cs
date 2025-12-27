using UnityEngine;

public class cameraFollowing : MonoBehaviour
{
    public float followSpeed = 2f;
    public float yOffset = 1f;
    public Transform target;

    void Update ()
    {   
         Vector3 newPosition = new Vector3(target.position.x,target.position.y + yOffset, -10f);
         transform.position = Vector3.Lerp(transform.position,newPosition,followSpeed * Time.deltaTime);
    }
}



 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollow : MonoBehaviour
{
    public void aimAtTarget(Vector3 position)
    {
        Vector2 direction = new Vector2(position.x - transform.position.x, position.y - transform.position.y);
        transform.up = -direction;

        //Gets the angle difference from mouse and gun
        Vector3 difference = position - transform.position;
        float rotz = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        float facingRight = Mathf.Abs(transform.localScale.x);

        //flips the gun sprite based on z Rotation
        if (rotz > 89 || rotz < -89)
        {
            transform.localScale = new Vector3(-facingRight, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(facingRight, transform.localScale.y, transform.localScale.z);
        }
    }

}

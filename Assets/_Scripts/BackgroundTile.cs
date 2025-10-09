using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{

    private Vector2 targetPosition;
    private bool isMoving = false;

    public void SetTargetPosition(Vector2 target)
    {
        targetPosition = target;
        isMoving = true;
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector2.Lerp(transform.position, targetPosition, 0.1f);
            if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }

}

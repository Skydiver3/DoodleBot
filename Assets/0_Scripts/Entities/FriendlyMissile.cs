using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyMissile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform leftBottom;
    [SerializeField] private Transform rightTop;
    [SerializeField] private int inaccuracy;

    void FixedUpdate()
    {
        transform.position += transform.right * speed;
        transform.Rotate(new Vector3(0,0, UnityEngine.Random.Range(-inaccuracy, inaccuracy + 1)));
        DestroyIfOutOfBounds();
    }

    private void DestroyIfOutOfBounds()
    {
        if (gameObject.transform.position.x < leftBottom.position.x ||
            gameObject.transform.position.y < leftBottom.position.y ||
            gameObject.transform.position.x > rightTop.position.x ||
            gameObject.transform.position.y > rightTop.position.y)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        //or add to pool
    }
}

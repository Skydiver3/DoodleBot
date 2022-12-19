using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    // Variables
    [SerializeField] private Transform leftBottom;
    [SerializeField] private Transform rightTop;
    [SerializeField] private float speed;

    private SpriteRenderer _explosionRenderer;
    private Animator _explosionAnimator;
    [SerializeField] private float animationTime;

    void Start()
    {
        _explosionAnimator = GetComponentInChildren<Animator>();
        _explosionRenderer = _explosionAnimator.GetComponent<SpriteRenderer>();
        GameObject sprite = gameObject.GetComponentInChildren<BoxCollider>().gameObject;
        sprite.transform.right = new Vector3(Random.Range(-180, 180),0,0);
    }

    void FixedUpdate()
    {
        move();
        DestroyIfOutOfBounds();
    }
    
    private void move()
    {
        transform.position += transform.right * speed;
    }

    private void DestroyIfOutOfBounds()
    {
        if (gameObject.transform.position.x < leftBottom.position.x - 2f ||
            gameObject.transform.position.y < leftBottom.position.y - 2f||
            gameObject.transform.position.x > rightTop.position.x + 2f||
            gameObject.transform.position.y > rightTop.position.y + 2f)
        {
            StartCoroutine(DestroyAfterSeconds(animationTime));
            //World.destroyAsteroid(gameObject);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(DestroyAfterSeconds(animationTime));
        }
        else if (other.CompareTag("FriendlyMissile"))
        {
            StartCoroutine(DestroyAfterSeconds(animationTime));
        }
        else if (other.CompareTag("Shockwave"))
        {            
            StartCoroutine(DestroyAfterSeconds(animationTime));
        }
    }

    private IEnumerator DestroyAfterSeconds(float seconds)
    {
        _explosionAnimator.SetTrigger("trigger");
        yield return new WaitForSeconds(seconds);
        _explosionRenderer.sprite = null;
        World.destroyAsteroid(gameObject);
    }
}

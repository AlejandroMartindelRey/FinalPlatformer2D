using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour, IDamagable
{
    [SerializeField] private Transform patrolPath;
    [SerializeField] private float speed;
    
    [SerializeField] Collider2D firstCollider;
    [SerializeField] Collider2D secondCollider;
    [SerializeField] Rigidbody2D rigidBody;

    [SerializeField] Animator anim;
    public float enemyHp = 1;

    private int currentIndex = 0;
    private List<Vector3> patrolPoints = new List<Vector3>();

    public void TakeDamage(GameObject dealer, int damage)
    {
        
    }

    private IEnumerator Death()
    {
        Destroy(firstCollider);
        Destroy(secondCollider);
        Destroy(rigidBody);
        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
    }
    private void Awake()
    {
        foreach (Transform child in patrolPath)
        {
            patrolPoints.Add(child.position);
        }
        
        transform.eulerAngles = transform.position.x > patrolPoints[currentIndex].x ? new Vector3(0, 180, 0) : Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyHp <= 0)
        {
            anim.Play("Enemy_Death");
            StartCoroutine(Death());
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, patrolPoints[currentIndex], speed * Time.deltaTime);
        
            if (transform.position == patrolPoints[currentIndex]) 
            {
                SetNewDestination();
            }
        }
    }

    private void SetNewDestination()
    {
        //currentIndex + 1 mod(Length)
        currentIndex = (currentIndex + 1) % patrolPoints.Count;
        
        transform.eulerAngles = transform.position.x > patrolPoints[currentIndex].x ? new Vector3(0, 180, 0) : Vector3.zero;
        
    }
}

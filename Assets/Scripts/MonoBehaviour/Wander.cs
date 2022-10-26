using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Animator))]


public class Wander : MonoBehaviour
{
    CircleCollider2D circleCollider;

    public float pursuitSpeed;
    public float wanderSpeed;
    float currentSpeed;

    public float directionChangeInterval;

    public bool followPlayer;

    Coroutine moveCoroutine;

    Rigidbody2D rb2D;
    Animator animator;

    Transform targetTransform;

    Vector3 endPosition;

    float currentAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();

        animator = GetComponent<Animator>();
        currentSpeed = wanderSpeed;

        rb2D = GetComponent<Rigidbody2D>();

        StartCoroutine(WanderRoutine());
    }
    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(rb2D.position, endPosition, Color.red);
    }

    public IEnumerator WanderRoutine()
    {
        while (true)
        {
            ChooseNewEndPoint();

            if(moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            moveCoroutine = StartCoroutine(Move(rb2D, currentSpeed));

            yield return new WaitForSeconds(directionChangeInterval);
        }
    }

    void ChooseNewEndPoint()
    {
        currentAngle += Random.Range(0, 360);

        currentAngle += Mathf.Repeat(currentAngle, 360);

        endPosition += Vector3FromAngle(currentAngle);
    }

    Vector3 Vector3FromAngle(float inputAngleDegrees)
    {
        float inputAngleRadians = inputAngleDegrees * Mathf.Deg2Rad;

        return new Vector3(Mathf.Cos(inputAngleRadians), Mathf.Sin(inputAngleRadians), 0);
    }

    public IEnumerator Move(Rigidbody2D rigidBodyToMove, float speed)
    {
        float remainingDistance = (transform.position - endPosition).sqrMagnitude;

        while(remainingDistance > float.Epsilon)
        {
            if(targetTransform != null)
            {
                endPosition = targetTransform.position;
            }

            if(rigidBodyToMove != null)
            {
                animator.SetBool("isWalking", true);

                Vector3 newPosition = Vector3.MoveTowards(rigidBodyToMove.position, endPosition, speed * Time.deltaTime);

                rb2D.MovePosition(newPosition);

                remainingDistance = (transform.position - endPosition).sqrMagnitude;
            }
            yield return new WaitForFixedUpdate();
        }
        animator.SetBool("isWalking", false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && followPlayer)
        {
            currentSpeed = pursuitSpeed;

            targetTransform = collision.gameObject.transform;

            if(moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }

            moveCoroutine = StartCoroutine(Move(rb2D, currentSpeed));
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            animator.SetBool("isWalking", false);
            currentSpeed = wanderSpeed;

            if(moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            targetTransform = null;
        }
    }

    private void OnDrawGizmos()
    {
        if(circleCollider != null)
        {
            Gizmos.DrawWireSphere(transform.position, circleCollider.radius);
        }
    }
}

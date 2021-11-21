using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : MonoBehaviour
{

    private enum BunnyStates { Idle, Alert, Move };

    //config params
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float awareRange = 5f;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float minFidgetTimer = 0.5f;
    [SerializeField] float maxFidgetTimer = 3f;
    [SerializeField] bool patrols;
    [SerializeField] float minPatrolTimer = 0.5f;
    [SerializeField] float maxPatrolTimer = 1f;

    //cached references
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    SpriteRenderer myRenderer;
    GameObject currentTarget;
    [SerializeField] BunnyStates myState;
    float fidgetTimer;
    float patrolTimer;
    [SerializeField] float hopAnimationTime;
    bool hopping = false;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myRenderer = GetComponent<SpriteRenderer>();
        myState = BunnyStates.Idle;
        fidgetTimer = Random.Range(minFidgetTimer, maxFidgetTimer);
        if (patrols)
        {
            patrolTimer = Random.Range(minPatrolTimer, maxPatrolTimer);
            AnimationClip[] clips = myAnimator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                switch(clip.name)
                {
                    case "Bunny Hop":
                        hopAnimationTime = clip.length;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
        CheckForCharacters();
        MoveToCharacter();
        AnimateBunny();
    }

    private void Patrol()
    {
        if (!patrols) { return; }
        if (myState == BunnyStates.Idle)
        {
            if (patrolTimer <= 0)
            {
                StartCoroutine(HopOnce());
            }
            else
            {
                patrolTimer -= Time.deltaTime;
            }
        }
        if (myState == BunnyStates.Move)
        {
            transform.Translate(moveSpeed * Time.deltaTime * ((myRenderer.flipX.GetHashCode() * 2) -1), 0, 0);
            CheckForWall();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        myRenderer.flipX = !myRenderer.flipX;
    }


    private void CheckForWall()
    {
        RaycastHit2D foundWall = Physics2D.Raycast(transform.position, Vector2.left, 0.1f);
        if (foundWall == false) { return; }
        if (foundWall.collider.gameObject.layer == 8)
        {
            myRenderer.flipX = !myRenderer.flipX;
        }
        else
        { return; }
    }

    private IEnumerator HopOnce()
    {
        myState = BunnyStates.Move;
        hopping = true;
        yield return new WaitForSeconds(hopAnimationTime);
        patrolTimer = Random.Range(minPatrolTimer, maxPatrolTimer);
        myState = BunnyStates.Idle;
        hopping = false;
    }


private void AnimateBunny()
    {
        switch (myState)
        {
            case BunnyStates.Idle:
                if(fidgetTimer <= 0)
                {
                    myAnimator.Play("Bunny Fidget");
                    fidgetTimer = Random.Range(minFidgetTimer, maxFidgetTimer);
                }
                else
                {
                    fidgetTimer -= Time.deltaTime;
                }
                break;
            case BunnyStates.Alert:
                myAnimator.Play("Bunny Alert");
                break;
            case BunnyStates.Move:
                myAnimator.Play("Bunny Hop");
                break;
            default:
                break;
        }
    }

    private void CheckForCharacters()
    {
        if (hopping) { return; }
        if (currentTarget != null) { return; }
        RaycastHit2D foundTarget = Physics2D.Raycast(transform.position, Vector2.left, awareRange);
        if (foundTarget == false) 
        {
            foundTarget = Physics2D.Raycast(transform.position, Vector2.right, awareRange);
            if (foundTarget == false) { return; } 
            else
            {
                myRenderer.flipX = true;
            }
        }
        else
        {
            myRenderer.flipX = false;
        }
        
        if (foundTarget.collider.gameObject.GetComponent<PlayerCharacter>() == null) { return; }
        else 
        {
            myState = BunnyStates.Alert;
            currentTarget = foundTarget.collider.gameObject; 
        }
    }
    private void MoveToCharacter()
    {
        if (currentTarget == null) { return; }
        var distanceToTarget = Vector2.Distance(transform.position, currentTarget.gameObject.transform.position);
        if(distanceToTarget > awareRange) 
        {
            currentTarget = null;
            myState = BunnyStates.Idle;
            return; 
        }
        else if (distanceToTarget > attackRange)
        {
            myState = BunnyStates.Alert;
        }
        var directionToTarget = Mathf.Sign(currentTarget.transform.position.x - transform.position.x);
        if (directionToTarget == 1)
        {
            myRenderer.flipX = true;
        }
        else if (directionToTarget == -1)
        {
            myRenderer.flipX = false;
        }
        if (distanceToTarget <= attackRange)
        {
            myState = BunnyStates.Move;
            transform.Translate(moveSpeed * Time.deltaTime * directionToTarget, 0, 0);
        }
    }

}

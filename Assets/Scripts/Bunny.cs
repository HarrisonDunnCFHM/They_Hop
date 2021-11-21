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
    [SerializeField] float patrolTimer;
    float hopAnimationTime;
    bool hopping = false;
    [SerializeField] float hopTimer;

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
            hopTimer = hopAnimationTime;
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
        if (myState == BunnyStates.Idle || hopping)
        {
            if (patrolTimer <= 0)
            {
                HopOnce();
            }
            else
            {
                patrolTimer -= Time.deltaTime;
            }
        }
        if (myState == BunnyStates.Move)
        {
            transform.Translate(moveSpeed * Time.deltaTime * -(transform.localScale.x), 0, 0);
            CheckForWall();
        }
    }
    private void HopOnce()
    {
        myState = BunnyStates.Move;
        hopping = true;
        if(hopTimer <= 0)
        {
            hopping = false;
            hopTimer = hopAnimationTime;
            patrolTimer = Random.Range(minPatrolTimer, maxPatrolTimer);
            myState = BunnyStates.Idle;
        }
        else
        {
            hopTimer -= Time.deltaTime;
        }
    }



    private void CheckForWall()
    {
        RaycastHit2D foundWall = Physics2D.Raycast(transform.position, Vector2.left, 0.1f, 1<<LayerMask.NameToLayer("Terrain"));
        if (foundWall == false) { return; }
        transform.localScale = new Vector2(-(Mathf.Sign(myRigidBody.velocity.x)), 1f);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        transform.localScale = new Vector2(-(transform.localScale.x), 1f);
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
        RaycastHit2D foundTarget = Physics2D.Raycast(transform.position + new Vector3(awareRange,0f,0f), Vector2.left, awareRange * 2, 1<<LayerMask.NameToLayer("Player Characters"));
        if (foundTarget == false) { return; }
        /*{
            foundTarget = Physics2D.Raycast(transform.position, Vector2.right, awareRange);
            if (foundTarget == false) { return; } 
            else
            {
                transform.localScale = new Vector2(1f, 1f);
            }
        }
        else
        {
            transform.localScale = new Vector2(-1f, 1f);
        }*/

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
        var directionToTarget = Mathf.Sign(transform.position.x - currentTarget.transform.position.x );
        transform.localScale = new Vector2(directionToTarget, 1f);
        if (distanceToTarget <= attackRange)
        {
            myState = BunnyStates.Move;
            transform.Translate(moveSpeed * Time.deltaTime * -directionToTarget, 0, 0);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : MonoBehaviour
{

    private enum BunnyStates { Idle, Alert, Move, Bite, MonsterIdle, MonsterMove };

    //config params
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float awareRange = 5f;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float minFidgetTimer = 0.5f;
    [SerializeField] float maxFidgetTimer = 3f;
    [SerializeField] bool patrols;
    [SerializeField] float minPatrolTimer = 0.5f;
    [SerializeField] float maxPatrolTimer = 1f;
    [SerializeField] ParticleSystem bloodSplat;

    //cached references
    PlayerHandler playerHandler;
    Renderer myRenderer;
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    GameObject currentTarget;
    BunnyStates myState;
    float fidgetTimer;
    float patrolTimer;
    float hopAnimationTime;
    float biteAnimationTime;
    bool hopping = false;
    float hopTimer;
    float spriteWidth;

    // Start is called before the first frame update
    void Start()
    {
        myRenderer = GetComponent<Renderer>();
        playerHandler = FindObjectOfType<PlayerHandler>();
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myState = BunnyStates.Idle;
        fidgetTimer = Random.Range(minFidgetTimer, maxFidgetTimer);
        spriteWidth = myRenderer.bounds.size.x;
        if (patrols) //set up patrol specific parameters
        {
            patrolTimer = Random.Range(minPatrolTimer, maxPatrolTimer);
            hopTimer = hopAnimationTime;
            AnimationClip[] clips = myAnimator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips) //getting the length of animations so that only one plays during action
            {
                switch(clip.name)
                {
                    case "Bunny Hop":
                        hopAnimationTime = clip.length;
                        break;
                    case "Bunny Bite":
                        biteAnimationTime = clip.length;
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
    
    private void AnimateBunny() //control animations based on state
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
            case BunnyStates.Bite:
                myAnimator.Play("Bunny Bite");
                break;
            default:
                break;
        }
    } 

    private void Patrol() //if idle, patrol
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
    
    private void HopOnce() //hop once and put random time between hops during a patrol
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
        RaycastHit2D foundWall = Physics2D.Raycast(transform.position, Vector2.left, spriteWidth, 1<<LayerMask.NameToLayer("Terrain"));
        if (foundWall == false) { return; }
        transform.localScale = new Vector2(-(Mathf.Sign(myRigidBody.velocity.x)), 1f);
    } //reverse direction if hitting a wall during a patrol
    
    private void OnTriggerExit2D(Collider2D collision) //reverse direction if hitting a ledge using box collider parascope on front of object
    {
        transform.localScale = new Vector2(-(transform.localScale.x), 1f);
    } 
    
    private void CheckForCharacters()
    {
        if (hopping) { return; }
        if (currentTarget != null) { return; }
        RaycastHit2D foundTarget = Physics2D.Raycast(transform.position + new Vector3(awareRange,0f,0f), Vector2.left, awareRange * 2, 1<<LayerMask.NameToLayer("Player Characters"));
        if (foundTarget == false) { return; }
        if (foundTarget.collider.gameObject.GetComponent<PlayerCharacter>() == null) { return; }
        else 
        {
            myState = BunnyStates.Alert;
            currentTarget = foundTarget.collider.gameObject; 
        }
    } //raycast for characters on the Player Characters layer
    
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
    }//if there's a target, be alert or move towards it, or forget it

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player Characters"))
        {
            currentTarget = null;
            myState = BunnyStates.Bite;
            StartCoroutine(DeathAnimation(collision.gameObject));
        }
    }

    private IEnumerator DeathAnimation(GameObject deadObject)
    {
        if (playerHandler.activeCharacter == deadObject)
        {
            playerHandler.activeCharacter = null;
        }
        deadObject.GetComponent<Collider2D>().enabled = false;
        deadObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        deadObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        yield return new WaitForSeconds(biteAnimationTime);
        deadObject.GetComponent<SpriteRenderer>().enabled = false;
        Instantiate(bloodSplat, deadObject.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(5f);
        myState = BunnyStates.Idle;
        Destroy(deadObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : MonoBehaviour
{

    private enum BunnyStates { Idle, Alert, Move};

    //config params
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float awareRange = 5f;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float minFidgetTimer = 0.5f;
    [SerializeField] float maxFidgetTimer = 3f;

    //cached references
    Animator myAnimator;
    SpriteRenderer myRenderer;
    GameObject currentTarget;
    [SerializeField] BunnyStates myState;
    float fidgetTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myRenderer = GetComponent<SpriteRenderer>();
        myState = BunnyStates.Idle;
        fidgetTimer = Random.Range(minFidgetTimer, maxFidgetTimer);
    }

    // Update is called once per frame
    void Update()
    {
        CheckForCharacters();
        MoveToCharacter();
        AnimateBunny();
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

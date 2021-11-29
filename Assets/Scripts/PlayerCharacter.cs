using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public enum CharacterState { Idle, Run, Climb, Jump, Fall, Dead, Throw, Crouch, Crawl, Use };

    public enum CharacterClass { Strong, Small, Smart };

    //config params
    public CharacterClass myClass;

    //cached references
    PlayerHandler playerHandler;
    Rigidbody2D myRigidBody2D;
    public CharacterState myState;
    Animator myAnimator;
    bool isActive;
    public PlayerCharacter[] throwableFriends;


    // Start is called before the first frame update
    void Start()
    {
        playerHandler = FindObjectOfType<PlayerHandler>();
        myAnimator = GetComponent<Animator>();
        myState = CharacterState.Idle;
        myRigidBody2D = GetComponent<Rigidbody2D>();
        throwableFriends = FindObjectsOfType<PlayerCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimateCharacter();
    }


    private void AnimateCharacter()
    {
        if (myRigidBody2D.velocity.y < 0f)
        {
            myState = PlayerCharacter.CharacterState.Fall;
        }
        else if (myRigidBody2D.velocity.y == 0f && !isActive)
        {
            myState = PlayerCharacter.CharacterState.Idle;
        }
        switch (myClass)
        {
            case CharacterClass.Strong:
                switch (myState)
                {
                    case CharacterState.Idle:
                        if (!this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Strong Land"))
                        {
                            myAnimator.Play("Strong Idle");
                        }
                        break;
                    case CharacterState.Run:
                        if (!this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Strong Run"))
                        {
                            myAnimator.Play("Strong Run");
                        }
                        break;
                    case CharacterState.Climb:
                        myAnimator.Play("Strong Climb");
                        break;
                    case CharacterState.Jump:
                        myAnimator.Play("Strong Jump");
                        break;
                    case CharacterState.Fall:
                        myAnimator.Play("Strong Fall");
                        break;
                    case CharacterState.Dead:
                        myAnimator.Play("Strong Dead");
                        break;
                    case CharacterState.Throw:
                        myAnimator.Play("Strong Throw");
                        break;
                    default:
                        break;
                }
                break;
            case CharacterClass.Small:
                switch (myState)
                {
                    case CharacterState.Idle:
                        myAnimator.Play("Small Idle");
                        break;
                    case CharacterState.Run:
                        myAnimator.Play("Small Run");
                        break;
                    case CharacterState.Climb:
                        myAnimator.Play("Small Climb");
                        break;
                    case CharacterState.Fall:
                        myAnimator.Play("Small Fall");
                        break;
                    case CharacterState.Dead:
                        myAnimator.Play("Small Dead");
                        break;
                    case CharacterState.Crouch:
                        myAnimator.Play("Small Crouch");
                        break;
                    case CharacterState.Crawl:
                        myAnimator.Play("Small Crawl");
                        break;
                    default:
                        break;
                }
                break;
            case CharacterClass.Smart:
                switch (myState)
                {
                    case CharacterState.Idle:
                        myAnimator.Play("Smart Idle");
                        break;
                    case CharacterState.Run:
                        myAnimator.Play("Smart Run");
                        break;
                    case CharacterState.Climb:
                        myAnimator.Play("Smart Climb");
                        break;
                    case CharacterState.Fall:
                        myAnimator.Play("Smart Fall");
                        break;
                    case CharacterState.Dead:
                        myAnimator.Play("Smart Dead");
                        break;
                    case CharacterState.Use:
                        myAnimator.Play("Smart Use");
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    public void OnMouseDown()
    {
        var allCharacters = FindObjectsOfType<PlayerCharacter>();
        foreach (PlayerCharacter playerChar in allCharacters)
        {
            playerChar.isActive = false;
            playerChar.gameObject.layer = LayerMask.NameToLayer("Player Characters");
        }
        isActive = true;
        playerHandler.SelectCharacter(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (myRigidBody2D.velocity.y < 0 && collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            myAnimator.Play("Strong Land");
            myState = CharacterState.Idle;
        }
 
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerCharacter>() != null)
        {
            throwableFriends = null;
        }
    }

}

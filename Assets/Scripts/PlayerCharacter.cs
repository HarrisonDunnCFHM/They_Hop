using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public enum CharacterState { Idle, Run, Climb, Fall, Dead, Throw, Crouch, Crawl };

    public enum CharacterClass { Strong, Small, Smart };

    //config params
    [SerializeField] CharacterClass myClass;

    //cached references
    PlayerHandler playerHandler;
    public CharacterState myState;
    Animator myAnimator;


    // Start is called before the first frame update
    void Start()
    {
        playerHandler = FindObjectOfType<PlayerHandler>();
        myAnimator = GetComponent<Animator>();
        myState = CharacterState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        AnimateCharacter();
    }


    private void AnimateCharacter()
    {
        switch (myClass)
        {
            case CharacterClass.Strong:
                switch (myState)
                {
                    case CharacterState.Idle:
                        myAnimator.Play("Strong Idle");
                        break;
                    case CharacterState.Run:
                        myAnimator.Play("Strong Run");
                        break;
                    case CharacterState.Climb:
                        myAnimator.Play("Strong Climb");
                        break;
                    case CharacterState.Fall:
                        myAnimator.Play("Strong Fall");
                        break;
                    case CharacterState.Dead:
                        myAnimator.Play("Strong Dead");
                        break;
                    case CharacterState.Throw:
                        myAnimator.Play("Strong THrow");
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
        playerHandler.SelectCharacter(gameObject);
    }
}

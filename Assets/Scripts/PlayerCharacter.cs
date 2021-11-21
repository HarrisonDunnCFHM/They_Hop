using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public enum CharacterStates { Idle, Run, Throw, Crouch, Dead};
    
    //cached references
    PlayerHandler playerHandler;
    public CharacterStates myState;
    Animator myAnimator;

    
    // Start is called before the first frame update
    void Start()
    {
        playerHandler = FindObjectOfType<PlayerHandler>();
        myAnimator = GetComponent<Animator>();
        myState = CharacterStates.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        AnimateCharacter();
    }

    private void AnimateCharacter()
    {
        switch (myState)
        {
            case CharacterStates.Idle:
                myAnimator.Play("Strong Idle");
                break;
            case CharacterStates.Run:
                myAnimator.Play("Strong Run");
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

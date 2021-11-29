using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHandler : MonoBehaviour
{
    //config params
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpSpeed = 10f;

    //cached references
    [SerializeField] public GameObject activeCharacter;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        ResetLevel();
    }

    private void ResetLevel()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void MovePlayer()
    {
        if(activeCharacter == null) { return; }
        float moveDirection = Input.GetAxisRaw("Horizontal");
        if (moveDirection != 0)
        {
            activeCharacter.transform.localScale = new Vector2(moveDirection, 1f);
            if (activeCharacter.GetComponent<Rigidbody2D>().velocity.y == 0)
            {
                activeCharacter.GetComponent<PlayerCharacter>().myState = PlayerCharacter.CharacterState.Run;
            }
        }
        else
        {
            activeCharacter.GetComponent<PlayerCharacter>().myState = PlayerCharacter.CharacterState.Idle;
        }
        activeCharacter.transform.Translate(moveSpeed * Time.deltaTime * moveDirection, 0, 0);
        if (activeCharacter.GetComponent<PlayerCharacter>().myClass == PlayerCharacter.CharacterClass.Strong) //Strong specific actions
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (activeCharacter.GetComponent<Rigidbody2D>().velocity.y == 0 && moveDirection != 0) //jump if moving
                {
                    activeCharacter.GetComponent<PlayerCharacter>().myState = PlayerCharacter.CharacterState.Jump;
                    activeCharacter.GetComponent<Rigidbody2D>().velocity = new Vector2(0, jumpSpeed);
                }
                else //throw if not moving and have a friend nearby
                {
                    if (activeCharacter.GetComponent<PlayerCharacter>().throwableFriend != null)
                    {
                        var throwDirection = activeCharacter.transform.localScale.x;
                        GameObject thrownCharacter = activeCharacter.GetComponent<PlayerCharacter>().throwableFriend;
                        thrownCharacter.GetComponent<Rigidbody2D>().velocity = new Vector2(jumpSpeed * throwDirection * 0.5f, jumpSpeed);
                        thrownCharacter.transform.localScale = new Vector2(throwDirection, 1);
                        activeCharacter.GetComponent<PlayerCharacter>().throwableFriend = null;
                    }
                }
            }
        }
    }

    public void SelectCharacter(GameObject clickedCharacter)
    {
        activeCharacter = clickedCharacter;
    }

}

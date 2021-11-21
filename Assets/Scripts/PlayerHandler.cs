using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHandler : MonoBehaviour
{
    //config params
    [SerializeField] float moveSpeed;

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
            activeCharacter.GetComponent<PlayerCharacter>().myState = PlayerCharacter.CharacterStates.Run;
        }
        else
        {
            activeCharacter.GetComponent<PlayerCharacter>().myState = PlayerCharacter.CharacterStates.Idle;
        }
        activeCharacter.transform.Translate(moveSpeed * Time.deltaTime * moveDirection, 0, 0);
    }

    public void SelectCharacter(GameObject clickedCharacter)
    {
        activeCharacter = clickedCharacter;
    }

}

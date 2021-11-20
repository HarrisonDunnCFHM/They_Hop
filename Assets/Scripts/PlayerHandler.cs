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
    [SerializeField] GameObject activeCharacter;
    
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
        activeCharacter.transform.Translate(moveSpeed * Time.deltaTime * moveDirection, 0, 0);
    }

    public void SelectCharacter(GameObject clickedCharacter)
    {
        activeCharacter = clickedCharacter;
    }

}

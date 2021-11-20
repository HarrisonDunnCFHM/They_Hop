using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    //cached references
    PlayerHandler playerHandler;
    
    // Start is called before the first frame update
    void Start()
    {
        playerHandler = FindObjectOfType<PlayerHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDown()
    {
        playerHandler.SelectCharacter(gameObject);
    }
}

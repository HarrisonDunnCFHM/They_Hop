using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : MonoBehaviour
{

    //config params
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float aggroRange = 2f;

    //cached references
    [SerializeField] GameObject currentTarget;
    PlayerCharacter[] allTargets;
    
    // Start is called before the first frame update
    void Start()
    {
        allTargets = FindObjectsOfType<PlayerCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForCharacters();
        MoveToCharacter();
    }

    private void CheckForCharacters()
    {
        
        if (currentTarget != null) { return; }
        RaycastHit2D foundTarget = Physics2D.Raycast(transform.position, Vector2.left, aggroRange);
        if (foundTarget == false) 
        {
            foundTarget = Physics2D.Raycast(transform.position, Vector2.right, aggroRange);
            if (foundTarget == false) { return; } 
        }
        Debug.Log("found " + foundTarget.collider.gameObject.name);
        if (foundTarget.collider.gameObject.GetComponent<PlayerCharacter>() == null) { return; }
        else { currentTarget = foundTarget.collider.gameObject; }
    }
    private void MoveToCharacter()
    {
        if (currentTarget == null) { return; }
        var distanceToTarget = Vector2.Distance(transform.position, currentTarget.gameObject.transform.position);
        if(distanceToTarget > (2 * aggroRange)) 
        {
            currentTarget = null;
            return; 
        }
        var directionToTarget = Mathf.Sign(currentTarget.transform.position.x - transform.position.x);
        transform.Translate(moveSpeed * Time.deltaTime * directionToTarget, 0, 0);
    }

}

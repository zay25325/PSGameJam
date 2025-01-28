/*
Class: TurnSystemManager
Description: This class will manage the turn system for the game. 
             It will handle the order of turns for the player and the enemies. Additionally will handle the 
             logic for the player and enemy actions.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystemManager : MonoBehaviour
{
    //Here are my goals for the turn system manager:

    //Enemy Intent Phase
    //Player Action Phase
    //Enemy Action Phase
    //Enemy Action Priority

    //Have logic related to
    //Units can ONLY move or Attack. Can't do both.

    //using an enum to contain different phases of the turn-based combat system
    private enum TurnPhase
    {
        EnemyActionPriority, //Enemy Action Priority
        EnemyIntent,    //Enemy Intent Phase
        PlayerAction,   //Player Action Phase
        EnemyAction    //Enemy Action Phase
    }

    private TurnPhase currentPhase; //variable to store the current phase of the turn-based combat system
    private bool isCombatActive; //variable to store if the combat is active

    private bool isPlayerTurn; //variable to store if it is the player's turn
    private bool isEnemyTurn; //variable to store if it is the enemy's turn

    private GameObject player; //variable to store the player object
    private List<GameObject> enemies; //variable to store the list of enemy objects

    private TileManager tileManager; //variable to store the tile manager object



    /*
        FUNCTION : Start()
        DESCRIPTION : This function is called before the first frame update. 
                      It initializes the current phase to the Enemy Intent Phase and initializes the list of enemy objects.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    void Start()
    {
        currentPhase = TurnPhase.EnemyActionPriority; //initialize the current phase to the Enemy Intent Phase

        //need to figure out a way to get reference of all the enemies that will be participating in combat
        enemies = new List<GameObject>(); //initialize the list of enemy objects

        tileManager = FindObjectOfType<TileManager>(); //get the tile manager object

        player = GameObject.FindGameObjectWithTag("Player");

        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy")); //get all the enemy objects in the scene

        StartCombat(player, enemies); //start the combat system
    }

    /*
        FUNCTION : Update()
        DESCRIPTION : This function is called once per frame. 
                      It will check the current phase of the turn-based combat system and run the appropriate phase.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    void Update()
    {
        //Debug.Log("Current Phase: " + currentPhase);
    }

    /*
        FUNCTION : RunTurnSystem()
        DESCRIPTION : This function will run the turn-based combat system. 
                      It will run the different phases of the turn-based combat system in a loop.
        PARAMETERS : NONE
        RETURNS : Yield return - returns an object that can be used to pause the execution of the function
    */
    private IEnumerator RunTurnSystem()
    {
        while (isCombatActive)
        {
            CheckCombatState();
            switch (currentPhase)
            {
                case TurnPhase.EnemyActionPriority:
                    //Enemy Action Priority Phase
                    //Logic for the enemy to determine their action priority
                    //This is where the enemy will determine their action priority

                    yield return StartCoroutine(EnemyActionPriorityPhase());
                    currentPhase = TurnPhase.EnemyIntent; //move back to the Enemy Intent Phase
                    break;
                case TurnPhase.EnemyIntent:
                    //Enemy Intent Phase
                    //Logic for the enemy to determine their intent
                    //This is where the enemy will determine their next move

                    yield return StartCoroutine(EnemyIntentPhase());
                    currentPhase = TurnPhase.PlayerAction; //move to the Player Action Phase
                    break;
                case TurnPhase.PlayerAction:
                    //Player Action Phase
                    //Logic for the player to determine their action
                    //This is where the player will determine their next move

                    yield return StartCoroutine(PlayerActionPhase());
                    currentPhase = TurnPhase.EnemyAction; //move to the Enemy Action Phase
                    break;
                case TurnPhase.EnemyAction:
                    //Enemy Action Phase
                    //Logic for the enemy to determine their action
                    //This is where the enemy will determine their next move

                    yield return StartCoroutine(EnemyActionPhase());
                    currentPhase = TurnPhase.EnemyIntent; //move to the Enemy Action Priority Phase
                    break;
            }
        }
    }

private IEnumerator EnemyActionPriorityPhase()
    {
        //Logic for the enemy to determine their action priority
        //This is where the enemy will determine their action priority

        Debug.Log("Enemy Action Priority Phase");
        yield return new WaitForSeconds(2f); //simulate the enemy thinking about their next move

        //Thought process for the enemy to determine their action priority
        //use a priority queue or a sorted list to manage who goes first
        //possibly to determine who is the closest to the player and have them go first?
        //unless stats can be involved to determine who goes first
    }

    /*
        FUNCTION : EnemyIntentPhase()
        DESCRIPTION : This function will run the Enemy Intent Phase of the turn-based combat system. 
                      It will simulate the enemy thinking about their next move.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private IEnumerator EnemyIntentPhase()
    {
        //Logic for the enemy to determine their intent
        //This is where the enemy will determine their next move

        Debug.Log("Enemy Intent Phase");
        yield return new WaitForSeconds(2f); //simulate the enemy thinking about their next move
    }

    /*
        FUNCTION : PlayerActionPhase()
        DESCRIPTION : This function will run the Player Action Phase of the turn-based combat system. 
                      Player decides what they want and initiate action. Their turn ends after action
                      is conducted
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private IEnumerator PlayerActionPhase()
    {
        //This currently works in the test environment, writing out pseudocode related to the player action phase below
        //Logic for the player to determine their action
        //This is where the player will determine their next move

        Debug.Log("Player Action Phase");
        player.GetComponent<DemoPlayer>().enabled = true;
        bool playerAction = false;

        //check to see if key was pressed or action was initiated

        //Thought process
        Vector3 initialPlayerPosition = player.transform.position; //get the initial player position

        while (!playerAction)
        {
            if(initialPlayerPosition != player.transform.position)
            {
                //lock the player from using mousebuttondown(0)
                //player has moved
                //disable the player script
                player.GetComponent<DemoPlayer>().enabled = false;
                playerAction = true;
                ///yield return new WaitForSeconds(0.5f);
            }
            else if(Input.GetMouseButtonDown(0))
            {
                //player has initiated an action
                //disable the player script
                player.GetComponent<DemoPlayer>().enabled = false;
                playerAction = true;
                //yield return new WaitForSeconds(0.5f);
            }
            yield return null; // wait for the next frame
        }
            yield return new WaitForSeconds(0.5f);
        // yield return new WaitForSeconds(2f); //simulate the player thinking about their next move


        //pseudocode
        //SO essentially it will run the same as the above
        //while looping going indefinetly until player does an action
        //Player once moved will end their turn
        //OR
        //Player once initiated an action will end their turn
        //So it might look like this

        /*
            isPlayerTurn = true;
            Vector3 initialPlayerPosition = player.transform.position; //get the initial player position


            while(isPlayerTurn)
            {
                //first instance checks if player has moved, not difficult to check that
                if(initialPlayerPosition != player.transform.position)
                {
                    //player has moved
                    isPlayerTurn = false;
                }
                //second instance checks if player has initiated an action
                //need to figure out how to properly check if the player has initiated an action
                //but thats all that is needed for this system.
                else if(Input.GetMouseButtonDown(0))
                {
                    //player has initiated an action
                    isPlayerTurn = false;
                }
                yield return null; // wait for the next frame
            }
        */
    }

    /*
        FUNCTION : EnemyActionPhase()
        DESCRIPTION : This function will run the Enemy Action Phase of the turn-based combat system. 
                      It will simulate the enemy doing their action sequentially after the player has done their action.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private IEnumerator EnemyActionPhase()
    {
        //Logic for the enemy to determine their action
        //This is where the enemy will determine their next move

        Debug.Log("Enemy Action Phase");
        yield return new WaitForSeconds(2f); //simulate the enemy thinking about their next move
    }

    /*
        FUNCTION : StartCombat()
        DESCRIPTION : This function will start the combat system. 
                      It will set the player and enemy objects and set the combat to active.
        PARAMETERS : GameObject playerRef - reference to the player object
                     List<GameObject> enemiesRef - reference to the list of enemy objects
        RETURNS : NONE
    */
    public void StartCombat(GameObject playerRef, List<GameObject> enemiesRef)
    {
        player = playerRef;
        enemies = enemiesRef;
        isCombatActive = true;
        StartCoroutine(RunTurnSystem());
        
    }

    /*
        FUNCTION : StopCombat()
        DESCRIPTION : This function will stop the combat system. 
                      It will set the combat to inactive and stop all coroutines. Addtionally, it will clear the player and enemy objects.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    public void StopCombat()
    {
        isCombatActive = false;
        StopAllCoroutines();
        player = null;
        enemies.Clear();
    }

    public void CheckCombatState()
    {
        if (isCombatActive)
        {
            //Check if the player is dead or all enemies are dead
            if (player == null || enemies.Count == 0)
            {
                //stop combat if either the player or all enemies are dead
                StopCombat();
                return;
            }
        }
    }
}

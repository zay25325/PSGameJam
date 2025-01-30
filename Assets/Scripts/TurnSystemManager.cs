/*
Class: TurnSystemManager
Developer: Emanuel Juracic
First Version: 1/26/2025
Description: This class will manage the turn system for the game. 
             It will handle the order of turns for the player and the enemies. Additionally will handle the 
             logic for the player and enemy actions.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static AttackShapeBuilder;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TurnSystemManager : MonoBehaviour
{
    //Turn Phase Enum
    //Contains different stages of the turn-based combat system
    private enum TurnPhase
    {
        EnemyIntent,    //Enemy Intent Phase
        PlayerAction,   //Player Action Phase
        EnemyAction    //Enemy Action Phase
    }

    //Variables
    private TurnPhase currentPhase; //variable to store the current phase of the turn-based combat system
    private bool isCombatActive; //variable to store if the combat is active

    private GameObject player; //variable to store the player object
    private List<GameObject> enemies; //variable to store the list of enemy objects

    private TileManager tileManager; //variable to store the tile manager object
    private CharacterInfo playerInfo; //variable to store the player info

    private List<MoveAction> plannedEnemyMoves = new List<MoveAction>();    // List of enemy moves
    private List<AttackShape> plannedEnemyAttacks = new List<AttackShape>();    // List of enemy attacks

    GameObject mainCanvas;


    /*
        FUNCTION : Start()
        DESCRIPTION : This function is called before the first frame update. 
                      It initializes the current phase to the Enemy Intent Phase and initializes the list of enemy objects.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    void Start()
    {
        currentPhase = TurnPhase.EnemyIntent; //initialize the current phase to the Enemy Intent Phase

        //need to figure out a way to get reference of all the enemies that will be participating in combat
        enemies = new List<GameObject>(); //initialize the list of enemy objects

        tileManager = FindObjectOfType<TileManager>(); //get the tile manager object

        player = GameObject.FindGameObjectWithTag("Player");    // Find the player object

        playerInfo = player.GetComponent<CharacterInfo>();  // Get the player info

        Debug.Log("MORTAL KOMBAT");

        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy")); //get all the enemy objects in the scene

        mainCanvas = GameObject.Find("Main Canvas");
        
        mainCanvas.SetActive(false);

        StartCombat(player, enemies); //start the combat system

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
            switch (currentPhase)
            {
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
            EnemyRefresh(); // Refresh the enemy queue
            CheckCombatState(); // Check the state of the combat system
        }
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
        // Logic for the enemy to determine their intent
        // This is where the enemy will determine their next move

        // Clear previous planned moves and attacks
        plannedEnemyMoves.Clear();
        plannedEnemyAttacks.Clear();

        // Log the start of the enemy intent phase
        Debug.Log("Enemy Intent Phase");

        // Refresh enemy states (e.g., update their status, reset flags, etc.)
        EnemyRefresh();

        // Get the player's current position
        Vector3 playerPosition = player.transform.position; // Assuming you have a reference to the player

        // Iterate through each enemy to determine their actions
        foreach (GameObject enemy in enemies)
        {
            // Get the enemy's current position
            Vector3 enemyPosition = enemy.transform.position;

            // Determine the direction from the enemy to the player
            Direction attackDirection = GetDirectionToPlayer(enemyPosition, playerPosition);

            // Convert enemy's position to tile coordinates
            Vector2Int enemyTilePosition = new Vector2Int(Mathf.FloorToInt(enemyPosition.x), Mathf.FloorToInt(enemyPosition.y));

            // Check if the enemy is adjacent to the player (within attack range)
            if (Vector2Int.Distance(enemyTilePosition, new Vector2Int(Mathf.FloorToInt(playerPosition.x), Mathf.FloorToInt(playerPosition.y))) <= 1.5f) // Adjust range as needed
            {
                // If in range, plan an attack
                // Create an attack shape based on the direction and enemy's position
                AttackShape attack = AttackShapeBuilder.AttackAt(
                    AttackShape.AttackDictionary[AttackShape.AttackKeys.Cleave],
                    attackDirection,
                    enemyTilePosition
                );
                // Add the planned attack to the list
                plannedEnemyAttacks.Add(attack);
            }
            else
            {
                // If not in range, plan to move toward the player
                // Determine the target tile to move toward the player
                Vector2Int targetTile = MoveToward(new Vector2Int(Mathf.FloorToInt(playerPosition.x), Mathf.FloorToInt(playerPosition.y)), enemyTilePosition); // Basic movement logic

                // Create a move action with the current and target positions
                MoveAction move = new MoveAction(enemyPosition, new Vector3(targetTile.x, enemyPosition.y, targetTile.y), enemy.GetComponent<CharacterInfo>());

                // Add the planned move to the list
                plannedEnemyMoves.Add(move);
            }
        }

        // End the coroutine
        yield return null;
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
    Debug.Log("Player Action Phase");
    bool playerAction = false;
    Toggle moveToggle;
    Vector3 initialPlayerPosition = player.transform.position; //get the initial player position

    mainCanvas.SetActive(true); //Turning back on Canvas so player can interact with UI
    
    
    Toggle selectedAttack = null;   // Selected attack button

    List<Toggle> combatButtons = new List<Toggle>();    // List of combat buttons

 
    // Get the combat buttons from the MainCanvas
    moveToggle = mainCanvas.transform.Find("Bottom Bar/MovementSection/MoveButton")?.GetComponent<Toggle>();

    // Get the DemoPlayer component from the MainCanvas
    DemoPlayer demoPlayer = mainCanvas.GetComponent<DemoPlayer>();

    // Check if the DemoPlayer component exists
    if (demoPlayer == null)
    {
        Debug.Log("DemoPlayer component not found on Main Canvas");
        yield break;
    }

    // Enable the DemoPlayer component indicating combat can be initiated by the player
    demoPlayer.enabled = true;

    // Get reference to objects with the tag "Attack"
    GameObject[] attackObjects = GameObject.FindGameObjectsWithTag("Attack");   // Get all objects with the tag "Attack"
    List<CombatButton> combatButtonsList = new List<CombatButton>();    // List of CombatButton objects
    
    // Add CombatButton components to the list
    foreach (GameObject obj in attackObjects)   
    {
        CombatButton combatButton = obj.GetComponent<CombatButton>();
        if (combatButton != null)
        {
            combatButtonsList.Add(combatButton);
        }
    }

    // Keep running until player completes an action
    // Essentially the turn can only end with the player completes an action
    while (!playerAction)
    {
        // Loop through the list of combat buttons to check if an attack button is selected
        foreach (CombatButton combatButton in combatButtonsList)
        {
            // Check if the combat button is selected
            if (combatButton.GetComponent<Toggle>().isOn)
            {
                // Set the selected attack button
                selectedAttack = combatButton.GetComponent<Toggle>();
                break;
            }
        }

        //Check if player has left clicked with their mouse
        if (Input.GetMouseButtonDown(0))
        {
            // Check if the mouse is over a UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Check if the current selected game object is a toggle
                GameObject selectedObj = EventSystem.current.currentSelectedGameObject;

                // Check if the selected object is not a toggle
                // This is so player can click anywhere after selecting an attack
                //so the attack isn't conducted immedietly after first choice, player can
                //change their mind
                if (selectedObj == null || selectedObj.GetComponent<Toggle>() == null)
                {
                    demoPlayer.ClickedWorldSpace();  // Call the player action
                    playerAction = true;    // Set player action to true
                    demoPlayer.enabled = false; // Disable the DemoPlayer component

                    // Turn off all toggles in the combatButtons list
                    foreach (Toggle toggle in combatButtons)
                    {
                        toggle.isOn = false;
                    }
                }
            }
        }
        // **Check if movement button is active and player has moved**
        // This is to end the player's turn if they have moved
        if (moveToggle != null && moveToggle.isOn)
        {
            demoPlayer.enabled = true; // Enable player movement

            if (player.transform.position != initialPlayerPosition) // Detect movement
            {
                demoPlayer.enabled = false; // Disable player interaction
                playerAction = true;    // Set player action to true to end turn
                moveToggle.isOn = false; // Reset move button
                Debug.Log("Player moved. Ending turn.");
            }
        }

        yield return null; // Wait for the next frame
    }

    // **End of Player Action Phase**

    //remove active slots
    if(selectedAttack != null)
    {
        selectedAttack.isOn = false; // Reset attack button
    }

    mainCanvas.SetActive(false);    // Turn off the MainCanvas
    yield return null; 
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
        Debug.Log("Enemy Action Phase");

        // // Create a set to keep track of occupied positions
        // HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>();

        // // Initialize occupied positions with current enemy positions
        // foreach (GameObject enemy in enemies)
        // {
        //     // Add the current enemy position to the set
        //     Vector2Int currentPosition = new Vector2Int(Mathf.FloorToInt(enemy.transform.position.x), Mathf.FloorToInt(enemy.transform.position.y));
        //     occupiedPositions.Add(currentPosition);
        // }

        // // Move enemies first
        // foreach (MoveAction move in plannedEnemyMoves)
        // {
        //     // Find the enemy object based on the character info
        //     GameObject enemy = enemies.FirstOrDefault(e => e.GetComponent<CharacterInfo>() == move.Character);
        //     // Check if the enemy exists and the target position is unoccupied
        //     if (enemy != null)
        //     {
        //         Vector2Int targetPosition = move.MoveTo;

        //         // Check if the target position is unoccupied
        //         if (!occupiedPositions.Contains(targetPosition))
        //         {
        //             // Move the enemy to the target position
        //             enemy.transform.position = new Vector3(targetPosition.x, enemy.transform.position.y, targetPosition.y);
        //             occupiedPositions.Add(targetPosition);
        //             yield return null; // Wait for the next frame
        //         }
        //         else
        //         {
        //             // Debug.Log($"Target position {targetPosition} is occupied. Skipping move for {enemy.name}");
        //         }
        //     }
        // }

        // // Execute attacks
        // foreach (AttackShape attack in plannedEnemyAttacks)
        // {
        //     // Convert the attack start position to a Vector2Int
        //     Vector2Int attackPosition = new Vector2Int(Mathf.FloorToInt(attack.StartPosition.x), Mathf.FloorToInt(attack.StartPosition.y));
        //     Vector3 enemyPosition = new Vector3(attack.StartPosition.x, 0, attack.StartPosition.y);
        //     Direction attackDirection = GetDirectionToPlayer(enemyPosition, player.transform.position);
        //     // Check if the attack position is unoccupied
        //     if (!occupiedPositions.Contains(attackPosition))
        //     {
        //         TileManager.Instance.AddPlayerAttack(attack, attackDirection, enemyPosition); // Apply attack to tiles
        //         occupiedPositions.Add(attackPosition);
        //     }
        //     else
        //     {
        //         //Debug.Log($"Attack position {attackPosition} is already occupied. Skipping attack.");
        //     }
        // }

        yield return null; // Wait for 3 seconds


        //vincent can give enemies on a priority, so the AI type can give a preferred turn order.
        //so enemies can decide what they do in that order and execute in that order
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
        // Set the player and enemy objects
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

    /*
        FUNCTION : CheckCombatState
        DESCRIPTION : This function will check the state of the combat system. 
                      It will check if the player or all enemies are dead and stop the combat if either is true.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    public void CheckCombatState()
    {
        // Check if all enemies' HP are 0 or less
        
        bool allEnemiesDead = true;
        if (isCombatActive)
        {
            //Check if the player is dead or all enemies are dead
            if (player == null)
            {
                //stop combat if either the player or all enemies are dead
                StopCombat();
                Debug.Log("Player is dead");
                return;
            }
        }

        // Check if the player's HP is 0 or less
        CharacterInfo playerInfo = player.GetComponent<CharacterInfo>();
        if (playerInfo.HP <= 0)
        {
            Debug.Log("Player is dead");
            StopCombat();
            return;
        }

        foreach (GameObject enemy in enemies)
        {
            CharacterInfo enemyInfo = enemy.GetComponent<CharacterInfo>();
            if (enemyInfo.HP > 0)
            {
                allEnemiesDead = false;
                break;
            }
        }

        if (allEnemiesDead)
        {
            Debug.Log("All enemies are dead");
            StopCombat();
        }
    }

    /*
        FUNCTION : EnemyRefresh()
        DESCRIPTION : This function will refresh the enemy queue. 
                      It will remove any null or destroyed enemies from the queue.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    public void EnemyRefresh()
    {
        // Refresh the enemy queue, removing any null or destroyed enemies
        enemies.RemoveAll(enemy => enemy == null || enemy.GetComponent<CharacterInfo>().HP <= 0);
    }

    /*
        FUNCTION : MoveToward
        DESCRIPTION : This function will move the enemy toward the player. 
                      It will determine the direction to move and return the new position.
                      Very basic and won't be used for final implementation
                      Mainly for testing purposes
        PARAMETERS : Vector2Int targetPosition - the target position to move toward
                      Vector2Int currentPosition - the current position of the enemy
        RETURNS : pos - the new position of the enemy
                 newPosition - the original new position if unoccupied
    */
    private Vector2Int MoveToward(Vector2Int targetPosition, Vector2Int currentPosition)
    {
        Vector2Int direction = targetPosition - currentPosition;
        direction.x = Mathf.Clamp(direction.x, -1, 1); // Ensure one step at a time
        direction.y = Mathf.Clamp(direction.y, -1, 1);

        Vector2Int newPosition = currentPosition + direction; // New position

        // Check if the new position is occupied
        if (enemies.Any(enemy => new Vector2Int(Mathf.FloorToInt(enemy.transform.position.x), Mathf.FloorToInt(enemy.transform.position.y)) == newPosition))
        {
            // If occupied, try to find an alternative position
            List<Vector2Int> possiblePositions = new List<Vector2Int>
            {
                currentPosition + new Vector2Int(1, 0),
                currentPosition + new Vector2Int(-1, 0),
                currentPosition + new Vector2Int(0, 1),
                currentPosition + new Vector2Int(0, -1)
            };

            foreach (var pos in possiblePositions)
            {
                if (!enemies.Any(enemy => new Vector2Int(Mathf.FloorToInt(enemy.transform.position.x), Mathf.FloorToInt(enemy.transform.position.y)) == pos))
                {
                    return pos; // Return the first unoccupied position
                }
            }
        }

        return newPosition; // Return the original new position if unoccupied
    }

    /*
        FUNCTION : GetDirectionToPlayer
        DESCRIPTION : This function will determine the direction from the enemy to the player. 
                      It will calculate the direction vector and return the direction.
                      Very basic and won't be used for final implementation
                      Mainly for testing purposes
        PARAMETERS : Vector3 enemyPosition - the position of the enemy
                      Vector3 playerPosition - the position of the player
        RETURNS : NONE
    */
    private Direction GetDirectionToPlayer(Vector3 enemyPosition, Vector3 playerPosition)
    {
        Vector3 directionVector = (playerPosition - enemyPosition).normalized;

        if (Mathf.Abs(directionVector.x) > Mathf.Abs(directionVector.y))
        {
            return directionVector.x > 0 ? Direction.Right : Direction.Left;
        }
        else
        {
            return directionVector.y > 0 ? Direction.Up : Direction.Down;
        }
    }
}

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

    EnemyControllerBase enemyController;

    private CharacterInfo enemyInfo;

    private List<GameObject> aggressiveEnemies = new List<GameObject>();
    private List<GameObject> tacticalEnemies = new List<GameObject>();

    private List<EnemyControllerBase> enemyControllers = new List<EnemyControllerBase>();


    /*
        FUNCTION : Start()
        DESCRIPTION : This function is called before the first frame update. 
                      It initializes the current phase to the Enemy Intent Phase and initializes the list of enemy objects.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    void Start()
    {
        Debug.Log("Turn Start");
        currentPhase = TurnPhase.EnemyIntent; //initialize the current phase to the Enemy Intent Phase

        //need to figure out a way to get reference of all the enemies that will be participating in combat
        enemies = new List<GameObject>(); //initialize the list of enemy objects

        tileManager = FindObjectOfType<TileManager>(); //get the tile manager object

        player = GameObject.FindGameObjectWithTag("Player");    // Find the player object

        playerInfo = player.GetComponent<CharacterInfo>();  // Get the player info

        Debug.Log("MORTAL KOMBAT");

        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy")); //get all the enemy objects in the scene

        mainCanvas = GameObject.Find("Main Canvas");
        
        //mainCanvas.SetActive(false);

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
        //New

    // Separate enemies into their respective lists
        foreach (GameObject enemy in enemies)
        {
            // Check if the enemy is active
            if (enemy.activeSelf)
            {
                // Check if the enemy has an AggressiveEnemy or TacticalEnemy component
                if (enemy.GetComponent<AggressiveEnemy>() != null)
                {
                    // Add the enemy to the aggressiveEnemies list if not already present
                    if (!aggressiveEnemies.Contains(enemy))
                    {
                        aggressiveEnemies.Add(enemy);
                    }
                }
                // Check if the enemy has a TacticalEnemy component
                else if (enemy.GetComponent<TacticalEnemy>() != null)
                {
                    // Add the enemy to the tacticalEnemies list if not already present
                    if (!tacticalEnemies.Contains(enemy))
                    {
                        tacticalEnemies.Add(enemy);
                    }
                }
            }
        }

        // Handle aggressive enemies
        foreach (GameObject enemy in aggressiveEnemies)
        {
            // Check if the enemy is active
            if (enemy.activeSelf)
            {
                // Get the AggressiveEnemy component from the enemy
                AggressiveEnemy aggressiveEnemy = enemy.GetComponent<AggressiveEnemy>();
                // Get the intent of the aggressive enemy
                aggressiveEnemy.intentType = aggressiveEnemy.GetIntent();
                // Add the aggressive enemy to the enemyControllers list if not already present
                if (!enemyControllers.Contains(aggressiveEnemy))
                {
                    enemyControllers.Add(aggressiveEnemy);
                }
                Debug.Log("Aggressive Enemy Intent + " + aggressiveEnemy.intentType);
            }
        }

        // Handle tactical enemies
        foreach (GameObject enemy in tacticalEnemies)
        {
            // Check if the enemy is active
            if (enemy.activeSelf)
            {
                // Get the TacticalEnemy component from the enemy
                TacticalEnemy tacticalEnemy = enemy.GetComponent<TacticalEnemy>();
                // Get the intent of the tactical enemy
                tacticalEnemy.GetIntent();
                // Add the tactical enemy to the enemyControllers list if not already present
                if (!enemyControllers.Contains(tacticalEnemy))
                {
                    enemyControllers.Add(tacticalEnemy);
                }
                Debug.Log("Tactical Enemy Intent" + tacticalEnemy.intentType);
            }
        }
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

    //mainCanvas.SetActive(true); //Turning back on Canvas so player can interact with UI
    
    
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
                    //if a selectedattack isOn then do this
                    if (selectedAttack != null)
                    {
                        // Get the attack button from the selected attack
                        CombatButton attackButton = selectedAttack.GetComponent<CombatButton>();

                        // Check if the attack button is not null
                        if (attackButton != null)
                        {
                            // Add the attack button to the list of combat buttons
                            //combatButtons.Add(selectedAttack);
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
            }
        }
        // **Check if movement button is active and player has moved**
        // This is to end the player's turn if they have moved
        if (tileManager.notifyPlayerTurnEnd) {
            TileManager.Instance.EndTurnCallback();
            playerAction = true;
        }

        yield return null; // Wait for the next frame
    }

    // **End of Player Action Phase**

    //remove active slots
    if(selectedAttack != null)
    {
        selectedAttack.isOn = false; // Reset attack button
    }

    //mainCanvas.SetActive(false);    // Turn off the MainCanvas
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

    // Loop through aggressive enemies and transform their position based on their targetTile
    HashSet<Vector2Int> occupiedTiles = new HashSet<Vector2Int>();

    // Loop through the enemyControllers list
    foreach (GameObject enemy in aggressiveEnemies)
    {
        // Get the AggressiveEnemy component from the enemy
        AggressiveEnemy aggressiveEnemy = enemy.GetComponent<AggressiveEnemy>();

        // Check if the intentType is Move
        if (aggressiveEnemy.intentType == AggressiveEnemy.IntentType.Move)
        {
            // Get the targetTile from the aggressiveEnemy
            Vector2Int targetTile = aggressiveEnemy.targetTile;

            // Store the original position of the enemy
            Vector2 originalPosition = enemy.transform.position;

            // Check if the targetTile is already occupied
            if (!tileManager.IsTileOccupied(targetTile))
            {
                //tile is not occupied so move forward
                // Get the target position from the targetTile
                Vector3 newPosition = TileManager.TileToPosition(targetTile);
                enemy.transform.position = newPosition;
                Debug.Log("AG: Moved");
            }
            // Otherwise for now have a log warning
            //would need to look into having a different logic if tile is occupied
            else
            {
                List<Vector2Int> directions = new List<Vector2Int>
                {
                    new Vector2Int(0, 1),  // Up
                    new Vector2Int(0, -1), // Down
                    new Vector2Int(-1, 0), // Left
                    new Vector2Int(1, 0)   // Right
                };
                directions = directions.OrderBy(d => Random.value).ToList();

                // Try to find a free tile in the shuffled directions
                bool moved = false;
                foreach (Vector2Int direction in directions)
                {
                    Vector2Int newTargetTile = aggressiveEnemy.targetTile + direction;
                    if (!occupiedTiles.Contains(newTargetTile) && !tileManager.IsTileOccupied(newTargetTile))
                    {
                        occupiedTiles.Add(newTargetTile);
                        Vector2 targetPosition = new Vector2(newTargetTile.x, newTargetTile.y);
                        Vector3 newPosition = TileManager.TileToPosition(new Vector2Int((int)targetPosition.x, (int)targetPosition.y));
                        enemy.transform.position = newPosition;
                        Debug.Log("AG: Not Moved");
                        moved = true;
                        break;
                    }
                }
            }
        }
        // Check if the intentType is Attack
        else if (aggressiveEnemy.intentType == AggressiveEnemy.IntentType.Attack)
        {
            // Add the preparedAttack to the tileManager
            // This will display the attack shape on the grid
            // and conduct the attack
            tileManager.AddShape(aggressiveEnemy.preparedAttack);
            Debug.Log("Aggressive Enemy is attacking + " + aggressiveEnemy.intentType);
        }
    }

    // Loop through tactical enemies and transform their position based on their targetTile
   foreach (GameObject enemy in tacticalEnemies)
    {
        // Get the TacticalEnemy component from the enemy
        TacticalEnemy tacticalEnemy = enemy.GetComponent<TacticalEnemy>();
        
        // Check if the intentType is Move
        if (tacticalEnemy.intentType == TacticalEnemy.IntentType.Move)
        {
            // Get the targetTile from the tacticalEnemy
            Vector2Int targetTile = tacticalEnemy.targetTile;

            // Store the original position of the enemy
            Vector2 originalPosition = enemy.transform.position;

            // Check if the targetTile is already occupied
            if (!tileManager.IsTileOccupied(targetTile))
            {
                Vector3 newPosition = TileManager.TileToPosition(targetTile);
                enemy.transform.position = newPosition;
                Debug.Log("TE: Moved");
            }
            // Otherwise for now have a log warning
            //would need to look into having a different logic if tile is occupied
            else
            {
                // Shuffle the directions randomly
                List<Vector2Int> directions = new List<Vector2Int>
                {
                    new Vector2Int(0, 1),  // Up
                    new Vector2Int(0, -1), // Down
                    new Vector2Int(-1, 0), // Left
                    new Vector2Int(1, 0)   // Right
                };
                directions = directions.OrderBy(d => Random.value).ToList();

                // Try to find a free tile in the shuffled directions
                bool moved = false;
                foreach (Vector2Int direction in directions)
                {
                    Vector2Int newTargetTile = tacticalEnemy.targetTile + direction;
                    if (!occupiedTiles.Contains(newTargetTile) && !tileManager.IsTileOccupied(newTargetTile))
                    {
                        occupiedTiles.Add(newTargetTile);
                        Vector2 targetPosition = new Vector2(newTargetTile.x, newTargetTile.y);
                        Vector3 newPosition = TileManager.TileToPosition(new Vector2Int((int)targetPosition.x, (int)targetPosition.y));
                        enemy.transform.position = newPosition;
                        Debug.Log("TE: Not Move");
                        moved = true;
                        break;
                    }
                }
            }
        }

        // Check if the intentType is Attack
        else if (tacticalEnemy.intentType == TacticalEnemy.IntentType.Attack)
        {
            // Add the preparedAttack to the tileManager
            // This will display the attack shape on the grid
            // And conduct the attack
            tileManager.AddShape(tacticalEnemy.preparedAttack);
            Debug.Log("Tactical Enemy is attacking + " + tacticalEnemy.intentType);
        }   
    }
        yield return null;
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
        SaveDataManager saveDataManager = FindObjectOfType<SaveDataManager>();
        CharacterInfo playerInfo = player.GetComponent<CharacterInfo>();
        
        bool allEnemiesDead = true;
        if (isCombatActive)
        {
            //Check if the player is dead or all enemies are dead
            if (player == null || playerInfo.HP <= 0)
            {
                //stop combat if either the player or all enemies are dead
                StopCombat();
                saveDataManager.SelectedContract = null;
                UnityEngine.SceneManagement.SceneManager.LoadScene("ContractSelection");
                Debug.Log("Player is dead");
                return;
            }
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
            UnityEngine.SceneManagement.SceneManager.LoadScene("ContractSelection");
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
        // using a variable to as true to make it cleaner
        bool isTemp = true;

        int maxTempAttacks = 5;
        // get the DemoPlayer component from the MainCanvas
        // this will help us get reference to create combat buttons for temp attacks
        DemoPlayer getAttackFromDemoPlayer = mainCanvas.GetComponent<DemoPlayer>();

        // to get reference the amount of combat buttons within temp attacks
        // this is to ensure only up to the limit of 5 temp attacks can be created and used
        // past 5 can't add, but below 5 can add
        GameObject tempAttacks = mainCanvas.transform.Find("Bottom Bar/TempAttacks").gameObject;
        
        // Refresh the enemy queue, removing any null or destroyed enemies
        // adding attacks if possible
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            // Check if the enemy is null or their HP is 0 or less
            if (enemies[i] == null || enemies[i].GetComponent<CharacterInfo>().HP <= 0)
            {
                var enemyInfo = enemies[i].GetComponent<CharacterInfo>();   // Get the enemy info

                // Check if the enemy has attacks
                if (enemyInfo.Attacks != null && enemyInfo.Attacks.Count > 0)
                {
                    int toggleCount = 0;    //variable to count number of toggles available

                    // Check if there are already 5 or more toggles in tempAttacks
                    foreach (Transform child in tempAttacks.transform)
                    {
                        //increase toggle if child has a toggle component
                        if (child.GetComponent<Toggle>() != null)
                        {
                            toggleCount++;
                        }
                    }

                    //if less than 5, add the temp attack to the player arsenal
                    if (toggleCount < maxTempAttacks)
                    {
                        // Select a random attack from the defeated enemy's attacks
                        var randomAttack = enemyInfo.Attacks[Random.Range(0, enemyInfo.Attacks.Count)];
                        getAttackFromDemoPlayer.CreateCombatButton(randomAttack, isTemp);
                    }
                }
                // Remove the enemy from the list and scene
                enemies[i].SetActive(false);
                enemies.RemoveAt(i);
            }
        }
    }
}

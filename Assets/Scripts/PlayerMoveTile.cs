using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveTile : MonoBehaviour
{
    public CharacterInfo character;

    private void OnMouseDown()
    {
        MoveAction move = new MoveAction(character.transform.position, transform.position, character);

        TileManager.Instance.AddPlayerMove(move);
        TileManager.Instance.EndTurn();
    }
}

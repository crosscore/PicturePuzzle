using System.Collections.Generic;
using UnityEngine;

public class PicturePuzzleSceneDirector : MonoBehaviour
{
    [SerializeField] List<GameObject> pieces;
    [SerializeField] int ShuffleCount;

    // Save the initial positions
    List<Vector2> startPositions;

    // Selected piece (1st click)
    GameObject selectedPiece;

    // Flag to check whether we are selecting the second piece
    bool isSecondSelection = false;

    // ------------------------------------
    // Clear flag
    // ------------------------------------
    bool isCleared = false;

    void Start()
    {
        // Save the start positions
        startPositions = new List<Vector2>();
        foreach (var piece in pieces)
        {
            startPositions.Add(piece.transform.position);
        }

        // Shuffle the puzzle
        for (int i = 0; i < ShuffleCount; i++)
        {
            int randomIndexA = Random.Range(0, pieces.Count);
            int randomIndexB = Random.Range(0, pieces.Count);

            Vector2 temp = pieces[randomIndexA].transform.position;
            pieces[randomIndexA].transform.position = pieces[randomIndexB].transform.position;
            pieces[randomIndexB].transform.position = temp;

            Debug.Log($"Shuffle {i}: {pieces[randomIndexA].name} <-> {pieces[randomIndexB].name}");
        }
    }

    void Update()
    {
        // If the puzzle is already cleared, do nothing
        if (isCleared)
        {
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Mouse Button Up");

            // Mouse position in screen coordinates (pixels)
            Vector2 mouseScreenPosition = Input.mousePosition;
            Debug.Log($"Screen Position: {mouseScreenPosition}");

            // Convert screen position to world coordinates
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            Debug.Log($"World Position: {mouseWorldPosition}");

            // Raycast from world position
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);

            if (hit.collider != null)
            {
                // If this is the second piece selection
                if (isSecondSelection)
                {
                    GameObject swapPiece = hit.collider.gameObject;
                    SwapPieces(selectedPiece, swapPiece);

                    // Reset the flag
                    isSecondSelection = false;
                    Debug.Log($"Swapped: {selectedPiece.name} <-> {swapPiece.name}");

                    // Check if cleared
                    CheckClear();
                }
                else
                {
                    // First click: select the piece
                    selectedPiece = hit.collider.gameObject;
                    isSecondSelection = true;
                    Debug.Log($"Selected Piece: {selectedPiece.name}");
                }
            }
        }
    }

    /// <summary>
    /// Swap the position of two pieces
    /// </summary>
    /// <param name="pieceA">Piece A</param>
    /// <param name="pieceB">Piece B</param>
    void SwapPieces(GameObject pieceA, GameObject pieceB)
    {
        Vector3 tempPos = pieceA.transform.position;
        pieceA.transform.position = pieceB.transform.position;
        pieceB.transform.position = tempPos;
    }

    /// <summary>
    /// Check if all the pieces are in their start positions
    /// </summary>
    void CheckClear()
    {
        // Loop through each piece and compare positions
        for (int i = 0; i < pieces.Count; i++)
        {
            // If not in the correct position, return
            if ((Vector2)pieces[i].transform.position != startPositions[i])
            {
                return;
            }
        }

        // If we reach here, all pieces are in the correct position
        isCleared = true;
        Debug.Log("Puzzle Cleared!");
    }
}

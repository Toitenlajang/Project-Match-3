using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchCounter : MonoBehaviour
{
    // Dictionary to store TOTAL count of each type matched this turn (accumulated)
    public Dictionary<string, int> matchedTilesThisTurn = new Dictionary<string, int>();

    // Reference to board
    private Board board;

    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    // Call this to count all matched tiles before they're destroyed
    // This method now ACCUMULATES counts instead of replacing them
    public void CountMatchedTiles()
    {
        FindMatch findMatch = FindObjectOfType<FindMatch>();
        if (findMatch == null) return;

        // Loop through all matched pieces and ADD to existing counts
        foreach (GameObject matchedDot in findMatch.currentMatches)
        {
            if (matchedDot != null)
            {
                string tileType = matchedDot.tag;

                // Add to dictionary (accumulate)
                if (matchedTilesThisTurn.ContainsKey(tileType))
                {
                    matchedTilesThisTurn[tileType]++;
                }
                else
                {
                    matchedTilesThisTurn[tileType] = 1;
                }
            }
        }

        // Debug: Show current accumulated count
        Debug.Log("<color=cyan>=== ACCUMULATED MATCH COUNT ===</color>");
        foreach (var pair in matchedTilesThisTurn)
        {
            Debug.Log($"<color=cyan>{pair.Key}: {pair.Value} tiles (total)</color>");
        }
        Debug.Log("<color=cyan>=================================</color>");
    }

    // Clear the accumulated counts (call this when attacks are done)
    public void ClearMatchCounts()
    {
        matchedTilesThisTurn.Clear();
        Debug.Log("<color=yellow>Match counts cleared</color>");
    }

    // Get count of specific type
    public int GetMatchCount(string tileType)
    {
        if (matchedTilesThisTurn.ContainsKey(tileType))
        {
            return matchedTilesThisTurn[tileType];
        }
        return 0;
    }

    // Check if any tiles were matched
    public bool HasMatches()
    {
        return matchedTilesThisTurn.Count > 0;
    }

    // Get all matched types
    public List<string> GetAllMatchedTypes()
    {
        return new List<string>(matchedTilesThisTurn.Keys);
    }
}

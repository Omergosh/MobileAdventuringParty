using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    AdventurerScript selectedAdventurer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool SelectAdventurer(AdventurerScript newAdventurer)
    {
        // If successful (or this adventurer was already selected), return true.
        selectedAdventurer = newAdventurer;
        return true;

        // If unsuccessful, return false.
    }

    public void DeselectAdventurer()
    {
        selectedAdventurer = null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectingObjectScript : MonoBehaviour
{
    [SerializeField] private GatheringAI _selectedGatheringAI;
    [SerializeField] private GatheringBuilding _selectedGatheringBuilding;
    void Start()
    {
        
    }

    
    void Update()
    {
        // Check if the player has clicked the left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            // Create a ray from the camera to the mouse cursor
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Check if the ray hits anything
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.TryGetComponent(out GatheringAI ai))
                {
                    _selectedGatheringAI = ai;
                }
                // Check if the object hit is a gathering building
                else if (hit.collider.gameObject.TryGetComponent(out GatheringBuilding building) && _selectedGatheringAI != null)
                {
                    _selectedGatheringBuilding = building;
                    
                    // Assign the AI to the building
                    _selectedGatheringAI.SetGatheringBuilding(_selectedGatheringBuilding);
                    // Reset the selected AI
                    _selectedGatheringAI = null;
                    _selectedGatheringBuilding = null;
                }
            }
        }
    }
}

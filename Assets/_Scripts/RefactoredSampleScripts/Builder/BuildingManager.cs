using System;
using System.Collections.Generic;
using _Scripts.ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.RefactoredSampleScripts.Builder
{
    public enum ResourceType
    {
        Wood,
        Stone
    }

    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager instance;
        private readonly List<Building> allBuildings = new();
        public Building[] buildingPrefabs;
        public List<SO_Currency> currentResources = new();

        [SerializeField] private ParticleSystem buildParticle;
        [SerializeField] private ParticleSystem finishParticle;
        private BuildingUI ui;

        private void Awake()
        {
            instance = this;
        }

        public void SpawnBuilding(int index, Vector3 position)
        {
            var building = buildingPrefabs[index];
            var buildingCosts = building.GetBuildingCosts();

            // Here you would check if the resources are sufficient and then deduct them
            foreach (var cost in buildingCosts)
            {
                var resource = currentResources.Find(r => r.name == cost.name);
                if (resource != null)
                {
                    if (resource.Value >= cost.Value)
                    {
                        resource.Value -= cost.Value;
                    }
                    else
                    {
                        Debug.Log($"Not enough {resource.name}: have {resource.Value}, need {cost.Value}");
                        return;
                    }
                }
                else
                {
                    Debug.Log($"Resource {cost.name} not found");
                    return;
                }
            }
            
            // Instantiate the building as resources are sufficient
            Instantiate(building, position, Quaternion.identity);
        }


        public List<Building> GetBuildings()
        {
            return allBuildings;
        }

        public Building GetPrefab(int index)
        {
            return buildingPrefabs[index];
        }

        public Building GetRandomBuilding()
        {
            if (allBuildings.Count > 0)
                return allBuildings[Random.Range(0, allBuildings.Count)];
            return null;
        }

        public void RemoveBuilding(Building building)
        {
            allBuildings.Remove(building);
        }

        public void AddResource(SO_Currency newResource)
        {
            var resource = currentResources.Find(r => r.name == newResource.name);
            if (resource != null)
            {
                resource.Value += newResource.Value;
            }
            else
            {
                currentResources.Add(newResource);
            }
        }

        public void RemoveResource(SO_Currency resourceToRemove)
        {
            var resource = currentResources.Find(r => r.name == resourceToRemove.name);
            if (resource != null)
            {
                if (resource.Value >= resourceToRemove.Value)
                {
                    resource.Value -= resourceToRemove.Value;
                }
                else
                {
                    Debug.LogError($"Attempted to remove more {resource.name} than available. Available: {resource.Value}, Attempted to remove: {resourceToRemove.Value}");
                    // Do not set resource.Value to 0; just log the error.
                }
            }
        }

        public void UpdateResource(string resourceName, int newValue)
        {
            var resourceToUpdate = currentResources.Find(resource => resource.PresentableName == resourceName);
            if (resourceToUpdate != null) resourceToUpdate.Value = newValue;
        }

        public void PlayParticle(Vector3 position)
        {
            if (buildParticle)
            {
                buildParticle.transform.position = position;
                buildParticle.Play();
            }
        }
    }
}
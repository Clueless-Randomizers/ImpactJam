using System;
using System.Collections.Generic;
using _Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
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

		[SerializeField] private ParticleSystem buildParticle;
		[SerializeField] private ParticleSystem finishParticle;
		private BuildingUI ui;

		public UnityEvent<Building> OnBuilt;

		private void Awake()
		{
			GameManager.BuildingManager = this;
			instance = this;
		}

		public void SpawnBuilding ( Building building, Vector3 position ) {
			// Get purchase prices from building.cs
			PurchasePrice[] _purchasePrices = building.PurchasePrices;

			// Here you would check if the resources are sufficient and then deduct them
			if (GameManager.CurrencyManager.CanAffordPurchase( _purchasePrices )) {
				OnBuilt?.Invoke( building );


				GameManager.CurrencyManager.DeductPurchase( _purchasePrices );

				// Instantiate the building as resources are sufficient
				Instantiate( building, position, Quaternion.identity );
			}
			else 
			{
				// This is where we should display some sort of juice to show that the building is too expensive.
			}
		}

		public List<Building> GetBuildings()
		{
			return allBuildings;
		}

		public Building GetRandomBuilding()
		{
			if (allBuildings.Count > 0) { 
				return allBuildings[Random.Range(0, allBuildings.Count)];
			}
			return null;
		}

		public void RemoveBuilding(Building building)
		{
			allBuildings.Remove(building);
		}

		public void AddResource(SO_Currency newResource)
		{
			var resource = GameManager.CurrencyManager.GetCurrency( newResource.name );
			if (resource != null)
			{
				resource.Value += newResource.Value;
			}
		}

		public void RemoveResource(SO_Currency resourceToRemove)
		{
			var resource = GameManager.CurrencyManager.GetCurrency( resourceToRemove.name );
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
			var resourceToUpdate = GameManager.CurrencyManager.GetCurrency( resourceName );

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
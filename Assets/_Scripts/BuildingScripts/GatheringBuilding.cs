using System.Collections.Generic;
using UnityEngine;

public class GatheringBuilding : MonoBehaviour
{
	[SerializeField] private float _timeBetweenGathering;
	[SerializeField] private float _timeSpentGathering;
	[SerializeField] private SO_Currency _buildingCurrency;
	[SerializeField] private int _maxGatheringPeople = 5;
	List<GatheringAI> _gatheringPeople = new();

	private void Start () {
		_buildingCurrency = GameManager.CurrencyManager.GetCurrency( _buildingCurrency.name );
	}

	/// <summary>
	/// Get this building's time between gathering.
	/// </summary>
	public float WorkerRestTime { get { return _timeBetweenGathering; } }

	/// <summary>
	/// Get this building's time spent gathering.
	/// </summary>
	public float TimeSpentGathering { get { return _timeSpentGathering; } }

	/// <summary>
	/// Get this building's currency.
	/// </summary>
	public SO_Currency GetCurrency {  get { return _buildingCurrency; } }

	/// <summary>
	/// Add person to gathering position in building provided there is enough room.
	/// </summary>
	/// <param name="gatheringAI"></param>
	/// <returns></returns>
	public bool AddGatheringPerson ( GatheringAI gatheringAI ) {
		if ( _gatheringPeople.Count < _maxGatheringPeople && !_gatheringPeople.Contains( gatheringAI ) ) {
			_gatheringPeople.Add( gatheringAI );
			return true;
		} else {
			return false;
		};
	}
	/// <summary>
	/// Remove person from gathering position in building.
	/// </summary>
	/// <param name="gatheringAI"></param>
	public void RemoveGatheringPerson( GatheringAI gatheringAI ) {
		if ( _gatheringPeople.Contains( gatheringAI ) ) {
			_gatheringPeople.Remove( gatheringAI );
		}
	}
	/// <summary>
	/// Add to the currency being gathered.
	/// </summary>
	/// <param name="currency"></param>
	public void AddCurrency (int currency) {
		_buildingCurrency.Value = currency;
	}

	/// <summary>
	/// Get Worker's hat-material.
	/// </summary>
	/// <returns></returns>
	public Material GetHatMaterial() { 
		return _gathererHatMaterial;
	}
}

using UnityEngine;
using UnityEngine.AI;

[RequireComponent( typeof( NavMeshAgent ) )]
public class GatheringAI : MonoBehaviour
{
	[SerializeField] GatheringBuilding _gatheringBuilding = default;
	[SerializeField] private int _maxFoundResources = 5;
	[SerializeField] private static float _mapSize = 400f;

	float _nextGatheringRun = 0f;
	float _gatheringTimeout = 20f;
	float _timeSpentGathering = 30f;
	float _nextReturnTime = 0f;

	bool _isGathering = false;
	bool _hasResources = false;
	
	int _carryingResources = 0;

	NavMeshAgent _agent;

	private void Start () {
		_agent = transform.GetComponent<NavMeshAgent>();
		_agent.isStopped = false;

		if ( _gatheringBuilding != default) {
			SetGatheringBuilding( _gatheringBuilding );
		}

		_nextGatheringRun = GetNextTime( _gatheringTimeout, 1.75f );
	}

	void Update()
	{
		if ( _hasResources ) {
			return;
		}

		if ( _isGathering ) {
			CheckIfDoneGathering();
		} else {
			if ( _nextGatheringRun < Time.realtimeSinceStartup ) {
				StartGathering();
			} 
		}
	}

	/// <summary>
	/// Checks if the character is done gathering:
	///		If Yes: redirects the character to the resource building, adjusts state-booleans, assigns how many resources the character managed to find;
	///		If No: Does nothing;
	/// </summary>
	private void CheckIfDoneGathering () {
		if ( _nextReturnTime < Time.realtimeSinceStartup) {
			GoToDestination(_gatheringBuilding.transform.position);
			_hasResources = true;
			_isGathering = false;
			_carryingResources = ( int ) 1 * Random.Range( 1 , _maxFoundResources );
		}
	}

	/// <summary>
	/// Tells NavMeshAgent SetDestination and isStopped = false.
	/// </summary>
	/// <param name="position"></param>
	private void GoToDestination ( Vector3 position ) {
		_agent.SetDestination(position);
		_agent.isStopped = false;
	}

	/// <summary>
	/// Starts the gathering procedures.
	/// </summary>
	private void StartGathering () {
		_isGathering = true;
		_nextReturnTime = Time.realtimeSinceStartup + GetNextTime( _timeSpentGathering, 1.75f );
		GoToDestination( GetRandomPosition() );
	}
	/// <summary>
	/// Access point for a random location.
	/// </summary>
	/// <returns></returns>
	private static Vector3 GetRandomPosition () {
		return new Vector3( Random.Range( -1 * _mapSize, _mapSize ), 0f, Random.Range( -1 * _mapSize, _mapSize ) );
	}

	/// <summary>
	/// Assigns this character's GatheringBuilding, updates _gatheringTimeout, updates _timeSpentGathering
	/// </summary>
	/// <param name="gatheringBuilding"></param>
	public void SetGatheringBuilding(GatheringBuilding gatheringBuilding) {
		if ( gatheringBuilding.AddGatheringPerson( this ) ) {
			_gatheringBuilding = gatheringBuilding;

			_gatheringTimeout = _gatheringBuilding.GatheringTime;

			// Divided by 2 to make the variable account for the whole time the character is spending gathering.
			_timeSpentGathering = _gatheringBuilding.TimeToGather / 2; 
		}
	}

	/// <summary>
	/// Checks if you have entered the range of your _gatheringBuilding: deposits the resources, resets the gathering state, and assigns the next time to gather.
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerEnter ( Collider other ) {
		if ( _hasResources && other.transform.TryGetComponent(out GatheringBuilding gatheringBuilding) && gatheringBuilding  == _gatheringBuilding) {
			_gatheringBuilding.AddCurrency(_carryingResources);
			_carryingResources = 0;

			_agent.isStopped = true;
			_hasResources = false;

			_nextGatheringRun = GetNextTime( _gatheringTimeout, 1.75f );
		}
	}
	/// <summary>
	/// Creates a time in the future based on timeout and randomMultiplicator.
	/// </summary>
	/// <param name="timeout"></param>
	/// <param name="randomMultiplicator"></param>
	/// <returns></returns>
	private float GetNextTime ( float timeout, float randomMultiplicator = 1 ) {
		return Time.realtimeSinceStartup + (timeout * Random.Range(1f, randomMultiplicator ) );
	}
}

using UnityEngine;
using UnityEngine.AI;

[RequireComponent( typeof( NavMeshAgent ) )]
public class GatheringAI : MonoBehaviour
{
	[SerializeField] GatheringBuilding _gatheringBuilding = default;
	[SerializeField] private int _maxFoundResources = 5;

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

		_nextGatheringRun = Time.realtimeSinceStartup + _gatheringTimeout;
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

	private void CheckIfDoneGathering () {
		if ( _nextReturnTime < Time.realtimeSinceStartup) {
			GoToDestination(_gatheringBuilding.transform.position);
			_hasResources = true;
			_isGathering = false;
			_carryingResources = ( int ) 1 * Random.Range( 1 , _maxFoundResources );
		}
	}

	private void GoToDestination ( Vector3 position ) {
		_agent.SetDestination(position);
		_agent.isStopped = false;
	}

	private void StartGathering () {
		_isGathering = true;
		_nextReturnTime = Time.realtimeSinceStartup + _timeSpentGathering;
		GoToDestination( GetRandomPosition() );
	}

	private static Vector3 GetRandomPosition () {
		return new Vector3( Random.Range( -400f, 400f ), 0f, Random.Range( -400f, 400f ) );
	}

	public void SetGatheringBuilding(GatheringBuilding gatheringBuilding) {
		if ( gatheringBuilding.AddGatheringPerson( this ) ) {
			_gatheringBuilding = gatheringBuilding;

			_gatheringTimeout = _gatheringBuilding.GatheringTime;
			_timeSpentGathering = _gatheringBuilding.TimeToGather;
		}
	}

	private void OnTriggerEnter ( Collider other ) {
		if ( _hasResources && other.transform.TryGetComponent(out GatheringBuilding gatheringBuilding) && gatheringBuilding  == _gatheringBuilding) {
			_gatheringBuilding.AddCurrency(_carryingResources);
			_carryingResources = 0;

			_agent.isStopped = true;
			_hasResources = false;

			_nextGatheringRun = Time.realtimeSinceStartup + _gatheringTimeout;
		}
	}
}

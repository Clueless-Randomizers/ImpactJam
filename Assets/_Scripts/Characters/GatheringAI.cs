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
	}

	void Update()
    {
        if ( !_hasResources && !_isGathering && _nextGatheringRun < Time.realtimeSinceStartup) {
			_nextGatheringRun = Time.realtimeSinceStartup + _gatheringTimeout;
			_isGathering = true;
			StartGathering();
		}
		CheckIfDoneGathering();
    }

	private void CheckIfDoneGathering () {
		if ( _isGathering && _nextReturnTime < Time.realtimeSinceStartup) {
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
		// This is where I would have loved to have a "Forest Zone" and be able to tell my person to walk to a random place inside that zone. 
		// We'll just do random location.
		Vector3 _targetPosition = new Vector3(Random.Range( 0f, 885f),0f, Random.Range( 0f, 885f ) );

		_isGathering = true;
		_nextReturnTime = Time.realtimeSinceStartup + _timeSpentGathering;

		GoToDestination( _targetPosition );
	}

	public void SetGatheringBuilding(GatheringBuilding gatheringBuilding) {
		if ( gatheringBuilding.AddGatheringPerson( this ) ) {
			_gatheringBuilding = gatheringBuilding;

			_gatheringTimeout = _gatheringBuilding.GatheringTime;
			_timeSpentGathering = gatheringBuilding.TimeToGather;
		}
	}

	private void OnTriggerEnter ( Collider other ) {
		if ( _hasResources && other.transform.TryGetComponent(out GatheringBuilding gatheringBuilding)) {
			gatheringBuilding.AddCurrency(_carryingResources);
			_carryingResources = 0;
			_agent.SetDestination( default );
			_agent.isStopped = true;
			_hasResources = false;
		}
	}
}
/*
 * using System;
using UnityEngine;
using ProjectDawn.Navigation.Hybrid;

namespace ProjectDawn.Navigation.Sample.Scenarios
{
    [Obsolete("AgentDestinationAuthoring is for sample purpose, please create your own component that will handle agent high level logic.")]
    [RequireComponent(typeof(AgentAuthoring))]
    [DisallowMultipleComponent]
    internal class AgentDestinationAuthoring : MonoBehaviour
    {
        public Transform Target;
        public float Radius;
        public bool EveryFrame;

        private void Start()
        {
            var agent = transform.GetComponent<AgentAuthoring>();
            var body = agent.EntityBody;
            body.Destination = Target.position;
            body.IsStopped = false;
            agent.EntityBody = body;
        }

        void Update()
        {
            if (!EveryFrame)
                return;
            Start();
        }
    }
}
*/
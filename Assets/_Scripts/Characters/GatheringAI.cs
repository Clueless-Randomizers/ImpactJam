using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatheringAI : MonoBehaviour
{
	[SerializeField] GatheringBuilding _gatheringBuilding = default;
	float _nextGatheringRun = 0f;
	float _gatheringTimeout = 20f;
	float _timeSpentGathering = 30f;
	bool _shouldGather = false;
	bool _isGathering = false;


    void Update()
    {
        if ( _shouldGather && !_isGathering && _nextGatheringRun < Time.realtimeSinceStartup) {
			_nextGatheringRun = Time.realtimeSinceStartup + _gatheringTimeout;
			_isGathering = true;
			StartGathering();
		}
		CheckIfDoneGathering();
    }

	private void CheckIfDoneGathering () {
		
	}

	private void StartGathering () {
		// This is where I would have loved to have a "Forest Zone" and be able to tell my person to walk to a random place inside that zone. 
		// We'll just do random location.

	}

	public void SetGatheringBuilding(GatheringBuilding gatheringBuilding) {
		_gatheringBuilding = gatheringBuilding;
		_gatheringTimeout = _gatheringBuilding.GatheringTime;
		_timeSpentGathering = gatheringBuilding.TimeToGather;
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
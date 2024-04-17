using _Scripts.RefactoredSampleScripts.Agent;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Scripts.RefactoredSampleScripts
{
    public class Utility : MonoBehaviour
    {
        public static Vector3 MouseToTerrainPosition()
        {
            Vector3 position = Vector3.zero;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit info, 100, LayerMask.GetMask("Level")))
                position = info.point;
            return position;
        }
        public static RaycastHit CameraRay()
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit info, 100))
                return info;
            return new RaycastHit();
        }

		public static Actor MouseToActor () {
			Actor _actor = default;

			if (Physics.Raycast( Camera.main.ScreenPointToRay( Input.mousePosition ), out RaycastHit info, 500, LayerMask.GetMask( "Player" ) )) {
				if (info.transform.TryGetComponent(out Actor actor)) {
					_actor = actor;
				}
			}

			return _actor;
		}
	}
}

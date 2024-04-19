using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DelayedNextLevel : MonoBehaviour
{
	[SerializeField] float _nextLevelDelay = 15f;
	[SerializeField] SceneReference _nextLevelScene;
	private float _nextLevelTime;

	private void Awake () {
		_nextLevelTime = Time.realtimeSinceStartup + _nextLevelDelay;
	}

	void Update()
    {
		if (_nextLevelTime < Time.realtimeSinceStartup) {
			SceneManager.LoadScene(_nextLevelScene.Name);
		}
    }
}

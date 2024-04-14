using UnityEngine;

public class DisableWhenWebGL : MonoBehaviour
{
    void Awake()
    {
		if ( Application.platform == RuntimePlatform.WebGLPlayer ) {
			gameObject.SetActive( false );
		}   
    }
}

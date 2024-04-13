using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This ScriptableObject is the base for any currency in this game.
/// </summary>
[Serializable]
[CreateAssetMenu( menuName = "ScriptableAssets/New Currency" )]
public class SO_Currency : ScriptableObject
{
	[SerializeField] int _currentCurrency = 0;
	[SerializeField] Sprite _currencyImage;
	/// <summary>
	/// Icon used for interface.
	/// </summary>
	public Sprite Icon { get { return _currencyImage; } }

	/// <summary>
	/// Subscribe to currency changes.
	/// </summary>
	public UnityEvent CurrencyChange;

	/// <summary>
	/// This variable always uses addition.
	/// </summary>
	public int Value { 
		get { return _currentCurrency; } 
		set { 
			int _newValue = _currentCurrency + value;  
			int _oldValue = _currentCurrency;

			_currentCurrency = ( _newValue > 0) ? _newValue : 0;

			if ( _currentCurrency != _oldValue ) {
				CurrencyChange?.Invoke();
			}
		}
	}

	/// <summary>
	/// Returns string representation of name without (clone) 
	/// </summary>
	public string PresentableName { get { return name.Split( "(" )[ 0 ]; }  }
}

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
	public Sprite Icon { get { return _currencyImage; } }
	public UnityEvent CurrencyChange;
	public int Value { 
		get { return _currentCurrency; } 
		set { 
			int _newValue = _currentCurrency + value;  
			if (_currentCurrency != _newValue) {
				CurrencyChange?.Invoke();
			} 
			_currentCurrency = ( _newValue > 0) ? _newValue : 0; 
		} 
	}

}

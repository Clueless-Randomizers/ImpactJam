using System;
using UnityEngine;

/// <summary>
/// This baseclass allows for multiple types of Currency to be included in the system
/// </summary>
[Serializable]
[CreateAssetMenu( menuName = "ScriptableAssets/New Currency" )]
public class Currency : ScriptableObject
{
	[SerializeField] int _currentCurrency = 0;
	[SerializeField] Sprite _currencyImage;
	[SerializeField] CurrencyType _currencyType;
	public int Value { 
		get { return _currentCurrency; } 
		set { int _newValue = _currentCurrency + value;  _currentCurrency = ( _newValue > 0) ? _newValue : 0; } 
	}
}
[Serializable]
public enum CurrencyType {
	Food,
	Wood,
	Heritage
}

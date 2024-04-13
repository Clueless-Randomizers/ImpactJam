using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
	[SerializeField] private SO_Currency[] _currencies;
	[SerializeField] private GameObject _currencyPrefab;
	private Dictionary<string, SO_Currency> _currencyRegister = new();

	// Start is called before the first frame update
	private void Awake()
	{
		// Register resource Manager so others can use it easily.
		GameManager.CurrencyManager = this;

		// This for-loop should hook up all the currency updating and stuff.
		foreach (SO_Currency currency in _currencies) {
			SO_Currency _instantiatedCurrency = Instantiate( currency, null );
			GameObject _newCurrencyUI = Instantiate(_currencyPrefab, transform);
			string _currencyKey = _instantiatedCurrency.name.Split( "(" )[ 0 ];

			_currencyRegister.Add( _currencyKey, _instantiatedCurrency );

			if ( _newCurrencyUI.TryGetComponent( out CurrencyDisplayManager currencyDisplayController)) {
				currencyDisplayController.SetCurrency( _instantiatedCurrency );
			}
		}
		Debug.Log(GameManager.CurrencyManager.GetCurrency("Food"));
	}

	public SO_Currency GetCurrency (string name) {
		return (_currencyRegister.ContainsKey( name ) ) ? _currencyRegister[ name ] :default;
	}
}

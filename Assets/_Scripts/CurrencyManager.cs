using System.Collections.Generic;
using _Scripts.ScriptableObjects;
using UnityEngine;

namespace _Scripts
{
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
			for ( int i = 0; i < _currencies.Length; i++ ) {
				SO_Currency currency = _currencies[ i ];
			
				SO_Currency _instantiatedCurrency = Instantiate( currency, null );
				GameObject _newCurrencyUI = Instantiate( _currencyPrefab, transform );

				_currencies[ i ] = _instantiatedCurrency;

				string _currencyKey = _instantiatedCurrency.PresentableName;
				_currencyRegister.Add( _currencyKey, _instantiatedCurrency );

				if ( _newCurrencyUI.TryGetComponent( out CurrencyDisplayManager currencyDisplayController ) ) {
					currencyDisplayController.SetCurrency( _instantiatedCurrency );
				}
			}
		}

		public SO_Currency GetCurrency (string name) {
			return (_currencyRegister.ContainsKey( name ) ) ? _currencyRegister[ name ] :default;
		}
	}
}

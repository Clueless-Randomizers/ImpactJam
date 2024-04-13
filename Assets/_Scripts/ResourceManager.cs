using UnityEngine;

public class ResourceManager : MonoBehaviour
{
	[SerializeField] private SO_Currency[] _currencies;
	[SerializeField] private GameObject _currencyPrefab;
	

	// Start is called before the first frame update
	void Start()
	{
		// This for-loop should hook up all the currency updating and stuff.
		foreach (SO_Currency currency in _currencies) {
			SO_Currency _instantiatedCurrency = Instantiate( currency, null );
			GameObject _newCurrencyUI = Instantiate(_currencyPrefab, transform);
			if ( _newCurrencyUI.TryGetComponent( out CurrencyDisplayManager currencyDisplayController)) {
				currencyDisplayController.SetCurrency( _instantiatedCurrency );
			}
		}
	}
}

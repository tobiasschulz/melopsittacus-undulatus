using System;
using Android.Locations;
using Android.Content;
using System.Collections.Generic;
using System.Linq;
using Core.Common;

namespace MelopsittacusUndulatus
{
	public class LocationListener : Java.Lang.Object, ILocationListener
	{
		Location _currentLocation;
		LocationManager _locationManager;
		String _locationProvider;

		public Action<Location> LocationChanged = loc => Log.Info ("location: ", loc.Latitude, ",", loc.Longitude);

		public LocationListener (Context context)
		{
			InitializeLocationManager (context);
		}

		void InitializeLocationManager (Context context)
		{
			try {
				_locationManager = (LocationManager)context.GetSystemService (Context.LocationService);
				Criteria criteriaForLocationService = new Criteria {
					Accuracy = Accuracy.Fine
				};
				IList<string> acceptableLocationProviders = _locationManager.GetProviders (criteriaForLocationService, true);

				if (acceptableLocationProviders.Any ()) {
					_locationProvider = acceptableLocationProviders.First ();
				} else {
					_locationProvider = String.Empty;
				}

				long minTime = 10 * 60 * 1000;
				float minDistance = 0;

				_locationManager.RequestLocationUpdates (provider: _locationProvider, minTime: minTime, minDistance: minDistance, listener: this);
			} catch (Exception ex) {
				Log.Error (ex);
			}
		}

		public void Destroy ()
		{
			_locationManager.RemoveUpdates (this);
		}

		#region ILocationListener implementation

		public void OnLocationChanged (Location location)
		{
			_currentLocation = location;
			if (_currentLocation == null) {
				Log.Error ("Unable to determine your location.");
			} else {
				LocationChanged (location);
			}
		}

		public void OnProviderDisabled (string provider)
		{
			Log.Debug (GetType ().Name, ": OnProviderDisabled: ", provider);
		}

		public void OnProviderEnabled (string provider)
		{
			Log.Debug (GetType ().Name, ": OnProviderEnabled: ", provider);
		}

		public void OnStatusChanged (string provider, Availability status, Android.OS.Bundle extras)
		{
			Log.Debug (GetType ().Name, ": OnStatusChanged: ", provider);
		}

		#endregion
	}
}


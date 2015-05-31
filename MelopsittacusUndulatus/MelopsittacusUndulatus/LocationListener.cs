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
		int countLocations = 0;
		int countErrors = 0;

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
					Accuracy = Accuracy.Medium,
					PowerRequirement = Power.Low,
				};
				IList<string> acceptableLocationProviders = _locationManager.GetProviders (criteriaForLocationService, true);

				if (acceptableLocationProviders.Any ()) {
					_locationProvider = acceptableLocationProviders.First ();
				} else {
					_locationProvider = LocationManager.PassiveProvider;
				}
				Log.Debug (GetType ().Name, ": providers: ", acceptableLocationProviders.Join (", "));

				const long minTime = 0;// : (10 * 60 * 1000);
				const float minDistance = 0;

				_locationManager.RequestLocationUpdates (provider: _locationProvider, minTime: minTime, minDistance: minDistance, listener: this);
			} catch (Exception ex) {
				Log.Error (ex);
				Destroy ();
			}
		}

		public void Destroy ()
		{
			Log.Debug (GetType ().Name, ": Destroy!");
			if (_locationManager != null) {
				_locationManager.RemoveUpdates (this);
				_locationManager = null;
			}
			countLocations = 0;
			countErrors = 0;
			LocationChanged = loc => Log.Error (GetType ().Name, ": LocationChanged handler: has been destroyed");
		}

		#region ILocationListener implementation

		public void OnLocationChanged (Location location)
		{
			_currentLocation = location;


			if (_currentLocation == null) {
				countErrors++;
				// print error
				Log.Error (GetType ().Name, ": Unable to determine your location.");
				if (countErrors >= 3) {
					// destroy
					Destroy ();
				}
			} else {
				countLocations++;
				Log.Debug (GetType ().Name, ": countLocations: ", countLocations);
				if (countLocations >= 1) {
					// send location to caller
					LocationChanged (location);
					// destroy
					Destroy ();
				}
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


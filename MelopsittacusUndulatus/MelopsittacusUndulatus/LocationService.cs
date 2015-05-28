using System;
using Android.App;
using Core.Common;
using Android.Content;

namespace MelopsittacusUndulatus
{
	[Service ()]
	public class LocationService : Service
	{
		static LocationService ()
		{
			Core.Platform.Android.AndroidPlatform.Start ();
		}

		LocationListener locationListener;

		public override void OnCreate ()
		{
			base.OnCreate ();
			Log.Debug (GetType ().Name, ": created");

			locationListener = new LocationListener (context: this);
			locationListener.LocationChanged += OnLocationChanged;

		}

		void OnLocationChanged (Android.Locations.Location location)
		{
			string locStr = String.Format ("{0},{1}", location.Latitude, location.Longitude);
			Log.Debug (locStr);
		}

		public override void OnDestroy ()
		{
			Log.Debug (GetType ().Name, ": destroy");
			if (locationListener != null) {
				locationListener.LocationChanged -= OnLocationChanged;
				locationListener.Destroy ();
			}
			base.OnDestroy ();
		}

		public override void OnLowMemory ()
		{
			Log.Debug (GetType ().Name, ": low memory");
			base.OnLowMemory ();
		}

		[Obsolete ()]
		public override StartCommandResult OnStartCommand (Android.Content.Intent intent, StartCommandFlags flags, int startId)
		{
			Log.Debug (GetType ().Name, ": started");

			AlarmReceiver.SetupAlarm (context: this);

			return StartCommandResult.Sticky;
		}

		#region implemented abstract members of Service

		public override Android.OS.IBinder OnBind (Intent intent)
		{
			return null;
		}

		#endregion
	}
}


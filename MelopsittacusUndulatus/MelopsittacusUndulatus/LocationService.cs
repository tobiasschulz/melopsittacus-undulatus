using System;
using Android.App;
using Android.Content;
using Core.Common;
using Core.IO;
using Core.Portable;
using Core.Math;

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
		PortableLocationCollection locationCollection = new PortableLocationCollection ();

		public override void OnCreate ()
		{
			base.OnCreate ();
			Log.Debug (GetType ().Name, ": created");

			locationListener = new LocationListener (context: this);
			locationListener.LocationChanged += OnLocationChanged;

			LoadFile ();
		}

		void OnLocationChanged (Android.Locations.Location location)
		{
			string locStr = String.Format ("{0},{1}", location.Latitude, location.Longitude);
			Log.Debug (locStr);
			SaveFile ();
		}

		public override void OnDestroy ()
		{
			Log.Debug (GetType ().Name, ": destroy");
			SaveFile ();
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

		public void LoadFile ()
		{
			try {
				locationCollection = ConfigHelper.OpenConfig<PortableLocationCollection> (fullPath: LocationFile);
			} catch (Exception ex) {
				Log.FatalError ("Error in ", GetType ().Name, ".LoadFile: ", LocationFile, ": ", ex);
			}
		}

		public void SaveFile ()
		{
			try {
				ConfigHelper.SaveConfig (fullPath: LocationFile, stuff: locationCollection);
			} catch (Exception ex) {
				Log.FatalError ("Error in ", GetType ().Name, ".SaveFile: ", LocationFile, ": ", ex);
			}
		}

		public string LocationFile {
			get { 
				return System.IO.Path.Combine (PlatformInfo.System.WorkingDirectory, "locations.txt");
			}
		}
	}
}


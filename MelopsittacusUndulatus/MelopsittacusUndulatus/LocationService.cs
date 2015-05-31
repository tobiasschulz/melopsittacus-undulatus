using System;
using Android.App;
using Android.Content;
using Core.Common;
using Core.IO;
using Core.Portable;
using Core.Math;
using System.Linq;

namespace MelopsittacusUndulatus
{
	[Service ()]
	public class LocationService : Service
	{
		static LocationService ()
		{
			Core.Platform.Android.AndroidPlatform.Start ();
		}

		const int notificationId = 0;
		const int pendingIntentId = 0;
		readonly object lockListeningForLocations = new object ();

		PortableLocationCollection locationCollection = new PortableLocationCollection ();
		bool listeningForLocations = false;

		public override void OnCreate ()
		{
			base.OnCreate ();
			Log.Debug (GetType ().Name, ": created");

			listenForLocations ();
		}

		void listenForLocations ()
		{
			lock (lockListeningForLocations) {
				if (listeningForLocations == false) {
					listeningForLocations = true;
					LocationListener locationListener = new LocationListener (context: this);
					locationListener.LocationChanged += OnLocationChanged;
				}
			}
		}

		void OnLocationChanged (Android.Locations.Location androidLocation)
		{
			LoadFile ();
			PortableLocation location = new PortableLocation {
				Latitude = androidLocation.Latitude,
				Longitude = androidLocation.Longitude,
				Altitude = androidLocation.Altitude,
				DateTime = DateTimeExtensions.MillisToDateTimeOffset (milliSecondsSinceEpoch: androidLocation.Time, offsetMinutes: 0).UtcDateTime,
				Provider = androidLocation.Provider,
			};
			locationCollection.AddLocation (location);
			Log.Debug (GetType ().Name, ": OnLocationChanged: ", location.ToJson (inline: true));
			SaveFile ();

			CreateNotification (location);

			lock (lockListeningForLocations) {
				listeningForLocations = false;
			}
		}

		void CreateNotification (PortableLocation location)
		{
			// Set up an intent so that tapping the notifications returns to the app
			Intent intent = new Intent (this, typeof(MainActivity));

			// Create a PendingIntent; we're only using one PendingIntent (ID = 0):
			PendingIntent pendingIntent = PendingIntent.GetActivity (this, pendingIntentId, intent, PendingIntentFlags.OneShot);
			
			string text = location.Latitude + " / " + location.Longitude;
			// Instantiate the builder and set notification elements
			Notification.Builder builder = new Notification.Builder (this)
				.SetContentIntent (pendingIntent)
				.SetContentTitle ("Location")
				.SetContentText (text)
			                               //.SetDefaults (NotificationDefaults.Vibrate)
				.SetSmallIcon (Resource.Drawable.Icon);

			// Build the notification
			Notification notification = builder.Build ();

			// Get the notification manager
			NotificationManager notificationManager = GetSystemService (Context.NotificationService) as NotificationManager;

			// Publish the notification
			notificationManager.Notify (notificationId, notification);
		}

		public override void OnDestroy ()
		{
			Log.Debug (GetType ().Name, ": destroy");
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

			listenForLocations ();

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


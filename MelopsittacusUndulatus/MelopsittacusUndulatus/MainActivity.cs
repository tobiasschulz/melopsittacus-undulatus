using System;

using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Core.Common;
using Core.Platform.Android;

namespace MelopsittacusUndulatus
{
	[Activity (Label = "Melopsittacus Undulatus", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		static MainActivity ()
		{
			AndroidPlatform.Start ();
		}

		int count = 1;

		TextView _locationText;
		TextView _logText;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			BootReceiver.Enable (context: this);
			StartService (new Intent (this, typeof(LocationService)));

			_locationText = FindViewById<TextView> (Resource.Id.location_text);
			_logText = FindViewById<TextView> (Resource.Id.log);


			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);

			updateLog ();
			
			button.Click += (sender, e) => updateLog ();
		}

		void updateLog ()
		{
			_logText.Text = string.Format ("{0} clicks!", count++)
			+ "\n"
			+ string.Join ("\n", AndroidPlatform.CapturedLogMessages.Reverse ());
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			Log.LogHandler += updateLog_LogHandler;
		}

		protected override void OnPause ()
		{
			Log.LogHandler -= updateLog_LogHandler;

			base.OnPause ();
		}

		void updateLog_LogHandler (Log.Type type, IEnumerable<string> messages)
		{
			updateLog ();
		}
	}
}



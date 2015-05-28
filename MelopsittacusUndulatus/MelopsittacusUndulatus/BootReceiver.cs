using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Core.Common;

namespace MelopsittacusUndulatus
{
	[BroadcastReceiver]
	[IntentFilter (new[] { Android.Content.Intent.ActionBootCompleted })]
	public class BootReceiver : BroadcastReceiver
	{
		public static void Enable (Context context)
		{
			ComponentName receiver = new ComponentName (context, Java.Lang.Class.FromType (typeof(BootReceiver)));
			PackageManager pm = context.PackageManager;

			pm.SetComponentEnabledSetting (receiver, ComponentEnabledState.Enabled, ComponentEnableOption.DontKillApp);
		}

		public override void OnReceive (Context context, Intent intent)
		{
			if (intent.Action == "android.intent.action.BOOT_COMPLETED") {
				
				Log.Info ("\r\nreceived boot broadcast.");

				// Set the alarm here.
				AlarmReceiver.SetupAlarm (context: context);

				Intent startIntent = new Intent (context, typeof(LocationService));
				context.StartService (startIntent);
			}
		}
	}
}


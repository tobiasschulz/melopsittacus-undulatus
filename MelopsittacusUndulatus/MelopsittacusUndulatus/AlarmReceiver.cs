using System;
using Android.App;
using Core.Common;
using Android.Content;

namespace MelopsittacusUndulatus
{
	[BroadcastReceiver]
	[IntentFilter (new[] { AlarmReceiver.ALARM_TRIGGER })]
	public class AlarmReceiver : BroadcastReceiver
	{
		public const string ALARM_TRIGGER = "ALARM_TRIGGERED";

		static AlarmManager alarmMgr;

		public static void SetupAlarm (Context context)
		{
			alarmMgr = (AlarmManager)context.GetSystemService (Context.AlarmService);
			Intent intent = new Intent (context, typeof(AlarmReceiver));

			bool isAlarmSet = PendingIntent.GetBroadcast (context, 0, intent, PendingIntentFlags.NoCreate) != null;

			if (!isAlarmSet) {
				var alarmIntent = PendingIntent.GetBroadcast (context, 0, intent, PendingIntentFlags.UpdateCurrent);

				alarmMgr.SetInexactRepeating (AlarmType.ElapsedRealtimeWakeup, AlarmManager.IntervalHalfHour, AlarmManager.IntervalHalfHour, alarmIntent);
			}
		}


		public override void OnReceive (Context context, Intent intent)
		{
			Console.WriteLine ("\r\nreceived alarm broadcast.\r\n");
			Intent startIntent = new Intent (context, typeof(LocationService));
			context.StartService (startIntent);
		}
	}

}


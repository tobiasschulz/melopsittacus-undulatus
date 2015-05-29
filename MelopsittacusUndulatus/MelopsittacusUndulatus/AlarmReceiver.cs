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
		internal const long IntervalMilliseconds = 20 * 60 * 1000;

		static AlarmManager alarmMgr;
		static bool isAlarmSetOnce = false;

		public static void SetupAlarm (Context context)
		{
			alarmMgr = (AlarmManager)context.GetSystemService (Context.AlarmService);
			Intent intent = new Intent (context, typeof(AlarmReceiver));

			bool isAlarmSet = PendingIntent.GetBroadcast (context, 0, intent, PendingIntentFlags.NoCreate) != null;

			if (!isAlarmSet || !isAlarmSetOnce) {
				var alarmIntent = PendingIntent.GetBroadcast (context, 0, intent, 0);

				//alarmMgr.SetInexactRepeating (AlarmType.ElapsedRealtimeWakeup, AlarmManager.IntervalHalfHour, AlarmManager.IntervalHalfHour, alarmIntent);
				alarmMgr.Cancel (alarmIntent);

				long millisecondsSinceMidnight = (long)DateTime.Now.TimeOfDay.TotalMilliseconds;
				long firstAlarm = IntervalMilliseconds - (millisecondsSinceMidnight % IntervalMilliseconds);

				Log.Debug ("set alarm: interval: ", (double)IntervalMilliseconds / 1000.0, " sec, first alarm: ", (double)firstAlarm / 1000.0, " sec");
				alarmMgr.SetInexactRepeating (AlarmType.RtcWakeup, firstAlarm, IntervalMilliseconds, alarmIntent);
				isAlarmSetOnce = true;
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


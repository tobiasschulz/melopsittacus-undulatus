using System;
using Core.Common;
using AndroidLog = Android.Util.Log;
using System.Collections.Generic;

namespace Core.Platform.Android
{
	public static class AndroidPlatform
	{
		internal static readonly int LOG_TYPE_LENGTH = 7 + 2;
		internal static string TAG = "CORE.LOG";

		internal static bool enabled = false;

		internal static FixedSizedQueue<string> queue = new FixedSizedQueue<string> (100);

		public static void Start ()
		{
			if (!enabled) {
				enabled = true;

				Log.LogHandler += (type, messageLines) => {
					foreach (string _message in messageLines) {
						string message = string.Format ("{0:yyyyMMdd-HHmmss} {1} {2}", DateTime.Now, formatType (type), _message);
						queue.Enqueue (message);

						if (type == Log.Type.ERROR || type == Log.Type.FATAL_ERROR) {
							AndroidLog.Error (TAG, message);
						} else {
							AndroidLog.Debug (TAG, message);
						}
					}
				};

				Log.Debug (typeof(AndroidPlatform).Name + ": started");
			}
		}

		static string formatType (Log.Type type)
		{
			return string.Format ("[{0}]", type).PadRight (LOG_TYPE_LENGTH);
		}

		public static IEnumerable<string> CapturedLogMessages {
			get {
				foreach (string message in queue) {
					yield return message;
				}
			}
		}
	}
}


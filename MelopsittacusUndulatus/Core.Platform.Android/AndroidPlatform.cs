using System;
using System.Collections.Generic;
using Core.Common;
using Core.Portable;
using AndroidEnvironment = Android.OS.Environment;
using AndroidLog = Android.Util.Log;

namespace Core.Platform.Android
{
	public static class AndroidPlatform
	{
		internal static readonly int LOG_TYPE_LENGTH = 7 + 2;
		internal static string TAG = "CORE.LOG";

		internal static bool enabled = false;

		internal static FixedSizedQueue<string> queue = new FixedSizedQueue<string> (500);

		public static void Start ()
		{
			if (!enabled) {
				enabled = true;

				Assign ();

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

		static void Assign ()
		{
			PlatformInfo.System = new AndroidInfo ();
			PlatformInfo.User = new AndroidInfo ();
		}
	}

	public sealed class AndroidInfo : ISystemInfo, IUserInfo
	{
		#region ISystemInfo implementation

		public ModernOperatingSystem OperatingSystem {
			get {
				return ModernOperatingSystem.Android;
			}
		}

		public bool IsRunningFromNUnit {
			get {
				return false;
			}
		}

		public string SDCardDirectory {
			get {
				return AndroidEnvironment.ExternalStorageDirectory.AbsolutePath;
			}
		}

		public string ApplicationPath {
			get {
				return SDCardDirectory;
			}
		}

		public string WorkingDirectory {
			get {
				return SDCardDirectory;
			}
		}

		public bool IsInteractive {
			get {
				return false;
			}
		}

		#endregion

		#region IUserInfo implementation

		public string UserFullName {
			get {
				return "User";
			}
		}

		public string UserShortName {
			get {
				return "android-user";
			}
		}

		public string HostName {
			get {
				return "android-host";
			}
		}

		public string UserMail {
			get {
				return null;
			}
		}

		public string HomeDirectory {
			get {
				return SDCardDirectory;
			}
		}

		#endregion
	}
}


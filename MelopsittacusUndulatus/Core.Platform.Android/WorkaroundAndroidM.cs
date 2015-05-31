using System;
using Android.Runtime;

namespace Core.Platform.Android
{
	public static class WorkaroundAndroidM
	{
		public static Java.Lang.String ToAndroidString (this string str)
		{
			return Java.Lang.Object.GetObject<Java.Lang.String> (JNIEnv.NewString (str), JniHandleOwnership.TransferLocalRef);
		}
	}
}


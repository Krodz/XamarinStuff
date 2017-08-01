using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using TK.CardIO.Android;
using Gcm.Client;

namespace WalletScanning.Droid
{
	[Activity(Label = "WalletScanning.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		public static MainActivity CurrentActivity { get; private set; }

		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);
			CurrentActivity = this;
	        try
	        {
	            GcmClient.CheckDevice(this);
				GcmClient.CheckManifest(this);
				GcmClient.Register(this, PushHandlerBroadcastReceiver.SENDER_IDS);
			}
	        catch (Exception ex)
	        {
	            
	        }
			LoadApplication(new App());
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, global::Android.Content.Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			CardIO.ForwardActivityResult(requestCode, resultCode, data);
		}
	}
}

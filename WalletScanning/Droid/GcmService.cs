using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Android.App;
using Android.Content;
using Android.Util;
using Gcm.Client;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using WindowsAzure.Messaging;

[assembly: Permission(Name = "com.krodz.walletscanning.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.krodz.walletscanning.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]
//GET_ACCOUNTS is only needed for android versions 4.0.3 and below
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
namespace WalletScanning.Droid
{
	[Service]
	public class GcmService : GcmServiceBase
	{
		public static string RegistrationToken { get; private set; }
		private NotificationHub Hub { get; set; }

		public GcmService() : base(PushHandlerBroadcastReceiver.SENDER_IDS) { }

		protected override void OnMessage(Context context, Intent intent)
		{
			 Log.Info("PushHandlerBroadcastReceiver", "GCM Message Received!");

	        var msg = new StringBuilder();
	        if (intent != null && intent.Extras != null)
	        {
	            foreach (var key in intent.Extras.KeySet())
	                msg.AppendLine(key + "=" + intent.Extras.Get(key).ToString());
	        }

	        // Retrieve the message
	        var prefs = GetSharedPreferences(context.PackageName, FileCreationMode.Private);
			var edit = prefs.Edit();
			edit.PutString("last_msg", msg.ToString());
	        edit.Commit();

	        string message = intent.Extras.GetString("message");
	        if (!string.IsNullOrEmpty(message))
	        {
				CreateNotification("Clarity Mobile", message);
	        }
		}

		void CreateNotification(string title, string desc)
		{
			Notification.Builder builder = new Notification.Builder(this)
						.SetContentTitle(title)
						.SetContentText(desc)
						.SetSmallIcon(Resource.Drawable.ic_audiotrack);

			// Build the notification:
			Notification notification = builder.Build();

			// Get the notification manager:
			NotificationManager notificationManager =
				GetSystemService(Context.NotificationService) as NotificationManager;

			// Publish the notification:
			const int notificationId = 0;
			notificationManager.Notify (notificationId, notification);
			//dialogNotify(title, desc);
		}

		protected void dialogNotify(String title, String message)
		{

			MainActivity.CurrentActivity.RunOnUiThread(() =>
			{
				AlertDialog.Builder dlg = new AlertDialog.Builder(MainActivity.CurrentActivity);
				AlertDialog alert = dlg.Create();
				alert.SetTitle(title);
				alert.SetButton("Ok", delegate
				{
					alert.Dismiss();
				});
				alert.SetMessage(message);
				alert.Show();
			});
		}

		protected override void OnError(Context context, string errorId)
		{
			
		}

		protected override void OnRegistered(Context context, string registrationToken)
		{
	        RegistrationToken = registrationToken;
			Hub = new NotificationHub("MyNotificationHub", "Endpoint=sb://krodznotificationhub.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=Ux2gszIJMJCKA06rYPZ5JlRWr81whhWwO0A+I3BLNpo=",
								 context);
			try
		    {
				var hubRegistration = Hub.Register(registrationToken, new string[] { "TEST" });
		    }
		    catch (Exception ex)
		    {
		        Log.Error("TEST", ex.Message);
		    }
	        //var push = DefaultManager.CurrentClient.GetPush();
			// OLD CODE
			//var client = new MobileServiceClient("http://clarityadminapp.azurewebsites.net");
			////var client = new MobileServiceClient("https://krodznotificationhub.servicebus.windows.net:443/");

			//var push = client.GetPush();
			//MainActivity.CurrentActivity.RunOnUiThread(() => Register(push, new List<string>() { "TEST" }));
		}

		public async void Register(Push push, IEnumerable<string> tags)
		{
			try
			{
				const string templateBodyGCM = "{\"data\":{\"message\":\"$(messageParam)\"}}";

				var templates = new JObject();
				templates["genericMessage"] = new JObject
					{
						{"body", templateBodyGCM}
					};

				await push.RegisterAsync(RegistrationToken, templates);
				Log.Info("Push Installation Id", push.InstallationId);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
				Debugger.Break();
			}
		}

		protected override void OnUnRegistered(Context context, string registrationId)
		{
			
		}
	}
}

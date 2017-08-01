using System;
using TK.CardIO;
using Xamarin.Forms;

namespace WalletScanning
{
	public partial class App : Application
	{
		public App()
		{
			var button = new Button
			{
				Text = "Scan Credit Card"
			};
			button.Clicked += button_Clicked;

            MainPage = new ContentPage
            {
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
						button
					}
				}
            };
		}

		async void button_Clicked(object sender, EventArgs e)
		{
			var result = await CardIO.Instance.Scan(
				new CardIOConfig
				{
					Localization = "en",
					ShowPaypalLogo = false
				});
			var cardNumber = result.CardNumber;

		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}

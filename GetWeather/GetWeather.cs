using System;

using Xamarin.Forms;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace GetWeather
{

	public class App : Application
	{
		// You'll need an API key to request the weather

		// You can get a _*FREE*_ key here - https://www.wunderground.com/weather/api/
		private string WeatherAPIKey = "YOUR_API_KEY_HERE";

		private WeatherData WeatherResponse;
		private List<City> Cities = new List<City> ();

		private StackLayout CitySelectionStacklayout;
		// defined here so we can access its children later

		private ListViewCustom lstHourlyForecast;


		public App ()
		{
			
			// The root page of your application

			CreateCitiesList (WeatherAPIKey);


			// City Button Selection ############################################################
			CitySelectionStacklayout = new StackLayout {
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Orientation = StackOrientation.Vertical,
			};

			// Add a button to CitySelectionStacklayout for each City in our Cities list
			foreach (var city in Cities) {
				var tmpGetWeatherButton = new Button { 
					Text = city.Name,
					Font = Font.SystemFontOfSize (NamedSize.Large),
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.CenterAndExpand,
				};
				// assign an onClick
				tmpGetWeatherButton.Clicked += OnButtonClicked;
				// Add to stcklayout
				CitySelectionStacklayout.Children.Add (tmpGetWeatherButton);
			}



			// Weather Forecast Results Selection ################################################

			lstHourlyForecast = new ListViewCustom(ListViewCachingStrategy.RecycleElement);
			// Links - Why aching_Strategy - Recycle?
			// - https://developer.xamarin.com/guides/xamarin-forms/user-interface/listview/performance/#RecycleElement
			// - https://developer.xamarin.com/api/field/Xamarin.Forms.ListViewCachingStrategy.RecycleElement/

			// Links - Learn more about Caching_Strategy!
			// - https://developer.xamarin.com/guides/xamarin-forms/user-interface/listview/performance/#Caching_Strategy
			// - https://developer.xamarin.com/api/type/Xamarin.Forms.ListViewCachingStrategy/	

			lstHourlyForecast.HasUnevenRows = true;

			var ResultsStacklayout = new StackLayout {
				Orientation = StackOrientation.Vertical,
				Children = {
					lstHourlyForecast
				}
			};


			// Set page layout ###################################################################
			var Pagelayout = new StackLayout {
				Orientation = StackOrientation.Vertical,
				Children = {
					CitySelectionStacklayout, // City Buttons #########
					ResultsStacklayout	// Weather Forecast results #########
				}
			};

			var myContentPage = new ContentPage
			{
				Padding = new Thickness(4, 20, 4, 0) // Left, Top, Right, Bottom
			};

			myContentPage.Content = Pagelayout;
			MainPage = myContentPage;

		}


		/// <summary>
		/// Clicked a City Button - Look up which one was selected
		/// </summary>
		/// <returns>The button clicked.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private async void OnButtonClicked (object sender, EventArgs e)
		{

			//default weather URL to San Francisco
			string WeatherLocation = "http://api.wunderground.com/api/" + WeatherAPIKey + "/hourly/q/CA/San_Francisco.json";

			var buttonYouClicked = (Button)sender;

			// Loop though all of the cites in the Cities list/array and compaire to the clicked button's text
			foreach (var city in Cities) {
				if (buttonYouClicked.Text == city.Name) {
					WeatherLocation = city.URL;
					Debug.WriteLine ("You clicked on " + city.Name);
					continue;
				}
			}

			EnableEntry (false); // Disable the buttons, we're going to make a web request!

			Debug.WriteLine ("Feathing Data!");

			await RequestWeatherData (WeatherLocation);
		}


		/// <summary>
		/// Gets a json string from the given url
		/// </summary>
		/// <param name="uri">The url to fetch data from</param>
		/// <returns>The json string from the API</returns>
		/// <remarks>If the status code isn't 200 it will return a string
		/// that is the status code.</remarks>
		public static async Task<string> WebRequest (Uri uri)
		{
			var client = new HttpClient ();
			HttpResponseMessage response = await client.GetAsync (uri);


			if (response.StatusCode == HttpStatusCode.OK) {
				// 200 - OK
				string responseString = await response.Content.ReadAsStringAsync ();
				return responseString;

			} else if (response.StatusCode == HttpStatusCode.BadRequest) {
				// 400 - Bad request
				return "400";

			} else if (response.StatusCode == HttpStatusCode.Unauthorized) {
				// 401 - Unauthoriezed
				return "401";

			} else if (response.StatusCode == HttpStatusCode.NotFound) {
				// 404 - Data not found
				return "404";

			} else if (response.StatusCode == HttpStatusCode.InternalServerError) {
				// 500 - Internal server error
				return "500";

			} else if (response.StatusCode == HttpStatusCode.ServiceUnavailable) {
				// 503 - Service unavalible
				return "503";

			} else if ((int)response.StatusCode == 429) {
				// 429 - Rate limit exceeded
				return "429";

			} else {
				return "Unknown status code";
			}
		}

		internal static bool StatusCodeSuccess(string response)
		{
			return response != "400" &&
			response != "401" &&
			response != "404" &&
			response != "429" &&
			response != "500" &&
			response != "503" &&
			response != "Unknown Status Code";
		}


		/// <summary>
		/// Deserializes the weather response.
		/// </summary>
		/// <returns>The weather response.</returns>
		/// <param name="url">URL</param>
		public async Task RequestWeatherData (string url)
		{
			Uri uri = new Uri (url.ToString ());
			string responseString = await WebRequest (uri);

			if (StatusCodeSuccess(responseString))
			{
				//Debug.WriteLine(responseString);
				WeatherResponse = JsonConvert.DeserializeObject<WeatherData>(responseString);

				// Populate the Listview with the retunred data
				lstHourlyForecast.ItemsSource = WeatherResponse.hourly_forecast;
				lstHourlyForecast.ItemTemplate = new DataTemplate (typeof(WeatherHourlyListviewCell));

			} else {
				Debug.WriteLine("Error Requesting Weather - Response Code " + responseString);
				// add Xamarin.Insights or Hockeyapp here for error logging???
			}

			EnableEntry(); // reenable the buttons

		}


		/// <summary>
		/// Enables/Disables each button inside the City Selection stacklayout
		/// </summary>
		/// <param name="Enable">Enable or Disable the button?</param>
		private void EnableEntry(bool Enable = true)
		{
			foreach (Button btn in CitySelectionStacklayout.Children)
			{
				btn.IsEnabled = Enable;
			}
		}


		/// <summary>
		/// Populate our sample cities list.
		/// </summary>
		void CreateCitiesList (string weatherAPIKey)
		{
			Debug.WriteLine ("CreateCitiesList()");
		
			Cities.Add (new City ("New York", "http://api.wunderground.com/api/" + weatherAPIKey + "/hourly/q/NY/NewYork.json"));
			Cities.Add (new City ("London", "http://api.wunderground.com/api/" + weatherAPIKey + "/hourly/q/gb/london.json"));
			Cities.Add (new City ("San Francisco", "http://api.wunderground.com/api/" + weatherAPIKey + "/hourly/q/CA/San_Francisco.json"));

			// Link - Amundsen–Scott South Pole Station
			// - https://en.wikipedia.org/wiki/Amundsen%E2%80%93Scott_South_Pole_Station
			Cities.Add (new City ("Antarctica", "http://api.wunderground.com/api/" + weatherAPIKey + "/hourly/q/aq/amundsen-scott.json"));

		}


		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}



using System;
using Xamarin.Forms;

namespace GetWeather
{


	/// <summary>
	/// Custom Listview to color the rows
	/// Example: Set the odd cells to X and the even cells to Y
	/// </summary>
	public class ListViewCustom : ListView
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:GetWeather.ListViewCustom"/> class.
		/// </summary>
		/// <param name="cachingStrategy">Caching strategy.</param>
		public ListViewCustom(ListViewCachingStrategy cachingStrategy) : base(cachingStrategy)
		{
			// Learn more about Caching_Strategy!
			// - https://developer.xamarin.com/guides/xamarin-forms/user-interface/listview/performance/#Caching_Strategy
			// - https://developer.xamarin.com/api/type/Xamarin.Forms.ListViewCachingStrategy/	
		}


		// Override SetupContent so we can automatically set the color of each cell
		protected override void SetupContent(Cell content, int index)
		{
			base.SetupContent(content, index);
			var currentViewCell = content as ViewCell;

			if (currentViewCell != null)
			{

				if (index % 2 == 0)
					currentViewCell.View.BackgroundColor = Color.FromHex("E4FFFF");
				else 
					currentViewCell.View.BackgroundColor = Color.FromHex("F8FAFA");
				
			}
		}

	}


	public class WeatherHourlyListviewCell : ViewCell
	{
		
		public WeatherHourlyListviewCell ()
		{
			
			// Time  ################################################################
			var lblTime = new Label {
				VerticalTextAlignment = TextAlignment.Start,
				FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label)),
				FontAttributes = FontAttributes.Bold,
			};
			lblTime.SetBinding (Label.TextProperty, "FCTTIME.pretty");



			// Weather Icon  ################################################################
			var WeatherIcon = new Image {
				HeightRequest = 35,
				WidthRequest = 35
			};
			WeatherIcon.SetBinding (Image.SourceProperty, "icon_url");


			
			// Current Temp  ################################################################
			var lblCurrentTemp = new Label
			{
				VerticalTextAlignment = TextAlignment.Start,
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				FontAttributes = FontAttributes.Bold,
			};
			lblCurrentTemp.SetBinding(Label.TextProperty, "temp.english");
			lblCurrentTemp.SetBinding(Label.TextColorProperty, "temp.english", BindingMode.OneWay, new Converter_WeatherTemp());
			


			// Condition  ################################################################
			var lblCondition = new Label {
				VerticalTextAlignment = TextAlignment.Start,
				FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label)),
				FontAttributes = FontAttributes.Bold,
			};
			lblCondition.SetBinding (Label.TextProperty, "condition");



			// Feels Like  ################################################################
			var lblFeelsLike = new Label
			{
				VerticalTextAlignment = TextAlignment.Start,
			};
			lblFeelsLike.SetBinding(Label.TextProperty, "feelslike.english", BindingMode.OneWay, null, "(Feels Like {0})");



			// Wind Direction  ################################################################
			var lblWindDirection = new Label {
				VerticalTextAlignment = TextAlignment.Start,
			};
			lblWindDirection.SetBinding (Label.TextProperty, "wdir.dir", BindingMode.OneWay, null, "Wind {0}");



			// UV Index  ################################################################
			var lblUVIndex = new Label {
				VerticalTextAlignment = TextAlignment.Start,
			};
			lblUVIndex.SetBinding (Label.TextProperty, "uvi", BindingMode.OneWay, null, "UV Index {0}");



			var cellMainLayout = new StackLayout
			{
				Padding = new Thickness(1, 0),
				Orientation = StackOrientation.Vertical,
				Children = {
					lblTime, // Time #########
					new StackLayout{
						Orientation = StackOrientation.Horizontal,
						Children = {
							WeatherIcon, // Icon #########
							new StackLayout{
								Orientation = StackOrientation.Vertical,
								Children = {
									new StackLayout{
										Orientation = StackOrientation.Horizontal,
										Children = {
											lblCurrentTemp, // Temp #########
											lblCondition // Condition #########
										}
									},
									new StackLayout{
										Orientation = StackOrientation.Horizontal,
										Children = {
											lblFeelsLike, // Feels Like #########
											lblWindDirection, // Wind #########
											lblUVIndex // UV Index #########
										}
									},
								}
							},
						}
					}
				}
			};

			View = cellMainLayout;

		}
	}

}




using System;
using System.Globalization;
using Xamarin.Forms;

namespace GetWeather
{
	
	public class Converter_WeatherTemp : IValueConverter
	{
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int intTemp = 0;

			if (value is int)
				intTemp = (int)value;
			else if (value is string)
				intTemp = Int32.Parse((string)value);
			
			// -------------------------------------------

			if (intTemp > 90)
				// hot
				return Color.Red;

			else if (intTemp > 50)
				// mild
				return Color.FromHex("FFA955");
			else
				// cold!
				return Color.FromHex("007BB4");
			}


		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

	}

}



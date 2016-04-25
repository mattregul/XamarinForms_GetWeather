using System;

namespace GetWeather
{
	public class City
	{
		public string Name { get; set; }

		public string URL { get; set; }

		public City (string name, string url)
		{
			Name = name;
			URL = url;
		}
	}

}


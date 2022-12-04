using CommonUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

/*
 * NOTE These classes can be created automatically for free by some tools (eg. https://json2csharp.com/).
 * Just copy-paste a sample of the required JSON and it will create the POCO classes. Make sure to replace
 * properties with fields (replace instances of "{ get; set; }" with ";".
 *
 * Properties can then be defined to show values in the inspector.
 *
 * These classes make up the response of a call to https://api.weather.gov/gridpoints/MTR/85,105/forecast
 */

namespace Demos.RestSdkAndWebResources {
	public class TestDto {
		[JsonProperty("@context")]
		public List<object> context;

		public string type;
		public Geometry geometry;
		public Properties properties;

		[ShowInInspector] public string Type => type;
	}

	public class Elevation {
		public string unitCode;
		public double value;
	}

	public class Geometry {
		public string type;
		public List<List<List<double>>> coordinates;
	}

	public class Period {
		public int number;
		public string name;
		public DateTime startTime;
		public DateTime endTime;
		public bool isDaytime;
		public int temperature;
		public string temperatureUnit;
		public string temperatureTrend;
		public string windSpeed;
		public string windDirection;
		public string icon;
		public string shortForecast;
		public string detailedForecast;
	}

	public class Properties {
		public DateTime updated;
		public string units;
		public string forecastGenerator;
		public DateTime generatedAt;
		public DateTime updateTime;
		public string validTimes;
		public Elevation elevation;
		public List<Period> periods;
	}
}
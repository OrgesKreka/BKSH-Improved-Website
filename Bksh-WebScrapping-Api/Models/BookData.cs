using System.Collections.Generic;

namespace Bksh_WebScrapping_Api.Models
{
	/// <summary>
	///		Modeli me gjithe te dhenat qe mund te kete nje liber
	///		Nje liber mund ti kete gjithe keto te dhenat ose disa prej tyre
	/// </summary>
	public class BookData
	{
		public string Title { get; set; }

		public IEnumerable<string> Authors { get; set; }

		public string PhysicalData { get; set; }

		public int NumberOfPages { get; set; }

		public float EstimatedReadingTime { get; set; }

		public string Notes { get; set; }

		public IEnumerable<string> Keywords { get; set; }

		public string Clasification { get; set; }

		public string PublishingData { get; set; }

		public string Isbn { get; set; }

		public string SerieTitle { get; set; }

		public IEnumerable<ExemplarData> Exemplars { get; set; }
	}
}
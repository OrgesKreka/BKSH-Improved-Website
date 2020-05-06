using System.ComponentModel;

namespace Bksh_WebScrapping_Api.Models
{
	/// <summary>
	///     Formati i formes qe behet submit
	///     kur kerkon nga faqja http://www.bksh.al/Katalogu/library/wwwopac/wwwroot/beginner/index_al.html
	/// </summary>
	public class RequestFormModel
	{
		#region Fusha Konstante
		// Si duket keto fusha kalojne si "legjende".


		[Description("OPAC_URL")]
		public string OpacUrl => "/adlib/beginner/index_al.html";

		[Description("LANGUAGE")]
		public int Language => 1; // Shqip

		[Description("FLD1")]
		public string Field1Constant => "ti";

		[Description("FLD2")]
		public string Field2Constant => "l1";

		[Description("FLD3")]
		public string Field3Constant => "l2";

		[Description("FLD4")]
		public string Field4Constant => "%F";

		[Description("FLD5")]
		public string Field5Constant => "lr";

		[Description("FLD51")]
		public string Field51Constant => "lg";

		[Description("FLD52")]
		public string Field52Constant => "lp";

		[Description("FLD6")]
		public string Field6Constant => "ju";

		[Description("FLD7")]
		public string Field7Constant => "ey";
		#endregion


		/// <summary>
		///     Kerkimi per nje fushe te caktuar behet sipas formatit:
		///     Fusha Konstante, FLDx
		///     Fusha e vleres, VALx
		///     Fusha e CheckBox, TRCx
		///     
		/// </summary>


		#region Fushat per kerkimin me titull

		[Description("FLD1")]
		public string TitleSearchConstant => Field1Constant;

		[Description("VAL1")]
		public string TitleSearchValue { get; set; } = string.Empty;

		[Description("TRC1")]
		public string IsSearchingByTitle { get; set; }
		#endregion

		#region Fushat per kerkimin me autor
		[Description("FLD2")]
		public string AuthorSearchConstant => Field2Constant;

		[Description("VAL2")]
		public string AuthorSearchValue { get; set; } = string.Empty;

		[Description("TRC2")]
		public string IsSearchingByAuthor { get; set; }

		#endregion

		#region Fushat per kerkimin me autor kolektiv ( per momentin nuk perdoret )

		[Description("FLD3")]
		public string CollectiveAuthorConstant => Field3Constant;

		[Description("VAL3")]
		public string CollectiveAuthorValue { get; set; } = string.Empty;

		[Description("TRC3")]
		public string IsSearchingByCollectiveAuthor { get; set; }
		#endregion

		#region Fushat per kerkimin me tekst
		[Description("FLD4")]
		public string TextSearchConstant => Field4Constant;

		[Description("VAL4")]
		public string TextSearchValue { get; set; } = string.Empty;

		[Description("TRC4")]
		public string IsSearchingByText { get; set; }
		#endregion

		#region Fushat per kerkimin me fjale kyce 
		[Description("FLD5")]
		public string KeyWordSearchConstant => Field5Constant;

		[Description("VAL5")]
		public string KeyWordSearchValue { get; set; } = string.Empty;

		[Description("TRC5")]
		public string IsSearchingByKeyWord { get; set; }
		#endregion

		#region Fushat per kerkimin me term gjeografik (per momentin nuk perdoret )

		[Description("FLD51")]
		public string GeographicTermSearchConstant => Field51Constant;

		[Description("VAL51")]
		public string GeographicTermSearchValue { get; set; } = string.Empty;

		[Description("TRC51")]
		public string IsSearchingByGeographicalTerm { get; set; }

		#endregion

		#region Fushat per kerkimin me emer personi ( per momentin nuk perdoret )

		[Description("FLD52")]
		public string PersonNameSearchConstant => Field52Constant;

		[Description("VAL52")]
		public string PersonNameSearchValue { get; set; } = string.Empty;

		[Description("TRC52")]
		public string IsSearchingByPersonName { get; set; }

		#endregion

		#region Fushat per kerkimin me vit

		[Description("FLD6")]
		public string YearSearchConstant => Field6Constant;

		[Description("VAL6")]
		public string YearSearchValue { get; set; } = string.Empty;

		[Description("TRC6")]
		public string IsSearchingByYear { get; set; }

		#endregion

		#region Fushat per kerkimin me number vendi ( per momentin nuk perdoret ) 

		[Description("FLD7")]
		public string PlaceNumberSearchConstant => Field7Constant;

		[Description("VAL7")]
		public string PlaceNumberSearchValue { get; set; } = string.Empty;

		[Description("TRC7")]
		public string IsSearchingByPlaceNumber { get; set; }

		#endregion

		/// <summary>
		///     Sa libra do te shfaqen
		/// </summary>
		[Description("LIMIT")]
		public int Limit { get; set; } = 25;

		[Description("DATABASE")]
		public string Database { get; set; }
	}
}
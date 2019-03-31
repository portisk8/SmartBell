using System;

namespace SmartBell.Core.Configuration
{
	public class GeneralConfig : IGeneralConfig
	{
		public string ConnectionString { get; set; }
		public string Dataset { get; set; }
		public string Contenedor { get; set; }
		public string FaceRecognitionKey { get; set; }
		public string TelegramBotKey { get; set; }
	}
}

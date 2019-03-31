using System;

namespace SmartBell.Core.Configuration
{
	public interface IGeneralConfig
	{
		string ConnectionString { get; set; }
		string Dataset { get; set; }
		string Contenedor { get; set; }
		string FaceRecognitionKey { get; set; }
		string TelegramBotKey { get; set; }
	}
}

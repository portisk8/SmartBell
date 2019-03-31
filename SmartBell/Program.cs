using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;
using SmartBell.Core.Configuration;
using SmartBell.Interfaces;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace SmartBell
{
	class Program
	{
		public static IGeneralConfig GeneralConfig { get; set; }
		public static Autofac.IContainer ConfigurationAutofac { get; set; }
		private static Serilog.ILogger _logger;

		static void Main(string[] args)
		{
			var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

			if (string.IsNullOrEmpty(environment))
				environment = "Production";

			var builderOnlyForConfig = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
			   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			   .AddJsonFile($"appsettings.{environment}.json", optional: true)
			   .AddEnvironmentVariables();

			var Configuration = builderOnlyForConfig.Build();

			//Armar configuracion general
			GeneralConfig = new GeneralConfig
			{
				ConnectionString = Configuration["ConnectionStrings"],
				Contenedor = Configuration["Storage:Contenedor"],
				Dataset = Configuration["Storage:Dataset"],
				FaceRecognitionKey = Configuration["Keys:FaceRecognitionKey"],
				TelegramBotKey = Configuration["Keys:TelegramBotKey"]
			};

			var builder = new Autofac.ContainerBuilder();


			//Autofac Config (http://autofaccn.readthedocs.io/en/latest/integration/netcore.html)
			var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

			serviceCollection.AddSingleton<Serilog.ILogger>(new LoggerConfiguration()
										.Enrich.WithExceptionDetails()
										.ReadFrom.Configuration(Configuration)
										.WriteTo.Console()
										.CreateLogger());

			builder.Populate(serviceCollection);
			//Agregar inyecciones de dependencia si fuese necesario
			builder.RegisterAssemblyTypes(typeof(ITelegramBot).GetTypeInfo().Assembly)
			 .WithParameter("generalConfig", GeneralConfig)
			 .AsImplementedInterfaces()
			 .InstancePerDependency();

			ConfigurationAutofac = builder.Build();

			var serviceProvider = new AutofacServiceProvider(ConfigurationAutofac);

			Console.WriteLine();
			Console.WriteLine($"{DateTime.Now} > Environment > {environment}.");
			Console.WriteLine();

			MainAsync(ConfigurationAutofac).Wait();
		}

		private static async Task MainAsync(IContainer configurationAutofac)
		{
			var telegramBot = ConfigurationAutofac.Resolve<ITelegramBot>();
			telegramBot.Init();
		}
	}
}

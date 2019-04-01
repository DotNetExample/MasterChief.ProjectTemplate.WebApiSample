using System;
using System.Globalization;
using MasterChief.DotNet.Core.Config;
using MasterChief.DotNet.ProjectTemplate.WebApi.Helper;
using MasterChief.DotNet.ProjectTemplate.WebApi.Model;
using MasterChief.DotNet4.Utilities.Common;
using MasterChief.DotNet4.Utilities.Web;

namespace MasterChief.ProjectTemplate.WebApiConsoleApp
{
    internal class Program
    {
        private static readonly Guid _appSecret = "097EF3B7-7FE3-4e84-B8FE-24A058EAF66F".ToGuidOrDefault();
        private static readonly Guid _appId = "622CF655-1559-4328-B493-EAC7AA833F2E".ToGuidOrDefault();

        private static void Main(string[] args)
        {
            try
            {
                CreateAppConfig();
                var apiServiceUrl = "http://localhost:24003/";
                var timestamp = UnixEpochHelper.GetCurrentUnixTimestamp().TotalMilliseconds
                    .ToString(CultureInfo.InvariantCulture);
                var nonce = new Random().NextDouble().ToString(CultureInfo.InvariantCulture);
                var signature = SignatureHelper.Create(_appSecret, timestamp, nonce);
                var appended =
                    $"&signature={signature}&timestamp={timestamp}&nonce={nonce}&appid={_appId}";

                var queryUrl = apiServiceUrl +
                               "api/Account/GetToken?userId=2c96ff542072420bc8d33bdd73bb9488&passWord=0000" + appended;
                var responeText = SimulateWebRequest.Post(queryUrl, null);
                Console.WriteLine(responeText);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadLine();
            }
        }
        private static void CreateAppConfig()
        {
            ConfigContext config = new CacheConfigContext();
            var appConfig = config.Get<AppConfig>("622CF655-1559-4328-B493-EAC7AA833F2E");
            if (appConfig == null)
            {
                appConfig = new AppConfig
                {
                    AppId = _appId,
                    AppSecret = _appSecret.ToString(),
                    SharedKey = "275CD14E-6B51-444b-BC1B-88776160F81E",
                    SignatureExpiredMinutes = 10,
                    TokenExpiredDay = 7
                };

                config.Save(appConfig, "622CF655-1559-4328-B493-EAC7AA833F2E");
            }
        }
    }
}
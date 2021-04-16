using Microsoft.Extensions.Options;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.DevTools.V88.Network;
using SeleniumRequestInterception.Dtos;
using SeleniumRequestInterception.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumRequestInterception.Services
{
    public class ChromeService : IChrome
    {
        private readonly ChromeDriverConfiguration _chromeDriverConfiguration;

        public ChromeService(IOptionsSnapshot<ChromeDriverConfiguration> chromeDriverOptions)
        {
            _chromeDriverConfiguration = chromeDriverOptions.Value;
        }

        public async Task GoToPage(string url)
        {
            try
            {
                ChromeOptions options = new ChromeOptions();
                options.BinaryLocation = _chromeDriverConfiguration.BrowserPath;
                //Descomenta las siguientes linea para usar el mode HeadLess de Chrome
                //options.AddArgument("--headless");
                //var user_agent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.50 Safari/537.36";
                //options.AddArgument($"user_agent={user_agent}");
                options.AddArgument("--ignore-certificate-errors");
                options.AddArgument("--disable-blink-features=AutomationControlled");


                // Create an instance of the browser and configure launch options
                using (ChromeDriver driver = new ChromeDriver(_chromeDriverConfiguration.Path, options))
                {
                    DevToolsSession devToolsSession = driver.GetDevToolsSession();
                    var fetch = devToolsSession.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V88.DevToolsSessionDomains>().Fetch;
                    var enableCommandSettings = new OpenQA.Selenium.DevTools.V88.Fetch.EnableCommandSettings();

                    enableCommandSettings.Patterns = new OpenQA.Selenium.DevTools.V88.Fetch.RequestPattern[]
                    {
                        new OpenQA.Selenium.DevTools.V88.Fetch.RequestPattern()
                        {
                            RequestStage = OpenQA.Selenium.DevTools.V88.Fetch.RequestStage.Request,
                             ResourceType = ResourceType.Image,
                        },
                        new OpenQA.Selenium.DevTools.V88.Fetch.RequestPattern()
                        {
                            RequestStage = OpenQA.Selenium.DevTools.V88.Fetch.RequestStage.Request,
                             ResourceType = ResourceType.Stylesheet,
                        }
                    };
                    await fetch.Enable(enableCommandSettings);

                    EventHandler<OpenQA.Selenium.DevTools.V88.Fetch.RequestPausedEventArgs> requestIntercepted = (sender, e) =>
                    {
                        fetch.FailRequest(new OpenQA.Selenium.DevTools.V88.Fetch.FailRequestCommandSettings
                        {
                            RequestId = e.RequestId,
                            ErrorReason = ErrorReason.BlockedByClient
                        });
                    };

                    fetch.RequestPaused += requestIntercepted;

                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
                    driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(40);


                    driver.Navigate().GoToUrl(String.IsNullOrEmpty(url) ? "https://www.youtube.com/" : url);

                    // Search for input user

                }
            }
            catch (Exception ex)
            {
            }
        }





    }
}

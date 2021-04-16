using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumRequestInterception.Dtos
{
    public class ChromeDriverConfiguration
    {
        // Path to ChromeDriver .exe
        public string Path { get; set; }

        // Path to Chrome / Chromium .exe
        public string BrowserPath { get; set; }

    }
}

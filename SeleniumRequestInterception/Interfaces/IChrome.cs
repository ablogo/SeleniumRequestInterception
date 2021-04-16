using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumRequestInterception.Interfaces
{
    public interface IChrome
    {
        Task GoToPage(string url);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace BLINK.Interface
{
    public interface IMapPage
    {
        Task<Location> GetLocation();
    }
}

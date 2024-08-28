using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow.Launcher.Plugin.WTFIsMyIP
{
    public enum IPVWhat
    {
        Dual,
        IPv4,
        IPv6
    }
    public class Settings
    {
        public IPVWhat ipv { get; set; }
    }
}

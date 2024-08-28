using Flow.Launcher.Plugin;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;
using System.Net.Http.Json;
using System.Threading;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.WTFIsMyIP
{
    internal class WTFIPClient
    {
        public class WTFData
        {
            public string YourFuckingIPAddress { get; set; }
            public string YourFuckingLocation { get; set; }
            public string YourFuckingHostname { get; set; }
            public string YourFuckingISP { get; set; }
            public bool YourFuckingTorExit { get; set; }
            public string YourFuckingCity { get; set; }
            public string YourFuckingCountry { get; set; }
            public string YourFuckingCountryCode { get; set; }
        }
        readonly HttpClient client;

        private static string GetURL(IPVWhat ipv = IPVWhat.Dual)
        {
            switch (ipv)
            {
                case IPVWhat.IPv4:
                    return "https://json.ipv4.wtfismyip.com";
                case IPVWhat.IPv6:
                    return "https://json.ipv6.wtfismyip.com";
                default:  // Surely Both
                    return "https://json.wtfismyip.com";
            }
        }

        private HttpRequestMessage GetRequest(IPVWhat ipv = IPVWhat.Dual)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, WTFIPClient.GetURL(ipv));
            request.Headers.Add("User-Agent", "Flow.Launcher.Plugin.WTFIsMyIP/0.0.1");
            return request;
        }

        public async Task<WTFIsMyIP.WTFIPClient.WTFData> GetData(IPVWhat ipv, CancellationToken cancToken)
        {
            HttpResponseMessage rawResp = await this.client.SendAsync(this.GetRequest(ipv), cancToken);
            return JsonConvert.DeserializeObject<WTFData>(await rawResp.Content.ReadAsStringAsync(cancToken));
        }

        public WTFIPClient()
        {
            this.client = new HttpClient();
        }
    }

    public class Main : IAsyncPlugin, ISettingProvider
    {
        private readonly WTFIPClient client = new WTFIPClient();
        private PluginInitContext _context;
        private Settings _settings;

        public Task InitAsync(PluginInitContext context)
        {
            return Task.Run(Init);
            void Init()
            {
                this._context = context;
                this._settings = this._context.API.LoadSettingJsonStorage<Settings>();
            }
        }

        private List<Result> ConstructResults(WTFIPClient.WTFData data) {
            List<Result> res = new List<Result> { };
            if (!String.IsNullOrEmpty(data.YourFuckingIPAddress))
            res.Add(new Result
            {
                Title = data.YourFuckingIPAddress,
                SubTitle = "Your Fucking IP",
                CopyText = data.YourFuckingIPAddress,
                Glyph = new GlyphInfo(FontFamily: "/Resources/#Segoe Fluent Icons", Glyph: "\ue928"),
                Action = _ => { this._context.API.CopyToClipboard(data.YourFuckingIPAddress); return true; }
            });
            if (data.YourFuckingTorExit)
                res.Add(new Result
                {
                    Title = "You are using Tor, nerd!",
                    Glyph = new GlyphInfo(FontFamily: "/Resources/#Segoe Fluent Icons", Glyph: "\uf19d")
                });
            if (!String.IsNullOrEmpty(data.YourFuckingLocation))
                res.Add(new Result {
                    Title = data.YourFuckingLocation,
                    SubTitle = "Your Fucking Geo",
                    CopyText = data.YourFuckingLocation,
                    Glyph = new GlyphInfo(FontFamily: "/Resources/#Segoe Fluent Icons", Glyph: "\ue81b"),
                    Action = _ => { this._context.API.CopyToClipboard(data.YourFuckingLocation); return true; }
                });
            if (!String.IsNullOrEmpty(data.YourFuckingISP))
                res.Add(new Result {
                    Title = data.YourFuckingISP,
                    SubTitle = "Your Fucking ISP",
                    CopyText = data.YourFuckingISP,
                    Glyph = new GlyphInfo(FontFamily: "/Resources/#Segoe Fluent Icons", Glyph: "\ue950"),
                    Action = _ => { this._context.API.CopyToClipboard(data.YourFuckingISP); return true; }
                });
            if (!String.IsNullOrEmpty(data.YourFuckingHostname))
                res.Add(new Result {
                    Title = data.YourFuckingHostname,
                    SubTitle = "Your Fucking Hostname",
                    CopyText = data.YourFuckingHostname,
                    Glyph = new GlyphInfo(FontFamily: "/Resources/#Segoe Fluent Icons", Glyph: "\ue753"),
                    Action = _ => { this._context.API.CopyToClipboard(data.YourFuckingHostname); return true; }
                });
            string jsonData = JsonConvert.SerializeObject(data);
            res.Add(new Result
            {
                Title = "Copy JSON",
                SubTitle = "Why you fucking need it, nerd?",
                Glyph = new GlyphInfo(FontFamily: "/Resources/#Segoe Fluent Icons", Glyph: "\ue8c8"),
                CopyText = jsonData,
                Action = _ => { this._context.API.CopyToClipboard(jsonData); return true; }
            });
            return res;
        }

        public async Task<List<Result>> QueryAsync(Query query, CancellationToken cancToken)
        {
            await Task.Delay(150, cancToken);
            cancToken.ThrowIfCancellationRequested();
            IPVWhat ipvForQuery;
            switch (query.Search) {
                case "4":
                    ipvForQuery = IPVWhat.IPv4;
                    break;
                case "6":
                    ipvForQuery = IPVWhat.IPv6;
                    break;
                case "d":
                    ipvForQuery = IPVWhat.Dual;
                    break;
                default:
                    ipvForQuery = this._settings.ipv;
                    break;
            }
            WTFIPClient.WTFData data = await this.client.GetData(ipvForQuery, cancToken);
            return ConstructResults(data);
        }

        public Control CreateSettingPanel() {
            return new WTFIsMyIPSettings(this._context, this._settings);
        }
    }
}
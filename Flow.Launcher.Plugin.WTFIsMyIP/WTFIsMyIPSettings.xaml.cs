using System.Windows;
using System.Windows.Controls;
using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.WTFIsMyIP
{
    public partial class WTFIsMyIPSettings : UserControl
    {
        private readonly Settings _settings;
        private readonly PluginInitContext _context;
        public WTFIsMyIPSettings(PluginInitContext context, Settings settings)
        {
            this._settings = settings;
            this._context = context;
            InitializeComponent();
        }

        private void WTFIsMyIPSettings_Loaded(object sender, RoutedEventArgs e) {
            switch (_settings.ipv) {
                case IPVWhat.IPv4:
                    StackComboBox.SelectedIndex = 1;
                    break;
                case IPVWhat.IPv6:
                    StackComboBox.SelectedIndex = 2;
                    break;
                case IPVWhat.Dual:
                default:
                    StackComboBox.SelectedIndex = 0;
                    break;
                
            }
        }

        private void StackComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (StackComboBox.SelectedIndex) {
                case 0:
                    _settings.ipv = IPVWhat.Dual;
                    break;
                case 1:
                    _settings.ipv = IPVWhat.IPv4;
                    break;
                case 2:
                default:
                    _settings.ipv = IPVWhat.IPv6;
                    break;
            }
            this._context.API.SavePluginSettings();
        }
    }
}
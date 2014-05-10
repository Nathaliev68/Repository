using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace MarktplaatsZoeker
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class ItemsPage : MarktplaatsZoeker.Common.LayoutAwarePage
    {
        public ItemsPage()
        {
            this.InitializeComponent();
        }

        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            MarktplaatsItems feedDataSource = (MarktplaatsItems)App.Current.Resources["feedDataSource"];
            var connectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();

            if (connectionProfile != null)
            {

                MarktPlaatsConnection mp = new MarktPlaatsConnection();
                if (feedDataSource != null)
                {
                    feedDataSource = await mp.GetMarktplaatsItems(navigationParameter.ToString());
                }
            }
            else
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog("An internet connection is needed to download feeds. Please check your connection and restart the app.");
                var result = messageDialog.ShowAsync();
            }

            if (feedDataSource != null)
            {
                this.DefaultViewModel["Items"] = feedDataSource.Items;
                App.Current.Resources["feedDataSource"] = feedDataSource;
            }
        }

        private void itemListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Windows.System.Launcher.LaunchUriAsync(((MarktplaatsItem)e.ClickedItem).Link);
        }

        private void itemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Windows.System.Launcher.LaunchUriAsync(((MarktplaatsItem)e.ClickedItem).Link);
        }

       

    }
}

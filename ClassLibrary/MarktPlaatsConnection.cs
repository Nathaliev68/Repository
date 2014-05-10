using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.Web.Syndication;

namespace MarktplaatsZoeker
{
    public class MarktPlaatsConnection
    {

        public async Task<SyndicationFeed> GetFeedAsync(string feedUriString)
        {
            Windows.Web.Syndication.SyndicationClient client = new SyndicationClient();
            Uri feedUri = new Uri(feedUriString);

            try
            {
                SyndicationFeed feed = await client.RetrieveFeedAsync(feedUri);
                return feed;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<MarktplaatsItems> GetMarktplaatsItems(string uri)
        {

            Task<SyndicationFeed> feed = GetFeedAsync(uri);
            SyndicationFeed feed2 = await feed;
            MarktplaatsItems items = new MarktplaatsItems();
            var appsettings = ApplicationData.Current.LocalSettings;

            foreach (SyndicationItem item in feed2.Items.OrderByDescending(x => x.PublishedDate))
            {
                MarktplaatsItem feedItem = new MarktplaatsItem();
                if (item.Title != null && item.Title.Text != null)
                {
                    feedItem.Title = item.Title.Text;
                }
                if (item.PublishedDate != null)
                {
                    feedItem.PubDate = item.PublishedDate.LocalDateTime;

                    if (appsettings.Values["lastlogon"] != null &&
                        feedItem.PubDate.ToBinary() > long.Parse(appsettings.Values["lastlogon"].ToString()))
                    {
                        feedItem.Read = false;
                        ShowToast(feedItem);
                    }
                    else
                    {
                        if (appsettings.Values["lastlogon"] == null)
                        {
                            feedItem.Read = false;
                            ShowToast(feedItem);
                        }
                        else
                        {
                            feedItem.Read = true;
                        }
                    }
                }
                if (item.Authors != null && item.Authors.Count > 0)
                {
                    feedItem.Author = item.Authors[0].Name.ToString();
                }
                // Handle the differences between RSS and Atom feeds.
                if (feed2.SourceFormat == SyndicationFormat.Atom10)
                {
                    if (item.Content != null && item.Content.Text != null)
                    {
                        feedItem.Content = item.Content.Text;
                    }
                    if (item.Id != null)
                    {
                        //feedItem.Link = new Uri("http://windowsteamblog.com" + item.Id);
                        feedItem.Link = new Uri(item.Id);
                    }
                }
                else if (feed2.SourceFormat == SyndicationFormat.Rss20)
                {
                    if (item.Summary != null && item.Summary.Text != null)
                    {
                        feedItem.Content = item.Summary.Text;
                    }
                    if (item.Links != null && item.Links.Count > 0)
                    {
                        feedItem.Link = item.Links[0].Uri;
                    }
                }

                ////deze specifieke moet eigenlijk met een link querie op objecttyoe uitgevraagd worden, 
                ///maar nu even niet zo belangrijk omdat we alleen marktplaats doen en dan staan ze altijd op dezeldfe plek.
                if (item.ElementExtensions != null && item.ElementExtensions.Count > 2 && item.ElementExtensions[2].AttributeExtensions.Count > 0)
                {
                    feedItem.Image = new Uri(item.ElementExtensions[2].AttributeExtensions[0].Value);
                }

                if (item.Summary != null)
                {
                    string[] p = item.Summary.Text.Split('-');
                    if (p.Count() > 1)
                    {
                        string[] p2 = p[1].Split(':');
                        if (p2.Count() > 1)
                        {
                            feedItem.Prijs = p2[1];
                        }
                    }
                }

                items.Items.Add(feedItem);
                if (items.Items.Count() == 92)
                { //stopr
                    Debug.WriteLine("kapot");
                }
            }
            return items;
        }

        private void ShowToast(MarktplaatsItem item)
        {
            ToastTemplateType template = ToastTemplateType.ToastText01;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(template);

            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(
                "Er is een nieuw artikel op Marktplaats aangeboden dat voldoet aan uw zoekopdracht:" + item.Title));

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);

        }


    }
}

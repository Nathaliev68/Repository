using MarktplaatsZoeker.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MarktplaatsZoeker
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class StartPage : MarktplaatsZoeker.Common.LayoutAwarePage
    {
        public StartPage()
        {
            this.InitializeComponent();
            this.DataContext = new StartPageViewModel(); ;
            FillGrid();
        }

        public async void FillGrid()
        {
            var vm = (StartPageViewModel)this.DataContext;
            var list = await vm.GetZoekOpdrachten();
            AdvListView.ItemsSource = list;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            var taskEntryPoint = "BackGroundUpdater.BackGroundRunner";
            var taskName = "BackGroundRunner";
            MaintenanceTrigger taskTrigger = new MaintenanceTrigger(15, false);

            var task = RegisterBackgroundTask(taskEntryPoint, taskName, taskTrigger, null);
            task.Progress += new BackgroundTaskProgressEventHandler(OnProgress);
            task.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
        }

        // Register a background task with the specified taskEntryPoint, name, trigger,
        // and condition (optional).
        //
        // taskEntryPoint: Task entry point for the background task.
        // taskName: A name for the background task.
        // trigger: The trigger for the background task.
        // condition: Optional parameter. A conditional event that must be true for the task to fire.
        //
        public BackgroundTaskRegistration RegisterBackgroundTask(string taskEntryPoint,
                                                                        string taskName,
                                                                        IBackgroundTrigger trigger,
                                                                        IBackgroundCondition condition)
        {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == taskName)
                {
                    // 
                    // The task is already registered.
                    // 
                    return (BackgroundTaskRegistration)(cur.Value);
                }
            }

            var builder = new BackgroundTaskBuilder();

            builder.Name = taskName;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(trigger);

            if (condition != null)
            {
                builder.AddCondition(condition);
            }
            BackgroundTaskRegistration task = builder.Register();
            return task;
        }

        private void OnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs arg)
        {
            var settings = ApplicationData.Current.LocalSettings;
            var key = task.TaskId.ToString();
            var message = "Klaar met synchroniseren-" + DateTime.Now;
            UpdateUIExampleMethod(message);

            string tileXmlString = "<tile>" + "<visual>" + "<binding template='TileWideText03'>" + "<text id='1'>"
                + "</text>" + "</binding>" + "<binding template='TileSquareText04'>" + "<text id='1'>"
             + "</text>" + "</binding>" + "</visual>" + "</tile>";
            var tileXml = new Windows.Data.Xml.Dom.XmlDocument(); tileXml.LoadXml(tileXmlString);
            var tile = new Windows.UI.Notifications.TileNotification(tileXml);
            Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication().Update(tile);

            try
            {
                arg.CheckResult();

            }
            catch (Exception)
            {
                message = "error";
                UpdateUIExampleMethod(message);
            }
        }

        private async void UpdateUIExampleMethod(string message)
        {

            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.DefaultViewModel["completed"] = message;
            });
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void OnProgress(BackgroundTaskRegistration sender, BackgroundTaskProgressEventArgs args)
        {
            //string tileXmlString = "<tile>" + "<visual>" + "<binding template='TileWideText03'>" + "<text id='1'>" + args.Progress.ToString()
            //    + "</text>" + "</binding>" + "<binding template='TileSquareText04'>" + "<text id='1'>"
            //    + args.Progress.ToString() + "</text>" + "</binding>" + "</visual>" + "</tile>";
            //var tileXml = new Windows.Data.Xml.Dom.XmlDocument(); tileXml.LoadXml(tileXmlString);
            //var tile = new Windows.UI.Notifications.TileNotification(tileXml);
            //Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication().Update(tile);
            UpdateUIExampleMethod("lekker bezig");
        }
     
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(AddZoekOpdracht));
        }
        

    }
}

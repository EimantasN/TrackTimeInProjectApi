using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace TrackTimeSpendInProjectAndroidAapp
{
    [Service]
    public class CheckService : Service
    {
        private bool Destroyed = false;
        public static WifiManager mWifiManager;
        WifiScanner mWifiScanReceiver;
        public override void OnCreate()
        {
            base.OnCreate();
            mWifiManager = (WifiManager)GetSystemService(Context.WifiService);
            RegisterReceiver(mWifiScanReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));
            mWifiManager.StartScan();
            Task.Run(() => CheckAsync());
        }

        public async Task CheckAsync()
        {
            while (!Destroyed)
            {
                if (mWifiManager == null)
                {
                    mWifiManager = (WifiManager)GetSystemService(Context.WifiService);
                    RegisterReceiver(mWifiScanReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));
                    mWifiManager.StartScan();
                    await Task.Delay(10000);
                }

                if (mWifiManager.ScanResults == null)
                {
                    mWifiManager.StartScan();
                    await Task.Delay(10000);
                }

                if (mWifiManager.ScanResults.Count != 0)
                {
                    if (mWifiManager.ScanResults.ToList().Any(x => x.Ssid.ToLower().Contains("insidev")))
                    {
                        CreateNotificationChannel();
                        Task.Run(() => Notification("Esi Startup", "Sveikas atvykęs į Pertraukties tašką"));
                    }
                    else
                    {
                        Task.Run(() => Notification("Nesi Startup", ""));
                    }
                }
                await Task.Delay(10000);
            }
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }

            var channelName = Resources.GetString(Resource.String.channel_name);
            var channelDescription = GetString(Resource.String.channel_description);
            var ChannelId = GetString(Resource.String.channel_description);
            var channel = new NotificationChannel(ChannelId, channelName, NotificationImportance.Default)
            {
                Description = channelDescription
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }

        void Notification(string title, string content)
        {
            try
            {
                NotificationCompat.Builder builder = new NotificationCompat.Builder(this, GetString(Resource.String.channel_description))
                .SetContentTitle(title)
                .SetContentText(content)
                .SetSmallIcon(Resource.Drawable.logo);

                // Build the notification:
                Notification notification = builder.Build();

                // Get the notification manager:
                NotificationManager notificationManager =
                    GetSystemService(Context.NotificationService) as NotificationManager;

                // Publish the notification:
                const int notificationId = 0;
                notificationManager.Notify(notificationId, notification);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            return StartCommandResult.StickyCompatibility;
        }
        public override void OnDestroy()
        {
            Destroyed = true;
            base.OnDestroy();
            Intent broadcastIntent = new Android.Content.Intent(this, typeof(ServiceRestart));

            SendBroadcast(broadcastIntent);
        }


        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }
    }
}
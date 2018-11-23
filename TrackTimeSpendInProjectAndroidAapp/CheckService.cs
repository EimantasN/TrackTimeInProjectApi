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
using TrackTimeSpendInProjectAndroidAapp.Models;

namespace TrackTimeSpendInProjectAndroidAapp
{
    [Service]
    public class CheckService : Service
    {
        private bool Destroyed = false;
        public static WifiManager mWifiManager;
        WifiScanner mWifiScanReceiver;
        private List<MemberModel> MemebersModelsList { get; set; }

        private readonly string CurrentMemberName = "Eimantas";
        public override void OnCreate()
        {
            base.OnCreate();
            Task.Run(() => UpdateMemebers());
            mWifiManager = (WifiManager)GetSystemService(Context.WifiService);
            RegisterReceiver(mWifiScanReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));
            mWifiManager.StartScan();
            Task.Run(() => CheckAsync());
        }

        public async Task UpdateMemebers()
        {
            try
            {
                API Api = new API();
                var members = await Api.GetMembers();

                if (members != null && members.Count > 0)
                {
                    MemebersModelsList = new List<MemberModel>();
                    foreach (var x in members)
                    {
                        MemebersModelsList.Add(x);
                    }
                }
            }
            catch (Exception e)
            {
            }
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
                        Task.Run(() => Notification("Esi Startup", "Sveikas atvykęs į Pertraukties tašką"));
                        if (!MemebersModelsList.First(x => x.Name.Contains(CurrentMemberName)).Active)
                        {
                            CreateNotificationChannel();
                            Task.Run(() => Start());
                            Task.Run(() => UpdateMemebers());
                        }
                    }
                    else
                    {
                        Task.Run(() => Notification("Nesi Startup", ""));
                        if (MemebersModelsList.First(x => x.Name.Contains(CurrentMemberName)).Active)
                        {
                            Task.Run(() => Stop());
                            Task.Run(() => UpdateMemebers());
                        }
                    }
                }
                await Task.Delay(10000);
            }
        }

        public async Task Stop()
        {
            await Task.Run(() => SwichState(false));
            await Task.Run(() => WorkAsync(false));
        }

        public async Task Start()
        {
            await Task.Run(() => SwichState(true));
            await Task.Run(() => WorkAsync(true));
        }

        public async Task WorkAsync(bool status)
        {
            var member = MemebersModelsList.FirstOrDefault(x => x.Name.Contains(CurrentMemberName));
            if (member != null)
            {
                API api = new API();
                var RequestStatus = await api.Work(status, member);
            }
        }

        public async Task SwichState(bool status)
        {
            bool Finded = false;
            try
            {
                if (MemebersModelsList.Count > 0)
                {
                    var member = MemebersModelsList.FirstOrDefault(x => x.Name.Contains(CurrentMemberName));
                    if (member != null)
                    {
                        Finded = true;
                        member.Active = status;
                        API api = new API();
                        var response = await api.SetActive(member);
                        if (response.Status)
                        {
                            Application.SynchronizationContext.Post(_ =>
                            {
                                member.Active = status;
                            }, null);
                        }
                    }
                }

                Thread.Sleep(250);
            }
            catch (Exception e)
            {
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
            Task.Run(() => Stop());
            base.OnDestroy();
            Intent broadcastIntent = new Android.Content.Intent(this, typeof(ServiceRestart));

            SendBroadcast(broadcastIntent);
        }


        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }
}
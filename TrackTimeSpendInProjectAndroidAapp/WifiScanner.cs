using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TrackTimeSpendInProjectAndroidAapp
{
    [BroadcastReceiver]
    public class WifiScanner : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == WifiManager.ScanResultsAvailableAction)
            {
                List<ScanResult> mScanResults = CheckService.mWifiManager.ScanResults.ToList();
            }
        }
    }
}
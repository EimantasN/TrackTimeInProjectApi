using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TrackTimeSpendInProjectAndroidAapp
{
    [BroadcastReceiver]
    public class ServiceRestart : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            context.StartService(new Android.Content.Intent(context, typeof(CheckService))); ;
        }
    }
}
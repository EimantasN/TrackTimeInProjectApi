using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace TrackTimeSpendInProjectAndroidAapp
{
    public class TaskFragment : SupportFragment
    {
        private View rootView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            rootView = inflater.Inflate(Resource.Layout.activity_main, container, false);

            return rootView;
        }

        internal static System.Threading.Tasks.Task Delay(int v)
        {
            throw new NotImplementedException();
        }
    }
}
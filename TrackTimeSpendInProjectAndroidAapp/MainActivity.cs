using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace TrackTimeSpendInProjectAndroidAapp
{
    [Activity(Label = "@string/app_name", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/logo", Theme = "@style/Theme.DesignDemo", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Holder);

            SupportToolbar toolbar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolbar);

            //SupportActionBar ab = SupportActionBar;
            //ab.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            //ab.SetDisplayHomeAsUpEnabled(true);
            TabLayout tabs = FindViewById<TabLayout>(Resource.Id.tabs);

            ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);

            SetUpViewPager(viewPager);

            tabs.SetupWithViewPager(viewPager);

            Android.Support.V7.App.ActionBar actionBar = SupportActionBar;
            actionBar.Hide();

            if (!IsMyServiceRunning(typeof(CheckService)))
            {
                StartService(new Android.Content.Intent(this, typeof(CheckService)));
            }
        }

        private bool IsMyServiceRunning(Type serviceClass)
        {
            ActivityManager manager = (ActivityManager)GetSystemService(Context.ActivityService);
            foreach (ActivityManager.RunningServiceInfo service in manager.GetRunningServices(int.MaxValue))
            {
                if (serviceClass.Name == service.Service.ClassName)
                {
                    return true;
                }
            }
            return false;
        }

        private void SetUpViewPager(ViewPager viewPager)
        {
            TabAdapter adapter = new TabAdapter(SupportFragmentManager);
            adapter.AddFragment(new Members(), "Members");
            adapter.AddFragment(new TaskFragment(), "Task");

            viewPager.Adapter = adapter;
        }

        public class TabAdapter : FragmentPagerAdapter
        {
            public List<SupportFragment> Fragments { get; set; }
            public List<string> FragmentsNames { get; set; }

            public TabAdapter(SupportFragmentManager sfm) : base(sfm)
            {
                Fragments = new List<SupportFragment>();
                FragmentsNames = new List<string>();
            }

            public void AddFragment(SupportFragment fragment, string name)
            {
                Fragments.Add(fragment);
                FragmentsNames.Add(name);
            }

            public override int Count
            {
                get
                {
                    return Fragments.Count;
                }
            }

            public override SupportFragment GetItem(int position)
            {
                return Fragments[position];
            }

            public override ICharSequence GetPageTitleFormatted(int position)
            {
                return new Java.Lang.String(FragmentsNames[position]);
            }
        }
    }
}
using Android.App;
using Android.OS;
using Android.Support.Constraints;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Refractored.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrackTimeSpendInProjectAndroidAapp.Models;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace TrackTimeSpendInProjectAndroidAapp
{
    public class Members : SupportFragment
    {
        private View rootView;

        private SwipeRefreshLayout refresh;

        private ConstraintLayout TimeContainer;
        private TextView Hours;
        private TextView Minutes;
        private TextView HoursText;
        private TextView MinutesText;

        private ConstraintLayout ActiveMemebersContainer;
        private CircleImageView FirstImg;
        private CircleImageView SecondImg;
        private CircleImageView ThirdImg;
        private TextView AcitiveMembersText;

        public static MyList<MemberModel> mData;
        private RecyclerView.LayoutManager mLayoutManager;
        public static RecyclerView.Adapter mAdapter;
        private RecyclerView mRecyclerView;

        private Button Button;

        private TimeSpan SpendTime = new TimeSpan();

        private readonly DateTime StartedTime;

        private bool Started { get; set; } = false;

        public bool AcitveTimeAdding = false;

        private API API = new API();

        private readonly string CurrentMemberName = "Eimantas";

        private List<MemberModel> MemebersModelsList { get; set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            rootView = inflater.Inflate(Resource.Layout.activity_main, container, false);

            refresh = rootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);

            TimeContainer = rootView.FindViewById<ConstraintLayout>(Resource.Id.constraintLayout);
            Hours = rootView.FindViewById<TextView>(Resource.Id.Hours);
            Minutes = rootView.FindViewById<TextView>(Resource.Id.Minutes);
            HoursText = rootView.FindViewById<TextView>(Resource.Id.HoursText);
            MinutesText = rootView.FindViewById<TextView>(Resource.Id.MinutesText);

            TimeContainer.Click += TimeContainer_Click;

            ActiveMemebersContainer = rootView.FindViewById<ConstraintLayout>(Resource.Id.constraintLayout3);
            FirstImg = rootView.FindViewById<CircleImageView>(Resource.Id.imageView9);
            SecondImg = rootView.FindViewById<CircleImageView>(Resource.Id.imageView10);
            ThirdImg = rootView.FindViewById<CircleImageView>(Resource.Id.imageView11);
            AcitiveMembersText = rootView.FindViewById<TextView>(Resource.Id.textView12);

            mRecyclerView = rootView.FindViewById<RecyclerView>(Resource.Id.RCMembers);

            mData = new MyList<MemberModel>();
            mLayoutManager = new LinearLayoutManager(Activity);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            mAdapter = new RecyclerAdapter(mData, mRecyclerView);
            mData.Adapter = mAdapter;
            mRecyclerView.SetAdapter(mAdapter);

            //mData.Add(new MemberModel { Name = "Eimantas Noreika" });
            //mData.Add(new MemberModel { Name = "Lukas Jokubauskas" });
            //mData.Add(new MemberModel { Name = "Renaldas Štilpa" });

            Button = rootView.FindViewById<Button>(Resource.Id.button);

            Button.Click += Button_Click;

            mData.Add(null);
            Task.Run(() => UpdateMemebers());
            Task.Run(() => ShowActiveMembers());

            refresh.Refresh += Refresh_Refresh;

            return rootView;
        }

        private void Refresh_Refresh(object sender, EventArgs e)
        {
            ActiveMemebersContainer.Visibility = ViewStates.Gone;
            MemebersModelsList = new List<MemberModel>();
            Erase();
            refresh.Refreshing = true;
            Task.Run(() => RefreshAsync());
        }

        public async Task RefreshAsync()
        {
            if (Started)
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    Button.Text = "Stoping clock";
                    Started = false;
                }, null);

                while (AcitveTimeAdding)
                {
                    System.Threading.Thread.Sleep(100);
                }
                Application.SynchronizationContext.Post(_ =>
                {
                    Button.Text = "Start";
                }, null);

                await Task.Run(() => WorkAsync(false));
            }

            await UpdateMemebers();
            Application.SynchronizationContext.Post(_ => { refresh.Refreshing = false; }, null);
        }

        public async Task UpdateMemebers()
        {
            try
            {
                var members = await API.GetMembers();

                if (members != null)
                {
                    if (members.Count > 0)
                    {
                        MemebersModelsList = new List<MemberModel>();

                        if (mData.Count != 0)
                            if (mData[0] != null)
                                Erase();

                        foreach (var x in members)
                        {
                            Application.SynchronizationContext.Post(_ =>
                           {
                               MemebersModelsList.Add(x);
                               if (x.Name.Contains(CurrentMemberName))
                                   SpendTime = new TimeSpan(0, (int)x.CurrentDayTime, 0);
                               mData.Add(x);
                           }, null);
                        }
                        Application.SynchronizationContext.Post(_ =>
                        {
                            mData.RemoveEmpty();
                        }, null);
                    }
                    else
                        SnackBarMessage("Members Count is 0");
                }
                else
                    SnackBarMessage("Members is null");

            }
            catch (Exception e)
            {
                SnackBarMessage(e.Message);
            }
            finally
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    Button.Visibility = ViewStates.Visible;
                    if (SpendTime.TotalMinutes != 0)
                    {
                        TimeContainer.Visibility = ViewStates.Visible;
                        UpdateTime(-1, new TimeSpan(0, 0, 0));
                    }
                }, null);
            }
        }

        public async Task ShowActiveMembers()
        {
            Thread.Sleep(1000);
            try
            {
                while (true)
                {
                    if (MemebersModelsList != null && MemebersModelsList.Count > 1 && refresh.Refreshing == false)
                    {
                        var models = await API.GetMembers();
                        if (models != null && models.Count > 0)
                        {
                            foreach (var x in models)
                            {
                                try
                                {
                                    if (MemebersModelsList.First(y => y.Id == x.Id).Active != x.Active && refresh.Refreshing == false)
                                    {
                                        Application.SynchronizationContext.Post(_ => { MemebersModelsList.First(y => y.Id == x.Id).Active = x.Active; }, null);
                                    }
                                }
                                catch (Exception)
                                {
                                    Task.Run(() => UpdateMemebers());
                                }
                            }
                        }
                        if (refresh.Refreshing == false)
                            CheckActiveMembersContainer();
                    }

                    Thread.Sleep(2500);
                }
            }
            catch (Exception e)
            {
                SnackBarMessage(e.Message);
            }
        }

        public bool ShowActiveMemebersImages(MemberModel x)
        {
            bool SomethingHided = false;
            if (x.Name.Contains("Eimantas"))
            {
                if (x.Active)
                {
                    if (FirstImg.Visibility != ViewStates.Visible)
                    {
                        Application.SynchronizationContext.Post(_ => { FirstImg.Visibility = ViewStates.Visible; }, null);
                    }
                }
                else
                {
                    if (FirstImg.Visibility != ViewStates.Gone)
                    {
                        Application.SynchronizationContext.Post(_ => { FirstImg.Visibility = ViewStates.Gone; }, null);
                        SomethingHided = true;
                    }
                }
            }
            else if (x.Name.Contains("Lukas"))
            {
                if (x.Active)
                {
                    if (SecondImg.Visibility != ViewStates.Visible)
                    {
                        Application.SynchronizationContext.Post(_ => { SecondImg.Visibility = ViewStates.Visible; }, null);
                    }
                }
                else
                {
                    if (SecondImg.Visibility != ViewStates.Gone)
                    {
                        Application.SynchronizationContext.Post(_ => { SecondImg.Visibility = ViewStates.Gone; }, null);
                        SomethingHided = true;
                    }
                }
            }
            else if (x.Name.Contains("Renaldas"))
            {
                if (x.Active)
                {
                    if (ThirdImg.Visibility != ViewStates.Visible)
                    {
                        Application.SynchronizationContext.Post(_ => { ThirdImg.Visibility = ViewStates.Visible; }, null);
                    }
                }
                else
                {
                    if (ThirdImg.Visibility != ViewStates.Gone)
                    {
                        Application.SynchronizationContext.Post(_ => { ThirdImg.Visibility = ViewStates.Gone; }, null);
                        SomethingHided = true;
                    }
                }
            }
            return SomethingHided;
        }

        public void CheckActiveMembersContainer()
        {
            if (MemebersModelsList.All(x => x.Active == false))
                Application.SynchronizationContext.Post(_ => { ActiveMemebersContainer.Visibility = ViewStates.Gone; }, null);
            else
            {
                Application.SynchronizationContext.Post(_ => { ActiveMemebersContainer.Visibility = ViewStates.Visible; }, null);
                foreach (var x in MemebersModelsList)
                {
                    if (ShowActiveMemebersImages(x))
                    {
                        if (MemebersModelsList.All(y => y.Active == false))
                            Application.SynchronizationContext.Post(_ => { ActiveMemebersContainer.Visibility = ViewStates.Gone; }, null);
                    };
                }
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
                        var response = await API.SetActive(member);
                        if (response.Status)
                        {
                            Application.SynchronizationContext.Post(_ =>
                            {
                                member.Active = status;
                            }, null);
                        }
                        else
                            SnackBarMessage("Set Active failed " + response.Msg);
                    }
                    else
                        SnackBarMessage("Member not found");

                    if (!Finded)
                        SnackBarMessage("Current memeber not found");
                }
                else
                    SnackBarMessage("mData in Empty");

                Thread.Sleep(250);
                CheckActiveMembersContainer();
            }
            catch (Exception e)
            {
                SnackBarMessage(e.Message);
            }
        }

        public void SnackBarMessage(string Message)
        {
            Application.SynchronizationContext.Post(_ =>
            {
                Snackbar snackbar = Snackbar.Make(rootView, Message, Snackbar.LengthLong);
                snackbar.Show();
            }, null);
        }

        private void TimeContainer_Click(object sender, EventArgs e)
        {
            Snackbar snackbar = Snackbar.Make(rootView, "Work time: " + SpendTime.ToString(@"dd\.hh\:mm\:ss"), Snackbar.LengthShort);
            snackbar.Show();
        }

        public void CountTimeAsync()
        {
            while (!Started)
                System.Threading.Thread.Sleep(50);

            AcitveTimeAdding = true;

            Application.SynchronizationContext.Post(_ => { TimeContainer.Visibility = ViewStates.Visible; }, null);

            DateTime StartedTime = DateTime.Now;
            TimeSpan Temp;
            int Minute = -1;
            while (Started)
            {
                Temp = DateTime.Now.Subtract(StartedTime);
                UpdateTime(Minute, Temp);
                StartedTime = DateTime.Now;
                System.Threading.Thread.Sleep(1000);
            }
            Temp = DateTime.Now.Subtract(StartedTime);
            UpdateTime(Minute, Temp);

            AcitveTimeAdding = false;
        }

        public void UpdateTime(int Minute, TimeSpan Temp)
        {
            Application.SynchronizationContext.Post(_ =>
            {

                SpendTime = SpendTime.Add(Temp);
                if (SpendTime.Minutes != Minute)
                {
                    string HourValue = SpendTime.Hours < 10 ? "0" + SpendTime.Hours : SpendTime.Hours.ToString();

                    if (HourValue == "00")
                    {
                        HoursText.Text = string.Empty;
                        Hours.Text = string.Empty;
                    }
                    else
                        Hours.Text = HourValue;

                    Minutes.Text = SpendTime.Minutes < 10 ? "0" + SpendTime.Minutes : SpendTime.Minutes.ToString();
                    Minute = SpendTime.Minutes;
                }
            }, null);
        }

        private void Button_Click(object sender, System.EventArgs e)
        {
            if (mData.Count > 1)
            {
                Task.Run(() => ClockButtonControlAsync());
            }
        }

        public async Task ClockButtonControlAsync()
        {
            if (Button.Text != "Stoping clock")
            {
                if (Started)
                {
                    await Task.Run(() => SwichState(false));
                    await Task.Run(() => WorkAsync(false));

                    Application.SynchronizationContext.Post(_ =>
                    {
                        Button.Text = "Stoping clock";
                        Started = false;
                    }, null);

                    while (AcitveTimeAdding)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    Application.SynchronizationContext.Post(_ =>
                    {
                        Button.Text = "Start";
                    }, null);
                }
                else
                {
                    await Task.Run(() => SwichState(true));
                    await Task.Run(() => WorkAsync(true));

                    Application.SynchronizationContext.Post(_ =>
                    {
                        Started = true;
                        Button.Text = "Stop";
                    }, null);
                    Task.Run(() => CountTimeAsync());
                }
            }
        }


        public async Task WorkAsync(bool status)
        {
            var member = MemebersModelsList.FirstOrDefault(x => x.Name.Contains(CurrentMemberName));
            if (member != null)
            {
                var RequestStatus = await API.Work(status, member);
                if (!RequestStatus.Status)
                {
                    SnackBarMessage(RequestStatus.Msg);
                }
            }
        }
        public void Erase()
        {
            for (int i = mData.Count; i >= 0; i--)
            {
                if (mData.Count == 1)
                {
                    mData.Remove(0);
                }
                if (mData.Count > 1)
                {
                    mData.Remove(mData.Count - 1);
                }
            }
            mData.Erase();
        }

        public class MyList<T>
        {
            private List<T> mItems;
            private RecyclerView.Adapter mAdapter;

            public void Erase()
            {
                mItems = new List<T>();
            }

            public MyList()
            {
                mItems = new List<T>();
            }

            public RecyclerView.Adapter Adapter
            {
                get => mAdapter;
                set => mAdapter = value;
            }

            public void Add(T item)
            {
                mItems.Add(item);

                if (Adapter != null)
                {
                    Adapter.NotifyItemInserted(Count);
                }
            }

            public void Remove(int position)
            {
                mItems.RemoveAt(position);

                if (Adapter != null)
                {
                    Adapter.NotifyItemRemoved(0);
                }
            }

            public T this[int index]
            {
                get => mItems[index];
                set => mItems[index] = value;
            }

            public int Count => mItems.Count;


            public void RemoveEmpty()
            {
                int Position = -1;
                for (int i = 0; i < mItems.Count; i++)
                {
                    if (mItems[i] == null)
                    {
                        Position = i;
                        break;
                    }
                }

                if (Position != -1)
                {
                    mItems.RemoveAt(Position);

                    if (Adapter != null)
                    {
                        Adapter.NotifyItemRemoved(0);
                    }
                }
            }

            public void clear()
            {
                int size = mItems.Count;
                Erase();
                mAdapter.NotifyItemRangeRemoved(0, size);
            }
        }

        public class RecyclerAdapter : RecyclerView.Adapter
        {
            private readonly MyList<MemberModel> mData;
            private readonly RecyclerView mRecyclerView;

            public RecyclerAdapter(MyList<MemberModel> Data, RecyclerView recyclerView)
            {
                mData = Data;
                mRecyclerView = recyclerView;
            }

            public class Loading : RecyclerView.ViewHolder
            {
                public View LoadingView { get; set; }

                public Loading(View view) : base(view)
                { }
            }

            public class MemberRCModel : RecyclerView.ViewHolder
            {
                public View MemberView { get; set; }
                public TextView mName { get; set; }
                public TextView mCurrentTask { get; set; }
                public TextView mCurrentActiveTime { get; set; }
                public TextView mTotalTime { get; set; }

                public ImageView mImage { get; set; }

                public MemberRCModel(View view) : base(view)
                { MemberView = view; }
            }

            public override int GetItemViewType(int position)
            {
                try
                {
                    if (mData[position] != null)
                    {
                        return Resource.Layout.Member;
                    }

                    else
                    {
                        return Resource.Layout.Loading;
                    }
                }
                catch { return Resource.Layout.Loading; }
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                if (viewType == Resource.Layout.Member)
                {
                    View NewTime = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Member, parent, false);
                    TextView Name = NewTime.FindViewById<TextView>(Resource.Id.Name);
                    TextView CurrentTask = NewTime.FindViewById<TextView>(Resource.Id.CurrentTask);
                    TextView CurrentActiveTime = NewTime.FindViewById<TextView>(Resource.Id.CurrentTime);

                    TextView TotalTime = NewTime.FindViewById<TextView>(Resource.Id.TotalTimeValue);
                    ImageView Image = NewTime.FindViewById<ImageView>(Resource.Id.Image);

                    MemberRCModel view = new MemberRCModel(NewTime)
                    {
                        mName = Name,
                        mCurrentTask = CurrentTask,
                        mCurrentActiveTime = CurrentActiveTime,
                        mTotalTime = TotalTime,
                        mImage = Image
                    };
                    return view;
                }
                else
                {
                    View Loading = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Loading, parent, false);

                    Loading view = new Loading(Loading) { };

                    return view;
                }
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                if (holder is MemberRCModel)
                {
                    MemberRCModel myHolder = holder as MemberRCModel;
                    myHolder.mName.Text = mData[position].Name;
                    myHolder.mCurrentTask.Text = "TODO";

                    TimeSpan time = new TimeSpan(0, (int)mData[position].CurrentDayTime, 0);

                    if (time.Hours != 0) { myHolder.mCurrentActiveTime.Text = time.Hours + " Hours - " + time.Minutes + " Min"; }
                    else { myHolder.mCurrentActiveTime.Text = time.Minutes + " Min"; }

                    time = new TimeSpan(0, (int)mData[position].TotalTime, 0);

                    if (time.Days != 0) { myHolder.mTotalTime.Text = time.Days + " Days - " + time.Hours + " Hours - " + time.Minutes + " Min"; }
                    else if (time.Hours != 0) { myHolder.mTotalTime.Text = time.Hours + " Hours - " + time.Minutes + " Min"; }
                    else { myHolder.mTotalTime.Text = time.Minutes + " Min"; }

                    if (mData[position].Name.Contains("Eimantas"))
                    {
                        myHolder.mImage.SetImageResource(Resource.Drawable.eimantas);
                    }
                    else if (mData[position].Name.Contains("Lukas"))
                    {
                        myHolder.mImage.SetImageResource(Resource.Drawable.lukas);
                    }
                    else if (mData[position].Name.Contains("Renaldas"))
                    {
                        myHolder.mImage.SetImageResource(Resource.Drawable.renaldas);
                    }
                }
            }

            public override int ItemCount => mData.Count;
        }
    }
}
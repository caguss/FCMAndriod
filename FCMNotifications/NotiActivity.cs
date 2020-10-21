using Android.App;
using Android.Gms.Common;
using Android.OS;
using Android.Util;
using Android.Widget;
using Firebase.Iid;
using Firebase.Messaging;
using System;
using Wahid.SwipemenuListview;
using Android.Graphics.Drawables;
using Android.Graphics;
using Xamarin.Essentials;
using System.IO;
using Android.Views;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Android.Content;


namespace FCMNotifications
{
    [Activity(Label = "FCMNotifications", MainLauncher = false, Icon = "@drawable/icon")]
    public class NotiActivity : Activity, ISwipeMenuCreator, IOnMenuItemClickListener
    {
    
        const string TAG = "NotiActivity";
        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static readonly int NOTIFICATION_ID = 114;
        string[] items;
        string[] LogData;
        int[] userinfo = new int[2];
        SwipeMenuListView mainList;
        DBClass dBClass = new DBClass();
        public void Create(SwipeMenu menu)
        {
            userinfo = Intent.Extras.GetIntArray("userinfo");

            SwipeMenuItem callItem = new SwipeMenuItem(this)
            {
                Width = 150,
                Background = new ColorDrawable(Color.AliceBlue),
                //IconRes = Resource.Mipmap.phone_co,
                TitleRes = Resource.String.finish,
                TitleSize = 10
            };
            menu.AddMenuItem(callItem);

                SwipeMenuItem copyItem = new SwipeMenuItem(this)
            {
                Background = new ColorDrawable(Color.Azure),
                Width = 150,
                TitleRes = Resource.String.ok,
                TitleSize = 10

            };
            menu.AddMenuItem(copyItem);
        }

        public bool OnMenuItemClick(int position, SwipeMenu menu, int index)
        {
            //contact = "시간 : 20/08/06 13:58:06\n제목 : Title\n내용 : TimerSending"
            string contact = LogData[position];
            switch (index)
            {
                case 0: // 처리완료

                    //처리완료하시겠습니까?
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("처리완료");
                    alert.SetMessage("처리완료하시겠습니까?");

                    alert.SetPositiveButton("취소", (senderAlert, args) =>
                    {

                    });

                    alert.SetNegativeButton("처리완료", (senderAlert, args) =>
                    {

                        //BOOL 프로시져
                        if(dBClass.TaskFinish_I10(userinfo, contact, "처리완료"))
                        {
                            //처리완료되었습니다.
                            string toast = string.Format("처리완료되었습니다.");
                            Toast.MakeText(this, toast, ToastLength.Long).Show();
                            //처리완료처리? 색깔변경?
                            LogData[position] = contact + "처리완료";
                            RefreshTask();
                        }
                        else
                        {
                            //처리실패
                            string toast = string.Format("처리실패! 잠시 후 다시 시도해 주세요.");
                            Toast.MakeText(this, toast, ToastLength.Long).Show();
                        }


                        
                    });
                    Dialog dialog = alert.Create();
                    dialog.Show();

                    break;
                case 1: // 확인
                    //확인
                    dBClass.TaskFinish_I10(userinfo, contact, "확인");
                    //확인처리? 색깔 변경?
                    LogData[position] = contact + "확인";
                    RefreshTask();
                    break;
            }
            return false;
        }

      

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var directory = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var filename = System.IO.Path.Combine(directory.ToString(), "LogData.txt");
            try
            {
                /*
                // 알람이 왔던 데이터들을 바인딩
                using (StreamReader sr = new StreamReader(System.IO.File.OpenRead(filename)))
                {
                    int i = 0;
                    while (!sr.EndOfStream)
                    {
                        string log = sr.ReadLine();

                        setLogdata[i] = log.Replace('%', '\n');
                        i++;
                    }
                }
                */
                LogData = dBClass.TaskList_R10(userinfo);


            }
            catch (Exception)
            {
                LogData = new string[1];
                LogData[0] = "불러오기 에러%";
            }
            //파일이없을경우
            if (LogData == null)
            {
                LogData = new string[1];
                LogData[0] = "이력이 없습니다.%";
            }

            else
            {
                /*
                int isnull = 0;
                while (isnull < 101)
                {
                    if (LogData[isnull] != null)
                    {
                        isnull++;

                    }
                }
                LogData = new string[isnull];
                for (int i = 0; i < LogData.Length; i++)
                {
                    LogData[i] = LogData[i];
                }
               */
            }
            RefreshTask();

            

            //mainlist background color 작업
        }

        private void RefreshTask()
        {
            for (int i = 0; i < LogData.Length; i++)
            {
                string taskdata = LogData[i].Split('%')[0];
                string taskstatus = LogData[i].Split('%')[1];
                if (taskstatus != "")
                {
                    switch (taskstatus)
                    {
                        case "확인":
                            mainList.FocusedChild.SetBackgroundColor(new Color(50,50,50));
                            break;
                        case "처리완료":
                            break;
                    }
                }

                LogData[i] = taskdata;

            }

            items = LogData;

            //items = new string[] { "Xamarin","Android","IOS","Windows","Xamarin-Native","Xamarin-Forms"};
            SetContentView(Resource.Layout.logbox);


            mainList = FindViewById<SwipeMenuListView>(Resource.Id.listView1);
            //mainList = (ListView)FindViewById<ListView>(Resource.Id.listView1);
            mainList.Adapter = new ArrayAdapter(this, Resource.Layout.ListTextLayout, items);
            mainList.MenuCreator = this;
            mainList.MenuItemClickListener = this;
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var param = mainList.LayoutParameters;
            param.Height = PixelsToDp(57 * mainList.Count);
        }

        private int PixelsToDp(int pixels)
        {
            return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, pixels, Resources.DisplayMetrics);
        }


    }
}

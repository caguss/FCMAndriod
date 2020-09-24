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
        string[] setLogdata = new string[100];
        SwipeMenuListView mainList;


        public void Create(SwipeMenu menu)
        {
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
            //var contact = ((listview.Adapter as SwipeMenuAdapter).WrappedAdapter as ContactUsSwipeAdapter).Items[position];
            string contact = LogData[position];
            switch (index)
            {
                //스와이프 기능 추가
                case 0: // 처리완료
                    int type = menu.GetViewType();
                    if (type == 0)
                    {

                        EditText userName = null;// FindViewById<EditText>(Resource.Id.txtUser);
                        EditText userPwd = null;//FindViewById<EditText>(Resource.Id.txtPwd);
                        var usrid = userName.Text;
                        var usrpwd = userPwd.Text;


                     
                    }
                    else
                    {
                    }
                    break;
                case 1: // 확인
                    
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

            }
            catch (Exception)
            {

            }
            //파일이없을경우
            if (setLogdata[0] == null)
            {
                LogData = new string[1];
                LogData[0] = "이력이 없습니다.";
            }
            else
            {
                int isnull = 0;
                while (setLogdata[isnull] != null)
                {
                    isnull++;
                }
                LogData = new string[isnull];
                for (int i = 0; i < LogData.Length; i++)
                {
                    LogData[i] = setLogdata[i];
                }
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

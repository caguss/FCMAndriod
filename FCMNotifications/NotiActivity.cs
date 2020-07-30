using Android.App;
using Android.Gms.Common;
using Android.OS;
using Android.Util;
using Android.Widget;
using Firebase.Iid;
using Firebase.Messaging;
using System;
using Xamarin.Essentials;
using System.IO;

namespace FCMNotifications
{
    [Activity(Label = "FCMNotifications", MainLauncher = false, Icon = "@drawable/icon")]
    public class NotiActivity : Activity
    {
        const string TAG = "NotiActivity";
        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static readonly int NOTIFICATION_ID = 114;
        string[] items;
        string[] LogData;
        string[] setLogdata = new string[100];
        ListView mainList;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var directory = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var filename = Path.Combine(directory.ToString(), "LogData.txt");
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

            mainList = (ListView)FindViewById<ListView>(Resource.Id.listView1);
            mainList.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, items);


            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var param = mainList.LayoutParameters;
            param.Height = PixelsToDp((Convert.ToInt32(mainDisplayInfo.Height) + 150));
        }
        private int PixelsToDp(int pixels)
        {
            return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, pixels, Resources.DisplayMetrics);
        }


    }
}

﻿using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.OS;
using Android.Util;
using Android.Widget;
using Firebase.Iid;
using Firebase.Messaging;
using System.Text;
using System;
using System.IO;
using Android;
using Android.Views;
using Android.Support.V4.App;
using Android.Support.Design.Widget;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Android.Net;
using Newtonsoft.Json;

namespace FCMNotifications
{
    [Activity(Label = "FCMNotifications", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        const string TAG = "MainActivity";

        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static readonly int NOTIFICATION_ID = 114;
        public static int noti = 1001;
        private EditText loginID;
        private EditText loginPW;
        private TextView msgText;
        private CheckBox checkbox1;

        private Spinner spinner1, spinner2;
        private Button btnSubmit;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            msgText = FindViewById<TextView>(Resource.Id.msgText);

            checkbox1 = FindViewById<CheckBox>(Resource.Id.checkBox1);

            this.InitializeIDPW();

            if (Intent.Extras != null)
            {
                foreach (var key in Intent.Extras.KeySet())
                {
                    var value = Intent.Extras.GetString(key);
                    Log.Debug(TAG, "Key: {0} Value: {1}", key, value);
                }
            }

            AddItemsOnSpinner1(); // spinner1 바인딩

            AddItemsOnSpinner2(); // spinner2 바인딩
            
            AddListenerOnButton(); // 로그인버튼 이벤트 추가
            

            #region 테스트
            /////////////////////////////////////////
            CreateNotificationChannel();
            IsPlayServicesAvailable();
            var logTokenButton = FindViewById<Button>(Resource.Id.logTokenButton);
            logTokenButton.Click += delegate { Log.Debug(TAG, "InstanceID token: " + FirebaseInstanceId.Instance.Token); };
            var subscribeButton = FindViewById<Button>(Resource.Id.subscribeButton);
            subscribeButton.Click += delegate
                                     {
                                         FirebaseMessaging.Instance.SubscribeToTopic("news");
                                         Log.Debug(TAG, "Subscribed to remote notifications");
                                     };
            #endregion
        }


        private void InitializeIDPW() //id와 비밀번호 불러오기
        {
            loginID = FindViewById<EditText>(Resource.Id.txtID);
            loginPW = FindViewById<EditText>(Resource.Id.txtPW);

            string content;


            var directory = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var filename = Path.Combine(directory.ToString(), "UserData.txt");

            try
            {
                using (StreamReader sr = new StreamReader(System.IO.File.OpenRead(filename)))
                {
                    content = sr.ReadLine();
                    loginID.Text = content.Split("%")[0];
                    loginPW.Text = content.Split("%")[1];
                }
            }
            catch (Exception)
            {

            }

        }
        #region test 메소드
        public bool IsPlayServicesAvailable()
        {
            var resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    msgText.Text = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                }
                else
                {
                    msgText.Text = "This device is not supported";
                    Finish();
                }

                return false;
            }

            msgText.Text = "Google Play Services is available.";
            return true;
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification 
                // channel on older versions of Android.
                return;
            }

            var channel = new NotificationChannel(CHANNEL_ID, "FCM Notifications", NotificationImportance.Default)
            {
                Description = "Firebase Cloud Messages appear in this channel"
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
        #endregion

        #region spinner 메소드
        public void AddItemsOnSpinner1()
        {
            /*
            var client = new HttpClient();
            client.BaseAddress = new Uri("localhost:64408");
            HttpResponseMessage res = await client.GetAsync("/company");
            var result = await res.Content.ReadAsStringAsync();

            spinner1 = FindViewById<Spinner>(Resource.Id.spinner);
            spinner1.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_ItemSelected);
            */
            //이거 수정
            spinner1 = (Spinner)FindViewById(Resource.Id.spinner);

            var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.country_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner1.Adapter = adapter;
            
            /*
            List<System.String> list = new List<System.String>
            {
                "기업 1",
                "기업 2", //추후 서버에서 불러옴
                "기업 3"
            };

            //List<System.String> list1 =  Resource.String.country_prompt;
            */
            
            spinner1.ItemSelected += Spinner_ItemSelected;
            
        }




        public void AddItemsOnSpinner2()
        {
            spinner2 = (Spinner)FindViewById(Resource.Id.spinner2);
            List<System.String> list = new List<System.String>
            {
                "서버 1",
                "서버 2", //추후 서버에서 불러옴
                "서버 3"
            };



            ArrayAdapter<System.String> dataAdapter = new ArrayAdapter<System.String>(this,
                Android.Resource.Layout.SimpleSpinnerItem, list);  //simple_spinner_item
            dataAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);//simple_spinner_dropdown_item

            spinner2.Adapter = dataAdapter;
            spinner2.ItemSelected += Spinner_ItemSelected;
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string toast = string.Format("{0}을 선택하셨습니다.", spinner.GetItemAtPosition(e.Position));
            Toast.MakeText(this, toast, ToastLength.Long).Show();
        }
        #endregion

        #region 로그인버튼 계획
        private void AddListenerOnButton()
        {
            btnSubmit = (Button)FindViewById(Resource.Id.btnLogin);
            btnSubmit.Click += BtnSubmit_Click;

        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {



            ///////////////////여기서 id,pw,tok send

            LoginCheck();

            
            string toast = string.Format("기업 : " + (spinner1.SelectedItem.ToString()) + "\n서버 : " + (spinner2.SelectedItem.ToString()));
            Toast.MakeText(this, toast, ToastLength.Long).Show();
            

            
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.WriteExternalStorage))
            {
                // Provide an additional rationale to the user if the permission was not granted
                // and the user would benefit from additional context for the use of the permission.
                Log.Info(TAG, "Permission Check");

                var requiredPermissions = new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage };
                Snackbar.Make(checkbox1,
                               Resource.String.permission_location_rationale,
                               Snackbar.LengthIndefinite)
                        .SetAction(Resource.String.ok,
                                   new Action<View>(delegate (View obj)
                                   {
                                       ActivityCompat.RequestPermissions(this, requiredPermissions, 114);
                                   }
                        )
                ).Show();
            }

            ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage }, 114);




            if (checkbox1.Checked)
            {


                var directory = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                var filename = Path.Combine(directory.ToString(), "UserData.txt");
                string tok = new MyFirebaseIIDService().GetTokenData();
                using (var writer = new StreamWriter(System.IO.File.Create(filename)))
                {
                    writer.WriteLine(loginID.Text + "%" + loginPW.Text + "%" + tok);
                }

            }
            else
            {
                var directory = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                var filename = Path.Combine(directory.ToString(), "UserData.txt");
                string tok = new MyFirebaseIIDService().GetTokenData();
                using (var writer = new StreamWriter(System.IO.File.Create(filename)))
                {
                    writer.WriteLine("ID" + "%" + "" + "%" + tok);
                }
            }


            var intent = new Intent(this, typeof(NotiActivity));
            StartActivity(intent);

            // get a writable file path
            // write the data to the writable path - now you can read and write it
        }

        #endregion



        #region servere connect
      
        public void SyncCompany()
        {
            // 회사이름 불러올때

        }






        public void LoginCheck()
        {
            //회사별 db접속 후 일치하는 아이디와 비밀번호 검색 후 
            //로그인 확인
            //로그인한 기록 이름 저장 후 notiactivity로 넘겨야하나
        }
        #endregion
    }
}

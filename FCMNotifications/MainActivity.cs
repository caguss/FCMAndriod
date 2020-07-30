using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.OS;
using Android.Util;
using Android.Widget;
using System.Resources;
using Android.Support;
using Firebase.Iid;
using Firebase.Messaging;
using Android.Content.Res;
using System.Text;
using System;
using System.Xml;
using Java.IO;
using System.IO;
using Android;
using Android.Views;
using Android.Support.V4.App;
using Android.Support.Design.Widget;
using static Android.Resource;

namespace FCMNotifications
{
    [Activity(Label = "FCMNotifications", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {


        const string TAG = "MainActivity";





        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static readonly int NOTIFICATION_ID = 114;
        public static int noti = 1001;

        EditText loginID;
        EditText loginPW;

        TextView msgText;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            msgText = FindViewById<TextView>(Resource.Id.msgText);

            this.InitializeIDPW();
            var checkbox1 = FindViewById<CheckBox>(Resource.Id.checkBox1);


            if (Intent.Extras != null)
            {
                foreach (var key in Intent.Extras.KeySet())
                {
                    var value = Intent.Extras.GetString(key);
                    Log.Debug(TAG, "Key: {0} Value: {1}", key, value);
                }
            }




            //로그인버튼 계획
            var loginButton = FindViewById<Button>(Resource.Id.btnLogin);
            loginButton.Click += delegate
            {
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
                        writer.WriteLine(loginID.Text + "%" + loginPW.Text+"%"+ tok);
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



            };
            






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

        }

    
        private void InitializeIDPW()
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

            var notificationManager = (NotificationManager) GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }



    }
}

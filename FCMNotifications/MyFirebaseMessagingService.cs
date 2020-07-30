using System;
using System.Collections.Generic;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Text.Format;
using Android.Util;

using Firebase.Messaging;
using static Android.Resource;

namespace FCMNotifications
{
    [Service]
    public class PeriodicService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
        Intent intent;
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {

            intent = new Intent(this, typeof(PeriodicService));
            StartService(intent);

            return StartCommandResult.NotSticky;
        }
    }

    [BroadcastReceiver]
    public class BackgroundReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            PowerManager pm = (PowerManager)context.GetSystemService(Context.PowerService);
            PowerManager.WakeLock wakeLock = pm.NewWakeLock(WakeLockFlags.Partial, "BackgroundReceiver");
            wakeLock.Acquire();


            wakeLock.Release();
        }
    }

    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {


        public static readonly string NOTIFICATION_REPLY = "NotificationReply";
        public static readonly string CHANNNEL_ID = "SimplifiedCodingChannel";
        public static readonly string CHANNEL_NAME = "SimplifiedCodingChannel";
        public static readonly string CHANNEL_DESC = "This is a channel for Simplified Coding Notifications";
        public static readonly string KEY_INTENT_MORE = "keyintentmore";
        public static readonly string KEY_INTENT_HELP = "keyintenthelp";
        public static readonly int REQUEST_CODE_MORE = 100;
        public static readonly int REQUEST_CODE_HELP = 101;



        const string TAG = "MyFirebaseMsgService";
        internal static readonly string FINISH_KEY = "finish";
        internal static readonly string FINISH_LABEL = "처리 완료";


      


        public override void OnMessageReceived(RemoteMessage message)
        {
            Log.Debug(TAG, "From: " + message.From);

            var body = message.GetNotification().Body;
            var head = message.GetNotification().Title;
            var timestamp = message.SentTime;
            double seconds = timestamp / 1000;
            DateTime utcConverted = new DateTime(1970, 1, 1, 9, 0, 0, DateTimeKind.Utc).AddSeconds(seconds).ToLocalTime();
            string[] lines = null;

            //알람시 logdata 에 작성
            var directory = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var filename = Path.Combine(directory.ToString(), "LogData.txt");

            try // 기존 로그 데이터 불러옴
            {
                lines = File.ReadAllLines(filename);
            }
            catch (Exception)
            {
            }
            try // 기존 로그데이터 + 신규 로그데이터
            {
                using (var writer = new StreamWriter(System.IO.File.OpenWrite(filename)))
                {
                    writer.WriteLine("시간 : " + utcConverted.ToString("yy/MM/dd HH:mm:ss") + "%" + "제목 : " + head + "%" + "내용 : " + body);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        writer.WriteLine(lines[i]);
                    }
                }

            }
            catch (Exception)
            {
            }



            Log.Debug(TAG, "Notification Message Body: " + body);
            SendNotification(head, body, message.Data);
        }

        void SendNotification(string messageHead, string messageBody, IDictionary<string, string> data)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            foreach (var key in data.Keys)
            {
                intent.PutExtra(key, data[key]);
            }

            //알림 내역 intent
            var pendingIntent = PendingIntent.GetActivity(this, MainActivity.NOTIFICATION_ID, intent, PendingIntentFlags.OneShot);



            PendingIntent helpPendingIntent = PendingIntent.GetBroadcast(
               this,
               REQUEST_CODE_HELP,
               new Intent(this, typeof(NotificationReceiver))
               .PutExtra(KEY_INTENT_HELP, REQUEST_CODE_HELP),
               PendingIntentFlags.OneShot
             );

            Android.Support.V4.App.RemoteInput remoteInput = new Android.Support.V4.App.RemoteInput.Builder(NOTIFICATION_REPLY)
               .SetLabel("Please enter your name")
               .Build();


            //NotificationCompat.Action action =
            //    new NotificationCompat.Action.Builder(Resource.Drawable.abc_edit_text_material,
            //            "Reply Now...", helpPendingIntent)
            //            .AddRemoteInput(remoteInput)
            //            .Build();


            var notificationBuilder = new NotificationCompat.Builder(this, MainActivity.CHANNEL_ID)
                                      .SetSmallIcon(Resource.Drawable.ic_stat_ic_notification)
                                      .SetContentTitle(messageHead)
                                      .SetContentText(messageBody)
                                      .SetAutoCancel(true)
                                      .SetColor(Color.HoloRedDark)
                                      .SetVibrate(new long[] { 100, 200, 300, 400, 500, 400, 300, 200, 400 })
                                      .SetContentIntent(pendingIntent)
                                      .AddAction(Resource.Drawable.abc_edit_text_material, GetString(Resource.String.finish), helpPendingIntent)
                                      .AddAction(Resource.Drawable.abc_edit_text_material, GetString(Resource.String.ok), pendingIntent);

            var notificationManager = NotificationManagerCompat.From(this);
            int not_nu = generateRandom();
            //notificationManager.Notify(MainActivity.NOTIFICATION_ID, notificationBuilder.Build());
            notificationManager.Notify(not_nu, notificationBuilder.Build());
        }
        public int generateRandom()
        {
            Random random = new Random();
            return random.Next(9999 - 1000) + 1000;
        }


        static DateTime ConvertFromUnixTimestamp(long timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 9, 0, 0, 0);
            return origin.AddSeconds(Convert.ToDouble(timestamp));

        }
    }
}

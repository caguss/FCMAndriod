using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace FCMNotifications
{
    class NotificationReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Bundle remoteInput = Android.Support.V4.App.RemoteInput.GetResultsFromIntent(intent);

            //if there is some input
            if (remoteInput != null)
            {

                //getting the input value
                string name = remoteInput.GetCharSequence(MyFirebaseMessagingService.NOTIFICATION_REPLY);

                //updating the notification with the input value
                NotificationCompat.Builder mBuilder = new NotificationCompat.Builder(context, MyFirebaseMessagingService.CHANNNEL_ID)
                        .SetSmallIcon(Resource.Drawable.abc_edit_text_material)
                        .SetContentTitle("Hey Thanks, " + name);
                NotificationManager notificationManager = (NotificationManager)context.
                        GetSystemService(Context.NotificationService);
                notificationManager.Notify(MainActivity.NOTIFICATION_ID, mBuilder.Build());
            }

            //if help button is clicked
            if (intent.GetIntExtra(MyFirebaseMessagingService.KEY_INTENT_HELP, -1) == MyFirebaseMessagingService.REQUEST_CODE_HELP)
            {
                Toast.MakeText(context, "You Clicked Help", ToastLength.Long).Show();
            }

            //if more button is clicked 
            if (intent.GetIntExtra(MyFirebaseMessagingService.KEY_INTENT_MORE, -1) == MyFirebaseMessagingService.REQUEST_CODE_MORE)
            {
                Toast.MakeText(context, "You Clicked More", ToastLength.Long).Show();
            }
        }
    }
}
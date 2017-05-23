using System;
using System.Collections.Generic;

using Android.App;
using Android.Views;
using Android.Widget;
using Java.Util;
using Java.Text;

namespace Firends_Party
{
    class CustomListViewAdapter : BaseAdapter<Event>
    {

        List<Event> eventList;
        Activity context;

        public CustomListViewAdapter(Activity context, List<Event> eventList) : base()
        {
            this.context = context;
            this.eventList = eventList;
        }


        public override long GetItemId(int position)
        {
            return position;
        }


        public override Event this[int position]
        {
            get { return eventList[position]; }
        }


        public override int Count
        {
            get { return eventList.Count; }
        }


        public override View GetView(int position, View convertView, ViewGroup parent)
        {

            // Etape 1 : On utilise le LayoutInflater pour inclure le layout EventListRow
            View view = convertView;
            if (view == null)
                view = context.LayoutInflater.Inflate(Resource.Layout.EventListRow, null);

            // Etape 2 : On récupère les références des TextViews
            TextView eventName = view.FindViewById<TextView>(Resource.Id.eventName);
            TextView dateName = view.FindViewById<TextView>(Resource.Id.eventDate);

            // Etape 3 : On attribue les valeurs texte à chaque TextView d'un EventListRow
            eventName.Text = eventList[position].Name;
            dateName.Text = GetDate(eventList[position].DateTimestamp);

            return view;
        }

        private String GetDate(long date)
        {
            return DateFormat.GetDateTimeInstance(DateFormat.Short, DateFormat.Short, Locale.Default).Format(date * 1000L);
        }

    }
}
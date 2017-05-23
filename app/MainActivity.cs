using Android.App;
using Android.Widget;
using Android.OS;
using Android.Util;
using Firebase.Auth;
using Android.Views;
using Java.Util;
using System.Collections.Generic;
using Firebase.Database;
using System;
using Android.Content;

namespace Firends_Party
{
    [Activity(Label = "Firends_Party")]
    public class MainActivity : Activity, IChildEventListener
    {

        protected Button addEvent;

        private CustomListViewAdapter upcomingEventsAdapter, pastEventsAdapter;
        private List<Event> upcomingEventList = new List<Event>();
        private List<Event> pastEventList = new List<Event>();
        protected ListView upcomingEvents, pastEvents;

        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        ///     Méthode issue de l'interface IChildEventListener : Gérer les erreur de traitement
        /// </summary>
        /// <see cref="IChildEventListener"/>
        /// <param name="error"></param>
        public void OnCancelled(DatabaseError error) { }

        /// <summary>
        ///     Méthode issue de l'interface IChildEventListener : Gérer les objets modifiés
        /// </summary>
        /// <see cref="IChildEventListener"/>
        /// <param name="error"></param>
        public void OnChildChanged(DataSnapshot snapshot, string previousChildName) { }

        /// <summary>
        ///     Méthode issue de l'interface IChildEventListener : Gérer les objets déplacés
        /// </summary>
        /// <see cref="IChildEventListener"/>
        /// <param name="error"></param>
        public void OnChildMoved(DataSnapshot snapshot, string previousChildName) { }

        /// <summary>
        ///     Méthode issue de l'interface IChildEventListener : Gérer les objets supprimés
        /// </summary>
        /// <see cref="IChildEventListener"/>
        /// <param name="error"></param>
        public void OnChildRemoved(DataSnapshot snapshot) { }

        /// <summary>
        ///     Méthode issue de l'interface IChildEventListener : Gérer les objets ajoutés
        /// </summary>
        /// <see cref="IChildEventListener"/>
        /// <param name="error"></param>
        public void OnChildAdded(DataSnapshot snapshot, string previousChildName)
        {
            Log.Debug("Valeurs", "Firebase : " + snapshot.Child("name").Value);
            if ((long)snapshot.Child("dateTimestamp").Value * 1000L < (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds)
            {
                // On crée un objet Event
                Event eventToAdd = new Event();
                eventToAdd.Name = Convert.ToString(snapshot.Child("name").Value);
                eventToAdd.User = Convert.ToString(snapshot.Child("user").Value);
                eventToAdd.DateTimestamp = Convert.ToInt64(snapshot.Child("dateTimestamp").Value);
                eventToAdd.Latitude = Convert.ToDouble(snapshot.Child("latitude").Value);
                eventToAdd.Longitude = Convert.ToDouble(snapshot.Child("longitude").Value);

                Log.Debug("Objet", "Valeurs : " + eventToAdd.ToString());

                pastEventList.Add(eventToAdd); // On ajoute l'événement dans la liste des événements passés...
                pastEventsAdapter.NotifyDataSetChanged(); // ... puis on notifie à l'adaptateur les changements
            }
            else // Sinon, cela veut dire que l'événement n'est pas encore passé !
            {
                // On crée un objet Event
                Event eventToAdd = new Event();
                eventToAdd.Name = Convert.ToString(snapshot.Child("name").Value);
                eventToAdd.User = Convert.ToString(snapshot.Child("user").Value);
                eventToAdd.DateTimestamp = Convert.ToInt64(snapshot.Child("dateTimestamp").Value);
                eventToAdd.Latitude = Convert.ToDouble(snapshot.Child("latitude").Value);
                eventToAdd.Longitude = Convert.ToDouble(snapshot.Child("longitude").Value);

                Log.Debug("Objet", "Valeurs : " + eventToAdd.ToString());

                upcomingEventList.Add(eventToAdd); // On ajoute l'événement dans la liste des événements à venir...
                upcomingEventsAdapter.NotifyDataSetChanged(); // ... puis on notifie à l'adaptateur les changements
            }
        }


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Log.Debug("Firebase", "Valeurs : " + FirebaseAuth.Instance.CurrentUser.DisplayName);
            SetContentView(Resource.Layout.Main);
            InitializeButtons();
            InitializeListViews();
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_disconnect:
                    FirebaseAuth.Instance.SignOut();
                    StartActivity(typeof(LoginActivity));
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void InitializeButtons()
        {
            addEvent = FindViewById<Button>(Resource.Id.add_event);
            addEvent.Click += delegate { StartActivity(typeof(AddPartyActivity)); };
        }

        /// <summary>
        ///     Méthode d'initialisation des ListView
        /// </summary>
        private void InitializeListViews()
        {
            upcomingEvents = FindViewById<ListView>(Resource.Id.upcoming_events);
            pastEvents = FindViewById<ListView>(Resource.Id.past_events);


            // On crée l'adapter par rapport aux données présentes dans la liste
            upcomingEventsAdapter = new CustomListViewAdapter(this, upcomingEventList);
            // On attache l'adapter
            upcomingEvents.Adapter = upcomingEventsAdapter;
            upcomingEvents.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                Intent eventPreview = new Intent(this, typeof(EventPreviewActivity));
                eventPreview.PutExtra("eventName", upcomingEventList[e.Position].Name);
                eventPreview.PutExtra("username", upcomingEventList[e.Position].User);
                eventPreview.PutExtra("date", upcomingEventList[e.Position].DateTimestamp);
                eventPreview.PutExtra("latitude", upcomingEventList[e.Position].Latitude);
                eventPreview.PutExtra("longitude", upcomingEventList[e.Position].Longitude);
                StartActivity(eventPreview);
            };

            // On crée l'adapter par rapport aux données présentes dans la liste
            pastEventsAdapter = new CustomListViewAdapter(this, pastEventList);
            // On attache l'adapter
            pastEvents.Adapter = pastEventsAdapter;
            pastEvents.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                Intent eventPreview = new Intent(this, typeof(EventPreviewActivity));
                eventPreview.PutExtra("eventName", pastEventList[e.Position].Name);
                eventPreview.PutExtra("username", pastEventList[e.Position].User);
                eventPreview.PutExtra("date", pastEventList[e.Position].DateTimestamp);
                eventPreview.PutExtra("latitude", pastEventList[e.Position].Latitude);
                eventPreview.PutExtra("longitude", pastEventList[e.Position].Longitude);
                StartActivity(eventPreview);
            };

            PrepareEventListDatas();

        }

        /// <summary>
        ///     Méthode pour récupérer la référence à la base de données Firebase
        /// </summary>
        private void PrepareEventListDatas()
        {
            Query firebaseRef =  FirebaseDatabase.Instance.Reference.Child("events"); // On récupère la référence
            firebaseRef.AddChildEventListener(this); // On attache l'écouteur d'événement présent dans l'activité
        }

    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps;
using Java.Util;
using Java.Text;
using Firebase.Database;
using Firebase.Auth;
using Android.Gms.Maps.Model;

namespace Firends_Party
{
    [Activity(Label = "EventPreviewActivity")]
    public class EventPreviewActivity : Activity, IOnMapReadyCallback, IChildEventListener
    {

        private GoogleMap _map;
        private MapFragment _mapFragment;
        protected TextView eventName, userName, eventDate;
        protected Button comeToEvent, notComeToEvent;

        protected ListView participantListView;
        protected ArrayAdapter<String> participantArrayAdapter;
        private List<string> participantArrayList = new List<string>();
        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.EventPreview);

            InitializeMap();
            InitializeTextViews();
            InitializeListView();
            InitializeButton();

        }

        /// <summary>
        ///     Méthode héritée de IOnMapReadyCallback permettant de signaler à l'activité que la map est prête à être utilisée
        /// </summary>
        /// <see cref="IOnMapReadyCallback"/>
        /// <param name="map"></param>
        public void OnMapReady(GoogleMap map)
        {
            _map = map;
            MarkerOptions markerOpt1 = new MarkerOptions();
            markerOpt1.SetPosition(new LatLng(Intent.GetDoubleExtra("latitude", -34), Intent.GetDoubleExtra("longitude", 151)));
            markerOpt1.SetTitle(Intent.GetStringExtra("eventName"));
            _map.AddMarker(markerOpt1);
            _map.MoveCamera(CameraUpdateFactory.NewLatLng(new LatLng(Intent.GetDoubleExtra("latitude", -34), Intent.GetDoubleExtra("longitude", 151))));
        }

        /// <summary>
        ///     Méthode héritée de IChildListener : Gére l'annulation d'une requête sur la base de données
        /// </summary>
        /// <see cref="IChildEventListener"/>
        /// <param name="snapshot"></param>
        public void OnCancelled(DatabaseError error){ }

        /// <summary>
        ///     Méthode héritée de IChildListener : Gére la mise à jour d'une ligne au sein la base de données
        /// </summary>
        /// <see cref="IChildEventListener"/>
        /// <param name="snapshot"></param>
        public void OnChildChanged(DataSnapshot snapshot, string previousChildName)
        {
            if (snapshot.Value.ToString().Equals(Convert.ToString(0)))
            { // On teste si la valeur de participation est égale à 0
                participantArrayList.Remove(snapshot.Key); // On enlève en temps réel les données d'un participant qui ne souhaiterait plus participer ...
                participantArrayAdapter.NotifyDataSetChanged(); // ... et on notifie à l'adaptateur que la liste des participants a changé !!
            }
            else if (snapshot.Value.ToString().Equals(Convert.ToString(1)))
            { // Sinon, on teste si la valeur de participation est égale à 1
                participantArrayList.Add(snapshot.Key); // On ajoute les données à la liste des participant ...
                participantArrayAdapter.NotifyDataSetChanged(); // ... et on notifie à l'adaptateur que la liste des participants a changé !!
            }
        }

        /// <summary>
        ///     Méthode héritée de IChildListener : Gére le déplacement d'une ligne au sein la base de données
        /// </summary>
        /// <see cref="IChildEventListener"/>
        /// <param name="snapshot"></param>
        public void OnChildMoved(DataSnapshot snapshot, string previousChildName){ }

        /// <summary>
        ///     Méthode héritée de IChildListener : Gére l'ajout d'une ligne à la base de données
        /// </summary>
        /// <see cref="IChildEventListener"/>
        /// <param name="snapshot"></param>
        public void OnChildAdded(DataSnapshot snapshot, string previousChildName)
        {
            if (snapshot.Exists())
            { // On vérifie bien dans un premier temps que les données existent
                if (snapshot.Value.ToString().Equals(Convert.ToString(1)))
                { // On teste si la valeur de participation est égale à 1
                    participantArrayList.Add(snapshot.Key); // On ajoute les données à la liste des participant ...
                    participantArrayAdapter.NotifyDataSetChanged(); // ... et on notifie à l'adaptateur que la liste des participants a changé !!
                }
            }
        }

        /// <summary>
        ///     Méthode héritée de IChildListener : Gère la suppression d'une ligne en base de données
        /// </summary>
        /// <see cref="IChildEventListener"/>
        /// <param name="snapshot"></param>
        public void OnChildRemoved(DataSnapshot snapshot){ }

        /// <summary>
        ///     Méthode permettant d'initialiser la Map
        /// </summary>
        private void InitializeMap()
        {
            _mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;
            if (_mapFragment == null)
            {
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeNormal)
                    .InvokeZoomControlsEnabled(false)
                    .InvokeCompassEnabled(true);

                FragmentTransaction fragTx = FragmentManager.BeginTransaction();
                _mapFragment = MapFragment.NewInstance(mapOptions);
                fragTx.Add(Resource.Id.map, _mapFragment, "map");
                fragTx.Commit();
            }
            _mapFragment.GetMapAsync(this);
        }

        /// <summary>
        ///     Méthode d'initialisation des TextViews
        /// </summary>
        private void InitializeTextViews()
        {
            // Etape 1 : On récupère la référence des vues via la classe R
            eventName = FindViewById<TextView>(Resource.Id.event_name);
            userName = FindViewById<TextView>(Resource.Id.event_username);
            eventDate = FindViewById<TextView>(Resource.Id.event_date);

            // Etape 2 : On ajoute les données récupérer grâce à l'Intent
            eventName.Text = Intent.GetStringExtra("eventName");
            userName.Text = Intent.GetStringExtra("username");

            // Etape 3 : Cas particulier. Comme on a enregistré les données de la date sous forme de Timestamp,
            //              on va dans un premier temps créer une instance de Calendar, qui se verra attribuer
            //              la valeur présent dans l'Intent.
            //           On formate enfin la date avec DateFormat
            Calendar dateCalendar = Calendar.Instance;
            dateCalendar.TimeInMillis = Intent.GetLongExtra("dateTimestamp", 0);
            eventDate.Text = DateFormat.GetDateTimeInstance(DateFormat.Short, DateFormat.Short, Locale.Default).Format(new Date(Intent.GetLongExtra("date", 0) * 1000L));
        }

        /// <summary>
        ///     Méthode d'initialisation des ListView
        /// </summary>
        private void InitializeListView()
        {
            // Etape 1 : On récupère la référence du composant via la classe R
            participantListView = FindViewById<ListView>(Resource.Id.participant_list);
            // Etape 2 : On crée l'adaptateur : Ici, on créera un adaptateur simple à partir d'une liste de chaine de caractère
            participantArrayAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, participantArrayList);
            // Etape 3 : On attache l'adaptateur à la vue
            participantListView.Adapter = participantArrayAdapter;
            // Etape 4 : On finit par préparer la liste des données à insérer dans la liste de vues
            PrepareParticipantDatas();
        }
            
        /// <summary>
        ///     Méthode d'initialisation des Button
        /// </summary>
        private void InitializeButton()
        {
            // Récupération des références
            comeToEvent = FindViewById<Button>(Resource.Id.come_to_event);
            notComeToEvent =FindViewById<Button>(Resource.Id.not_come_to_event);

            // Gestion du clic sur le bouton de participation à un événement
            comeToEvent.Click += delegate
            {
                FirebaseDatabase.Instance.Reference.Child("events").Child(Convert.ToString(Intent.GetLongExtra("date", 0))).Child("participant").Child(FirebaseAuth.Instance.CurrentUser.DisplayName).SetValue(1);
            };

            // Gestion du clic sur le bouton de non participation à un événement
            notComeToEvent.Click += delegate
            {
                FirebaseDatabase.Instance.Reference.Child("events").Child(Convert.ToString(Intent.GetLongExtra("date", 0))).Child("participant").Child(FirebaseAuth.Instance.CurrentUser.DisplayName).SetValue(0);
            };

            // Ici, on précise que si l'événement est passé, on n'affiche plus les boutons de participation !
            if(Intent.GetLongExtra("date", 0) * 1000L < (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds)
            {
                comeToEvent.Visibility = ViewStates.Gone;
                notComeToEvent.Visibility = ViewStates.Gone;
            }

        }

        /// <summary>
        ///     Méthode pour récupérer les données via la base de données Firebase
        /// </summary>
        private void PrepareParticipantDatas()
        {
            Query firebaseRef = FirebaseDatabase.Instance.Reference.Child("events").Child(Convert.ToString(Intent.GetLongExtra("date", 0))).Child("participant");
            firebaseRef.AddChildEventListener(this);
        }

    }
}
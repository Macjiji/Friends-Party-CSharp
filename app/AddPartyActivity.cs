using System;

using Android.App;
using Android.OS;
using Android.Widget;
using Java.Util;
using Android.Gms.Location.Places.UI;
using Firebase.Auth;
using Firebase.Database;
using Android.Gms.Location.Places;
using Android.Gms.Common.Apis;
using Android.Util;
using static Android.App.DatePickerDialog;
using static Android.App.TimePickerDialog;

namespace Firends_Party
{
    [Activity(Label = "AddPartyActivity")]
    public class AddPartyActivity : Activity, IPlaceSelectionListener, IOnDateSetListener, IOnTimeSetListener
    {

        private Calendar savedCalendar;

        protected Button addEvent;
        protected EditText eventName;
        protected ImageButton editDate, editTime;
        protected TextView previewDate, previewTime;
        protected PlaceAutocompleteFragment autocompleteFragment;

        private Event eventToSave = new Event();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddParty);

            savedCalendar = Calendar.Instance;

            InitializeButtons();
            InitializeEditText();
            InitializeAutoCompleteFragment();
            InitializeTextViews();
            InitializeImageButtons();
        }

        /// <summary>
        ///     Méthode héritée de l'interface IPlaceSelectionListener.
        /// </summary>
        /// <see cref="IPlaceSelectionListener"/>
        /// <param name="place"></param>
        public void OnPlaceSelected(IPlace place)
        {
            eventToSave.Latitude = place.LatLng.Latitude;
            eventToSave.Longitude = place.LatLng.Longitude;
        }

        /// <summary>
        ///     Méthode héritée de l'interface IPlaceSelectionListener.
        /// </summary>
        /// <see cref="IPlaceSelectionListener"/>
        /// <param name="status"></param>
        public void OnError(Statuses status)
        {
            Log.Info("Maps", "An error occurred: " + status);
        }

        /// <summary>
        ///     Méthode permettant de récupérer les informations dans la boite de dialogue DatePickerDialog en vue d'attribuer les valeurs à l'événement. C'est un écouteur d'événement !
        /// </summary>
        /// <see cref="IOnDateSetListener"/>
        /// <param name="view">La vue du DatePicker</param>
        /// <param name="year">L'année récupérée</param>
        /// <param name="monthOfYear">Le mois récupéré</param>
        /// <param name="dayOfMonth">Le jour du mois récupéré</param>
        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            savedCalendar.Set(Calendar.Year, year); // Attribution de l'année au Calendar à sauvegardé
            savedCalendar.Set(Calendar.Month, monthOfYear); // Attribution du mois au Calendar à sauvegardé
            savedCalendar.Set(Calendar.DayOfMonth, dayOfMonth); // Attribution du jour du mois au Calendar à sauvegardé
            previewDate.Text = dayOfMonth + "/" + (monthOfYear + 1) + "/" + year; // Affichage de la date dans l'aperçu texte
        }

        /// <summary>
        ///     Méthode permettant de récupérer les informations renseignées par l'utilisateur dans la boite de dialogue d'édition de l'heure !
        /// </summary>
        /// <see cref="IOnTimeSetListener"/>
        /// <param name="timePicker"></param>
        /// <param name="selectedHour"></param>
        /// <param name="selectedMinute"></param>
        public void OnTimeSet(TimePicker timePicker, int selectedHour, int selectedMinute)
        {
            savedCalendar.Set(Calendar.HourOfDay, selectedHour); // Attribution de l'heure au Calendar à sauvegardé
            savedCalendar.Set(Calendar.Minute, selectedMinute); // Attribution des minutes au Calendar à sauvegardé
            previewTime.Text = selectedHour + ":" + selectedMinute; // Affichage de l'heure dans l'aperçu texte
        }

        /// <summary>
        ///     Méthode permettant d'initialiser les Buttons
        /// </summary>
        private void InitializeButtons()
        {
            addEvent = FindViewById<Button>(Resource.Id.button_validate);
            addEvent.Click += async delegate
            {
                // Etape 1 : On récupère le reste des valeurs à attribuer à l'événement.
                //      Les valeurs de latitude et de longitude sont directement attribuées
                //      via la Listener de l'AutoCompleteFragment.
                eventToSave.Name = eventName.Text.ToString();
                eventToSave.User = FirebaseAuth.Instance.CurrentUser.DisplayName;
                eventToSave.DateTimestamp = savedCalendar.TimeInMillis / 1000L;

                // Etape 2 : On sauvegarde les données de l'événement dans la base de données Firebase.
                await FirebaseDatabase.Instance.Reference.Child("events").Child(Convert.ToString(eventToSave.DateTimestamp)).Child("name").SetValueAsync(eventToSave.Name);
                await FirebaseDatabase.Instance.Reference.Child("events").Child(Convert.ToString(eventToSave.DateTimestamp)).Child("user").SetValueAsync(eventToSave.User);
                await FirebaseDatabase.Instance.Reference.Child("events").Child(Convert.ToString(eventToSave.DateTimestamp)).Child("dateTimestamp").SetValueAsync(eventToSave.DateTimestamp);
                await FirebaseDatabase.Instance.Reference.Child("events").Child(Convert.ToString(eventToSave.DateTimestamp)).Child("latitude").SetValueAsync(eventToSave.Latitude);
                await FirebaseDatabase.Instance.Reference.Child("events").Child(Convert.ToString(eventToSave.DateTimestamp)).Child("longitude").SetValueAsync(eventToSave.Longitude);           
                StartActivity(typeof(MainActivity)); // On repars vers l'accueil une fois que les données ont bien été enregistrées.
            };
        }

        /// <summary>
        ///     Méthode permettant d'initialiser les EditText
        /// </summary>
        private void InitializeEditText()
        {
            eventName = FindViewById<EditText>(Resource.Id.event_name);
        }

        /// <summary>
        ///     Méthode permettant d'initialiser le fragment d'auto-completion de l'adresse
        /// </summary>
        private void InitializeAutoCompleteFragment()
        {
            //Etape 1 : On récupère la référence du fragment AutoCOmplete.
            autocompleteFragment = (PlaceAutocompleteFragment)FragmentManager.FindFragmentById(Resource.Id.place_autocomplete_fragment);
            // Etape 2 : On lui attribue un Listener pour récupérer la latitude et la longitude d'un lieu renseigné.
            autocompleteFragment.SetOnPlaceSelectedListener(this);
        }

        /// <summary>
        ///     Méthode permettant d'initialiser les TextViews
        /// </summary>
        private void InitializeTextViews()
        {
            previewDate = FindViewById<TextView>(Resource.Id.event_date_preview);
            previewTime = FindViewById<TextView>(Resource.Id.event_time_preview);
        }

        /// <summary>
        ///     Méthode permettant d'initialiser les Images Buttons
        /// </summary>
        private void InitializeImageButtons()
        {
            // Etape 1 : On récupère les références des vues via la classe R
            editDate = FindViewById<ImageButton>(Resource.Id.event_date_button);
            editTime = FindViewById<ImageButton>(Resource.Id.event_time_button);

            // Etape 2 : Gestion du clic sur le bouton d'édition de la date
            editDate.Click += delegate
            {
                // On récupère au préalable la date et l'heure actuelle
                Calendar c = Calendar.Instance;
                int mYear = c.Get(Calendar.Year);
                int mMonth = c.Get(Calendar.Month);
                int mDay = c.Get(Calendar.DayOfMonth);

                // Puis on crée la boite de dialogue contenant un DatePicker.
                new DatePickerDialog(this, this, mYear, mMonth, mDay).Show();
            };

            // Etape 3 : Gestion du clic sur le bouton d'édition de l'heure
            editTime.Click += delegate
            {
                // Pareil que pour la date mais avec l'heure et les minutes
                Calendar mCurrentTime = Calendar.Instance;
                int mHour = mCurrentTime.Get(Calendar.HourOfDay);
                int mMinute = mCurrentTime.Get(Calendar.Minute);

                // Et on crée la boite de dialogue contenant le TimePicker
                // NB : Le true à la fin du constructeur signifie que les heures apparaitront sous le format 24H.
                //      Il suffira de mettre false pour avoir une base de 12H avec "AM" et "PM"
                new TimePickerDialog(this, this, mHour, mMinute, true).Show();

            };

        }

    }
}
using System;

using Android.App;
using Android.OS;
using Android.Widget;
using Firebase.Auth;

namespace Firends_Party
{
    [Activity(Label = "RegisterActivity")]
    public class RegisterActivity : Activity
    {

        protected EditText userName, userMail, userPassword;
        protected Button register;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Register);

            InitializeButtons();
            InitializeEditText();
        }

        /// <summary>
        ///     Méthode d'initialisation des boutons
        /// </summary>
        private void InitializeButtons()
        {
            register = FindViewById<Button>(Resource.Id.button_register);
            register.Click += async delegate
            {
                try
                {
                    await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(userMail.Text.ToString(), userPassword.Text.ToString());
                    var profileUpdates = new UserProfileChangeRequest.Builder()
                        .SetDisplayName(userName.Text.ToString())
                        .Build();

                    try
                    {
                        await FirebaseAuth.Instance.CurrentUser.UpdateProfileAsync(profileUpdates);
                        StartActivity(typeof(MainActivity));
                    }
                    catch (Exception ex)
                    {
                        // Failed to update user profile
                    }
                }
                catch
                {
                    Toast.MakeText(this, "Authentication failed.", ToastLength.Short).Show();
                }

            };
        }

        private void InitializeEditText()
        {
            userName = FindViewById<EditText>(Resource.Id.username);
            userMail = FindViewById<EditText>(Resource.Id.email);
            userPassword = FindViewById<EditText>(Resource.Id.password);
        }

    }
}
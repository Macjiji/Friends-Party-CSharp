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

namespace Firends_Party
{
    class Event
    {

        private string name;
        private string user;
        private long dateTimestamp;
        private double latitude;
        private double longitude;

        public Event() { }

        public Event(string name, string username, long dateTimestamp, double latitude, double longitude)
        {
            this.name = name;
            this.user = username;
            this.dateTimestamp = dateTimestamp;
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string User
        {
            get { return this.user; }
            set { this.user = value; }
        }

        public long DateTimestamp
        {
            get { return this.dateTimestamp; }
            set { this.dateTimestamp = value; }
        }

        public double Latitude
        {
            get { return this.latitude; }
            set { this.latitude = value; }
        }

        public double Longitude
        {
            get { return this.longitude; }
            set { this.longitude = value; }
        }

        public override string ToString()
        {
            return "Evenement { Nom : " + this.name
                        + ", Utilisateur : " + this.user
                        + ", Date : " + this.dateTimestamp
                        + ", Latitude : " + this.latitude
                        + ", Longitude : " + this.longitude + "}";
        }

    }
}
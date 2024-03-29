﻿using Plugin.Media;
using PM2EXAMEN1_5905.Models;
using PM2EXAMEN1_5905.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM2EXAMEN1_5905
{
    public partial class MainPage : ContentPage
    {
        Plugin.Media.Abstractions.MediaFile FileFoto = null;
        public MainPage()
        {
            InitializeComponent();
            if (App.DBase == null)
            {

            }
        }
        private Byte[] ConvertImageToByteArray()
        {
            if (FileFoto != null)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    Stream stream = FileFoto.GetStream();
                    stream.CopyTo(memory);
                    return memory.ToArray();

                }
            }
            return null;
        }

        private async void Foto_Clicked(object sender, EventArgs e)
        {
            try
            {
                FileFoto = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "FotosExamen",
                    Name = "lugar.jpg",
                    SaveToAlbum = true
                });

                if (FileFoto != null)
                {
                    Foto.Source = ImageSource.FromStream(() =>
                    {
                        return FileFoto.GetStream();
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message.ToString(), "Ok");
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            currentLocation();
        }

        private async void currentLocation()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (status == PermissionStatus.Granted)
                {
                    var localizacion = await Geolocation.GetLocationAsync();
                    if (localizacion != null)
                    {
                        txtLatitud.Text = Convert.ToString(localizacion.Latitude);
                        txtLongitud.Text = Convert.ToString(localizacion.Longitude);
                    }
                    else
                    {
                        await DisplayAlert("Advertencia", "No pudimos obtener su ubicacion", "Ok");
                        txtLatitud.Text = Convert.ToString(14.079700596864964);
                        txtLongitud.Text = Convert.ToString(-87.18946052765942);
                    }

                }
                else
                {
                    await DisplayAlert("Advertencia", "Active el GPS para el correcto funcionamiento de la aplicación.", "Ok");
                    await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }
            }
            catch
            {

            }


        }

        private async void btnGuardar_Clicked(object sender, EventArgs e)
        {
            if (FileFoto == null)
            {
                await DisplayAlert("Error", "No se ha tomado una fotografia", "OK");
                return;
            }


            if (string.IsNullOrEmpty(txtDescripcion.Text))
            {
                await DisplayAlert("Error", "No se ha ingresado la descripcion", "OK");
                return;
            }

            if (string.IsNullOrEmpty(txtLatitud.Text))
            {
                await DisplayAlert("Error", "No se ha ingresado la latitud", "OK");
                return;
            }

            if (string.IsNullOrEmpty(txtLongitud.Text))
            {
                await DisplayAlert("Error", "No se ha ingresado la longitud", "OK");
                return;
            }

            var sitio = new Sitios
            {
                id = 0,
                foto = ConvertImageToByteArray(),
                fotoPath = FileFoto.Path,
                latitud = Convert.ToDouble(txtLatitud.Text),
                longitud = Convert.ToDouble(txtLongitud.Text),
                descripcion = txtDescripcion.Text
            };

            var result = await App.DBase.SiteSave(sitio);


            if (result > 0)
            {
                await DisplayAlert("Alert", "Guardado Correctamente", "OK");
                txtDescripcion.Text = "";
            }
            else
            {
                await DisplayAlert("Alert", "Ha ocurrido un error", "OK");
            }
        }

        private async void btnLista_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListSites());
        }

        private void btnExit_Clicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
        }
    }
}

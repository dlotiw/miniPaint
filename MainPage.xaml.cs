using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace miniPaint
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Point punktPoczatkowy;
        SolidColorBrush kolor = new SolidColorBrush(Windows.UI.Colors.Black);
        SolidColorBrush pedzel;
        bool czyRysuje = false;
        Line linia;
        UIElement usun_poprzednie = null;
        Stack<UIElement> listaUndo = new Stack<UIElement>();
        ContentDialog wiadomosc;
        public MainPage()
        {
            this.InitializeComponent();
            slider_value.Text = thickness.Value.ToString();

        }

        private void rysowanieStart(object sender, PointerRoutedEventArgs e)
        {
            czyRysuje = true;
            punktPoczatkowy = e.GetCurrentPoint(poleRysowania).Position;
            usun_poprzednie = null;

        }

        private void rysowanieKoniec(object sender, PointerRoutedEventArgs e)
        {
            czyRysuje = false;
            if (rdbProsta.IsChecked == true)
            {
                listaUndo.Push(linia);
            }
        }
        private void Rysowanie(object sender, PointerRoutedEventArgs e)
        {
            
            if (czyRysuje == true)
            {
                if (rdbProsta.IsChecked == true)
                {
                    if (usun_poprzednie != null)
                    {
                        poleRysowania.Children.Remove(usun_poprzednie);
                    }
                    RysujLinie(sender, e);
                    

                }
                if (rdbDowolna.IsChecked == true)
                {
                    RysujLinie(sender, e);
                    punktPoczatkowy = e.GetCurrentPoint(poleRysowania).Position;
                    listaUndo.Push(linia);
                }
            }

            

        }
        private void RysujLinie(object sender, PointerRoutedEventArgs e)
        {
            Point punktAktualny;
            
            punktAktualny = e.GetCurrentPoint(poleRysowania).Position;
            linia = new Line();
            pedzel = new SolidColorBrush();
            pedzel = kolor;
            linia.StrokeStartLineCap = PenLineCap.Round;
            linia.StrokeEndLineCap = PenLineCap.Round;
            linia.StrokeThickness = thickness.Value;
            linia.X1 = punktPoczatkowy.X;
            linia.Y1 = punktPoczatkowy.Y;
            linia.X2 = punktAktualny.X;
            linia.Y2 = punktAktualny.Y;
            linia.Stroke = pedzel;
            poleRysowania.Children.Add(linia);
            usun_poprzednie = linia;

            
        }


        private void Rectangle_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var kwadrat = (Windows.UI.Xaml.Shapes.Rectangle)e.OriginalSource;
            kolor = (SolidColorBrush)kwadrat.Fill;
            
        }

        private void undo_Click(object sender, RoutedEventArgs e)
        {
            if (listaUndo.Count > 0)
            {
                UIElement undo = listaUndo.Pop();
                poleRysowania.Children.Remove(undo);
            }
        }

        //Zmiania pola textBlock dla slider klawiatura
        private void thickness_PreviewKeyUp(object sender, KeyRoutedEventArgs e)
        {
            var slider = sender as Slider;
            double value = slider.Value;
            slider_value.Text = value.ToString();
        }
        //Zmiania pola textBlock dla slider myszka
        private void thickness_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var slider = sender as Slider;
            double value = slider.Value;
            slider_value.Text = value.ToString();
        }

        private async void koniec_Click(object sender, RoutedEventArgs e)
        {
            //Syntezator głosu(sygnał dźwiękowy)
            string messageBoxText = "Czy naprawdę chcesz opuścić program ?";
            MediaElement mediaElement = new MediaElement();
            var synth = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
            Windows.Media.SpeechSynthesis.SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync("Do widzenia");
            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();
            //Messagebox (wyskakujące okno)
            wiadomosc = new ContentDialog();
            wiadomosc.Title = messageBoxText;
            wiadomosc.PrimaryButtonText = "tak";
            wiadomosc.SecondaryButtonText = "nie";
            wiadomosc.PrimaryButtonClick += wiadomosc_tak_secondary_button_click;
            await wiadomosc.ShowAsync();
        }

        //Wyłączenie aplikacji przez wciśnięcie tak
        private void wiadomosc_tak_secondary_button_click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            CoreApplication.Exit();
        }
    }
}

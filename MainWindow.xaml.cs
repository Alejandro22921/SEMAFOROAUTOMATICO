using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Windows.Threading;

namespace SemaforoAutomatico
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort puerto = new SerialPort("COM1", 9600, Parity.None, 8);
        DispatcherTimer tiempo = new DispatcherTimer();
        int contador = 0;
        public MainWindow()
        {
            InitializeComponent();
            tiempo.Interval = TimeSpan.FromMilliseconds(1000);  /* --- CADA SEGUNDO --- */
            tiempo.Tick += new EventHandler(tiempo_Tick);
            
        }

        void tiempo_Tick(object sender, EventArgs e)
        {
            switch(contador)
            {
                /*
                 * puerto.Write("g"); PRENDE LUZ VERDE
                 * puerto.Write("y"); PRENDE LUZ AMARILLA
                 * puerto.Write("r"); PRENDE LUZ ROJA
                 * puerto.Write("b"); APAGA LUZ VERDE
                 * puerto.Write("c"); APAGA LUZ AMARILLA
                 * puerto.Write("d"); APAGA LUZ ROJA
                 * 
                 * */

                case 0:
                    ellipseVerde.Fill = Brushes.Green;
                    puerto.Write("g");
                    ellipseRojo.Fill = Brushes.Black;
                    puerto.Write("d");
                    break;
                case 2:
                    ellipseVerde.Fill = Brushes.Black;
                    puerto.Write("b");
                    break;
                case 3:
                    ellipseVerde.Fill = Brushes.Green;
                    puerto.Write("g");
                    break;
                case 4:
                    ellipseVerde.Fill = Brushes.Black;
                    puerto.Write("b");
                    break;
                case 5:
                    ellipseVerde.Fill = Brushes.Green;
                    puerto.Write("g");
                    break;
                case 6:
                    ellipseVerde.Fill = Brushes.Black;
                    puerto.Write("b");
                    ellipseAmarillo.Fill = Brushes.Yellow;
                    puerto.Write("y");
                    break;
                case 8:
                    ellipseAmarillo.Fill = Brushes.Black;
                    puerto.Write("c");
                    ellipseRojo.Fill = Brushes.Red;
                    puerto.Write("r");
                    break;
            }
            contador++;
            if (contador > 10)
            {
                contador = 0; /* --- SE REESTABLECE EL CONTADOR --- */
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (btnConectar.Content.ToString() == "CONECTAR")
            {
                puerto.Open();
                puerto.DataReceived += new SerialDataReceivedEventHandler(puerto_DataReceived);
                ellipseConectado.Fill = Brushes.OrangeRed;
                btnConectar.Content = "DESCONECTAR";
            }
            else
            {
                Apaga();
                contador = 0;
                puerto.Close();
                ellipseConectado.Fill = Brushes.Black;
                btnConectar.Content = "CONECTAR";
            }
        }

        delegate void ActualizaDatos();
        string datos = "";

        void puerto_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            datos = puerto.ReadExisting();
            Dispatcher.Invoke(new ActualizaDatos(actualiza));
        }

        void actualiza()
        {
            if (datos == "1")  /* --- SE CIERRA EL CIRCUITO --- */
            {
                tiempo.Start();
                contador = 0;
                ellipseConectado.Fill = Brushes.LightGreen;
            }
            else if (datos == "0")  /* --- SE ABRE EL CIRCUITO --- */
            {
                
                contador = 0;
                Apaga();
                ellipseConectado.Fill = Brushes.OrangeRed;
            }

            datos = "";
        }
        void Apaga()
        {
            tiempo.Stop();
            ellipseVerde.Fill = Brushes.Black;
            ellipseAmarillo.Fill = Brushes.Black;
            ellipseRojo.Fill = Brushes.Black;
            puerto.Write("b");
            puerto.Write("c");
            puerto.Write("d");
        }

    }
}

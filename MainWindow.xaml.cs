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
                    break;
                case 2:
                    ellipseVerde.Fill = Brushes.Black;
                    puerto.Write("b");
                    break;
            }
            contador++;
            if (contador > 3)
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
                tiempo.Stop();
                contador = 0;
                puerto.Write("b");
                ellipseVerde.Fill = Brushes.Black;
                ellipseConectado.Fill = Brushes.OrangeRed;
            }

            datos = "";
        }

    }
}

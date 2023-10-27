﻿using OfficeOpenXml;
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
using Newtonsoft.Json;
using System.IO;

namespace ProyectoGeneticoFinal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    enum TipoCruzamiento
    {
        TPX,
        OPX
    }

    enum TipoMutación
    {
        Swap,
        HSwap
    }

    public partial class MainWindow : Window
    {
        private Random rand = new Random();
        private List<(int, int)> coordenadas = new List<(int, int)>();
        private int cantidadPuntos = 15;
        private int cantPoblación;
        private int[,] distancias = new int[15, 15];      
        private int[,] Población = new int[1, 1];
        private int[,] Población2 = new int[1, 1];
        private int probCruzamiento;
        private int probMutación;
        private bool esPob1Actual = true;
        private int criterioParo = 100;
        private TipoCruzamiento cruzamiento;
        private TipoMutación mutación;
        private int[,] aptitudes = new int[32, 30];
        private int[,] tiempos = new int[32, 30];

        public MainWindow()
        {
            InitializeComponent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        private void btnEjecutar_Click(object sender, RoutedEventArgs e)
        {
            for(int opcionesPoblación = 500; opcionesPoblación <= 1000; opcionesPoblación+=500)
            {
                for(int opcionesCruzamiento = 0; opcionesCruzamiento <= 1; opcionesCruzamiento++)
                {
                    for (int opcionesProbCruzamiento = 80; opcionesProbCruzamiento <= 90; opcionesProbCruzamiento+=10)
                    {
                        for (int opcionesMutación = 0; opcionesMutación <= 1; opcionesMutación++)
                        {
                            for (int opcionesProbMutación = 20; opcionesProbMutación <= 30; opcionesProbMutación += 10)
                            {

                            }
                        }
                    }
                }
            }
        }

        private async Task<bool> LeerPuntos()
        {
            string nombreArchivo = "D:\\Documentos\\Visual Studio\\ProyectoGenetico\\ProyectoGenetico\\bin\\Debug\\net7.0-windows10.0.17763.0\\CoordenadasGuardadas.json";
            try
            {
                if (File.Exists(nombreArchivo))
                {
                    string json = await File.ReadAllTextAsync(nombreArchivo);
                    List<(int, int)>? listaCoordenadas = JsonConvert.DeserializeObject<List<(int, int)>>(json);

                    if (listaCoordenadas != null)
                    {
                        coordenadas = listaCoordenadas;
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool lecturaHecha = await LeerPuntos();
            if (lecturaHecha == false)
            {
                MessageBox.Show("No se ha encontrado una lista de ciudades", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                App.Current.Shutdown();
            }
        }
    }
}

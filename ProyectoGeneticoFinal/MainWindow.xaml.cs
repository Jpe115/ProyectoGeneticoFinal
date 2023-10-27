using OfficeOpenXml;
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
        private int mejorAptitud;
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

        #region Población
        private void InicializarPoblación(int[,] pob)
        {
            for (int a = 0; a < cantPoblación; a++)
            {
                for (int b = 0; b < cantidadPuntos; b++)
                {
                    pob[a, b] = b;
                }
                pob[a, 0] = 0;
            }
        }

        private void GenerarPobInicial(int[,] pob)
        {
            for (int a = 0; a < cantPoblación; a++)
            {
                for (int b = 1; b < cantidadPuntos; b++)
                {
                    int temp = pob[a, b];
                    int random = rand.Next(1, cantidadPuntos - 1);
                    pob[a, b] = pob[a, random];
                    pob[a, random] = temp;

                }
                pob[a, cantidadPuntos] = 0;
                pob[a, cantidadPuntos + 1] = 0;
            }
        }

        private void CalcularAptitud(int[,] pob)
        {
            for (int a = 0; a < cantPoblación; a++)
            {
                int aptitud = 0;
                for (int b = 1; b < cantidadPuntos; b++)
                {
                    aptitud += distancias[pob[a, b], pob[a, b + 1]];
                }
                pob[a, cantidadPuntos + 1] = aptitud;
            }
        }
        #endregion

        private void ProcesoSelección(int[,] pob, int[,] pobContraria)
        {
            for (int a = 0; a < cantPoblación; a++)
            {
                int r = rand.Next(0, cantPoblación - 1);
                if (pob[a, cantidadPuntos + 1] < pob[r, cantidadPuntos + 1])
                {
                    CopiarSolucion(a, a, pob, pobContraria);
                }
                else
                {
                    CopiarSolucion(r, a, pob, pobContraria);
                }
            }
        }

        #region Búsqueda de soluciones
        private void CopiarSolucion(int filaGanadora, int filaActual, int[,] pob, int[,] pobContraria)
        {
            for (int a = 0; a < cantidadPuntos + 2; a++)
            {
                pobContraria[filaActual, a] = pob[filaGanadora, a];
            }
        }

        private void BuscarMejorSolución(int[,] pob)
        {
            for (int fila = 0; fila < cantPoblación; fila++)
            {
                if (pob[fila, cantidadPuntos + 1] < mejorAptitud)
                {
                    mejorAptitud = pob[fila, cantidadPuntos + 1];
                }
            }
        }
        #endregion

        #region Cruzamiento
        private bool ProcesoCruzamiento(int[,] pobContraria, int[,] pob)
        {
            if (probCruzamiento >= 0 && probCruzamiento <= 100)
            {
                int prob = rand.Next(1, 100);
                if (prob <= probCruzamiento)
                {
                    if (cruzamiento == TipoCruzamiento.TPX)
                    {
                        int[] valoresS1yS2 = ObtenerS1yS2();
                        TwoPointCrossover(true, 1, valoresS1yS2, pob, pobContraria);
                        TwoPointCrossover(false, -1, valoresS1yS2, pob, pobContraria);
                        return true;
                    }
                    else
                    {
                        int S1 = rand.Next(3, cantidadPuntos - 3);
                        OnePointCrossover(true, 1, S1, pob, pobContraria);
                        OnePointCrossover(false, -1, S1, pob, pobContraria);
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void OnePointCrossover(bool esPar, int intercambio, int punto, int[,] pob, int[,] pobContraria)
        {
            int parImpar = esPar ? 0 : 1;

            //LLenar desde el padre 1 al hijo
            for (int a = parImpar; a < cantPoblación; a += 2)
            {
                //LLenar desde el padre 1 al hijo
                for (int b = 1; b <= punto; b++)
                {
                    //Pasar todos los elementos del padre 1 dentro de los rangos, al hijo
                    pob[a, b] = pobContraria[a, b];
                }

                //Verificar que no sea duplicado                
                int fila = a + intercambio;
                int columna = 1;

                for (int b = 2; b < cantidadPuntos; b++)
                {
                    if (!(b <= punto))
                    {
                        bool bandera = false;
                        while (bandera == false && columna < cantidadPuntos)
                        {
                            int valorActual = pobContraria[fila, columna];

                            if (!EsDuplicado(a, valorActual, punto, pobContraria))
                            {
                                pob[a, b] = valorActual;
                                bandera = true;
                            }
                            else
                            {
                                columna++;
                            }
                        }
                        columna++;
                    }
                }
            }
        }

        private bool EsDuplicado(int a, int valor, int punto, int[,] pobContraria)
        {
            for (int col = 1; col <= punto; col++)
            {
                if (pobContraria[a, col] != 0)
                {
                    if (pobContraria[a, col] == valor)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private int[] ObtenerS1yS2()
        {
            int S1 = rand.Next(1, cantidadPuntos - 3);
            int S2 = rand.Next(S1 + 1, cantidadPuntos);

            int[] valoresS1yS2 = { S1, S2 };
            return valoresS1yS2;
        }

        private void TwoPointCrossover(bool esPar, int intercambio, int[] valoresS1yS2, int[,] pob, int[,] pobContraria)
        {
            int parImpar = esPar ? 0 : 1;

            //LLenar desde el padre 1 al hijo
            for (int a = parImpar; a < cantPoblación; a += 2)
            {
                //LLenar desde el padre 1 al hijo
                for (int b = 1; b < cantidadPuntos + 2; b++)
                {
                    if (b <= valoresS1yS2[0] || b >= valoresS1yS2[1])
                    {
                        //Pasar todos los elementos del padre 1 dentro de los rangos, al hijo
                        pob[a, b] = pobContraria[a, b];
                    }
                }

                //Verificar que no sea duplicado                
                int fila = a + intercambio;
                int columna = 1;

                for (int b = 2; b < cantidadPuntos; b++)
                {
                    if (!(b <= valoresS1yS2[0] || b >= valoresS1yS2[1]))
                    {
                        bool bandera = false;
                        while (bandera == false && columna < cantidadPuntos)
                        {
                            int valorActual = pobContraria[fila, columna];

                            if (!EsDuplicado(a, valorActual, valoresS1yS2, pobContraria))
                            {
                                pob[a, b] = valorActual;
                                bandera = true;
                            }
                            else
                            {
                                columna++;
                            }
                        }
                        columna++;
                    }
                }
            }
        }

        private bool EsDuplicado(int a, int valor, int[] valoresS1yS2, int[,] pobContraria)
        {
            for (int col = 1; col < cantidadPuntos; col++)
            {
                if (pobContraria[a, col] != 0)
                {
                    if (col <= valoresS1yS2[0] || col >= valoresS1yS2[1])
                    {
                        if (pobContraria[a, col] == valor)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        #endregion

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

        private async Task GuardarDatosExcel(int aptitud, string tiempo)
        {
            string rutaArchivo = "D:\\Escuela\\7 Semestre\\Algoritmos metaheuristicos\\Experimento_AG.xlsx";

            using (var package = new ExcelPackage(new FileInfo(rutaArchivo)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                worksheet.Cells[5, 7].Value = "asdfg";

                await package.SaveAsync();
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

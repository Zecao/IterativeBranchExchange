using ExecutorOpenDSS.Classes;
using ExecutorOpenDSS.Classes_Auxiliares;
using ExecutorOpenDSS.Classes_Principais;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;


namespace ExecutorOpenDSS.Interfaces
{
    /// <summary>
    /// Interaction logic for OpcoesAvancadas.xaml
    /// </summary>
    /// 
    public partial class OpcoesAvancadas : Window
    {
        public MainWindow _janelaPrincipal;
        public ParametrosGUI _parGUI;
        public ParamAvancados _parAvan;

        public OpcoesAvancadas(MainWindow jan, ParametrosGUI parGUI, ParamAvancados parAvan)
        {
            _janelaPrincipal = jan;
            _parGUI = parGUI;
            _parAvan = parAvan;

            //
            InitializeComponent();

            // parametros avancados
            calculaPUOtm.IsChecked = _parAvan._otimizaPUSaidaSE;
            calculaDRPDRCCheckBox.IsChecked = _parAvan._calcDRPDRC;
            calcTensaoBarTrafoCheckBox.IsChecked = _parAvan._calcTensaoBarTrafo;
            verifCargaIsolada.IsChecked = _parAvan._verifCargaIsolada;
        }

        private void SalvarButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelarButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        //Exibe janela de feriados
        private void FeriadosButton_Click(object sender, RoutedEventArgs e)
        {
            FeriadosWindow FW = new FeriadosWindow( _parGUI );
            FW.ShowDialog();
        }

        //
        private void conexoesSE_Button_Click(object sender, RoutedEventArgs e)
        {
            //instancia _paramGerais
            ParamConexoesSE _paramConexoesSE = new ParamConexoesSE(_parGUI);

            //cria objeto conexoes SE
            ConexoesSE conexoes = new ConexoesSE(_paramConexoesSE);
        }

        //
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            // parametros avancados
            _parAvan._otimizaPUSaidaSE = calculaPUOtm.IsChecked.Value;
            _parAvan._calcDRPDRC = calculaDRPDRCCheckBox.IsChecked.Value;
            _parAvan._calcTensaoBarTrafo = calcTensaoBarTrafoCheckBox.IsChecked.Value;
            _parAvan._verifCargaIsolada = verifCargaIsolada.IsChecked.Value;

            //
            this.DialogResult = true;
            //this.Close();
        }



    }
}

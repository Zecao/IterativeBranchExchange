using ExecutorOpenDSS.Classes;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExecutorOpenDSS.Interfaces
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    /// 
    public partial class HolidaysWindow : Window
    {
        private readonly GUIParameters _parGUI;

        public HolidaysWindow(GUIParameters parGUI)
        {
            InitializeComponent();

            //
            _parGUI = parGUI;

            // obtem informacoes do arquivo de veriados
            GetFeriadosArq(_parGUI._ano);

            // 
            this.Title = "Feriados de " + _parGUI._ano;
        }

        private void SalvarButton_Click(object sender, RoutedEventArgs e)
        {
            SetFeriadosArq(_parGUI._ano);
            this.Close();
        }

        private void CancelarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //Seleciona todo o conteúdo da caixa de texto, quando realiza-se um clique duplo
        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        //Valida a caixa 
        private void ValidaTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.Text = tb.Text.Replace(',', ';');
            tb.Text = tb.Text.Replace('/', ';');
            tb.Text = tb.Text.Replace('\\', ';');
            tb.Text = tb.Text.Replace('-', ';');
            tb.Text = tb.Text.Replace('*', ' ');
            string pattern = "[a-z,A-Z,.]";
            Regex rgx = new Regex(pattern);
            tb.Text = rgx.Replace(tb.Text, "");

        }

        //Pega os feriados do ano
        public void GetFeriadosArq(string ano)
        {
            //
            string nomeArqFeriados = _parGUI.GetNomeArquivoFeriadosCompleto();

            if (File.Exists(nomeArqFeriados))
            {
                using (StreamReader file = new StreamReader(nomeArqFeriados))
                {
                    this.janeiroTextBox.Text = file.ReadLine();
                    this.fevereiroTextBox.Text = file.ReadLine();
                    this.marcoTextBox.Text = file.ReadLine();
                    this.abrilTextBox.Text = file.ReadLine();
                    this.maioTextBox.Text = file.ReadLine();
                    this.junhoTextBox.Text = file.ReadLine();
                    this.julhoTextBox.Text = file.ReadLine();
                    this.agostoTextBox.Text = file.ReadLine();
                    this.setembroTextBox.Text = file.ReadLine();
                    this.outubroTextBox.Text = file.ReadLine();
                    this.novembroTextBox.Text = file.ReadLine();
                    this.dezembroTextBox.Text = file.ReadLine();
                }
            }            
        }

        //Grava arquivo com os feriados
        public void SetFeriadosArq(string ano)
        {
            // nome arquivo feriados
            //string nomeArqFeriados = pathRecursosPerm + "Feriados" + ano + ".txt";
            string nomeArqFeriados = _parGUI.GetNomeArquivoFeriadosCompleto();

            using (StreamWriter file = new StreamWriter(nomeArqFeriados, false))
            {
                file.WriteLine(this.janeiroTextBox.Text);
                file.WriteLine(this.fevereiroTextBox.Text);
                file.WriteLine(this.marcoTextBox.Text);
                file.WriteLine(this.abrilTextBox.Text);
                file.WriteLine(this.maioTextBox.Text);
                file.WriteLine(this.junhoTextBox.Text);
                file.WriteLine(this.julhoTextBox.Text);
                file.WriteLine(this.agostoTextBox.Text);
                file.WriteLine(this.setembroTextBox.Text);
                file.WriteLine(this.outubroTextBox.Text);
                file.WriteLine(this.novembroTextBox.Text);
                file.WriteLine(this.dezembroTextBox.Text);
            }
        }

    }
}

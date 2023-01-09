﻿using ExecutorOpenDSS.Classes;
using ExecutorOpenDSS.Classes_Principais;
using ExecutorOpenDSS.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ExecutorOpenDSS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///

    public partial class MainWindow : Window
    {
        //Variáveis Globais da classe
        public System.Windows.Threading.Dispatcher _displayDispatcher;
        public System.Windows.Threading.Dispatcher _mainWindowDispatcher;

        // variaveis da GUI
        public GUIParameters _parGUI = new GUIParameters();
        public GeneralParameters _paramGerais;

        // variaveis classe 
        public bool _cancelarExecucao;
        public bool _fimExecucao = false;
        public string _logName;
        public DateTime _inicio;
        public int _indiceArq = 0;

        //Função principal
        public MainWindow()
        {
            // tem q ser inicializado antes dos componentes
            InitializeComponent();

            //Inicializa os valores da interface
            InicializaValores();

            // prenche variaveis de classe 
            _displayDispatcher = display.Dispatcher; //OBS: devi vir antes new GeneralParameters
            _mainWindowDispatcher = this.Dispatcher;

            // copia variaveis MainWindow
            _parGUI.CopiaVariaveis(this);

            //
            _parGUI.PreencheTipoDia();

            //
            _paramGerais = new GeneralParameters(this);
        }

        //Função que exibe as mensagens na TextBox e as grava em um arquivo de log
        public void ExibeMsgDisplay(string mensagem)
        {
            //Pega o texto do display. Como a interface roda em outro processo, é necessária a utilização
            //de função delegada, dispatcher e invoke
            MainWindow.GetDisplayDelegate getDisplay = new MainWindow.GetDisplayDelegate(GetDisplay);

            //
            string str = _displayDispatcher.Invoke(getDisplay).ToString();

            // chama DispPvt
            str = DispPvt(str, mensagem);

            //Escreve no display da tela.Como a interface roda em outro processo, é necessária a utilização
            //de função delegada, dispatcher e invoke
            MainWindow.UpdateDisplayDelegate update = new MainWindow.UpdateDisplayDelegate(UpdateDisplay);
            _displayDispatcher.BeginInvoke(update, str);

            /*
            // OLD CODE ExibeMsgDisplay
            string str = GetDisplay();

            // chama DispPvt
            str = DispPvt(str, mensagem);

            UpdateDisplay(str);*/
        }

        //
        public string DispPvt(string str, string mensagem)
        {
            //Verifica se o display está em branco.
            if (str.Equals(""))
            {
                //Adiciona a mensagem
                str = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + mensagem;
            }
            else
            {
                //Caso o display não esteja em branco, pula uma linha e adiciona a mensagem
                str = str + "\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + mensagem;
            }
            return str;
        }

        //Função chamada quando o botão "Executar" é clicado
        private void ExecutaButton_Click(object sender, RoutedEventArgs e)
        {
            //Desabilita a interface
            StatusUI(false);

            //FIX ME 
            //_indiceArq = 0;

            //limpa display
            display.Text = "";

            //
            GravaConfig();

            // Roda worker_ExecutaFluxo em background
            Task.Run((Action)Worker_ExecutaFluxo);
        }

        // Executa Fluxo potencia
        void Worker_ExecutaFluxo()
        {
            //
            _inicio = DateTime.Now;

            //Mensagem de Início
            ExibeMsgDisplay("Início");

            //Lê os alimentadores e armazena a lista de alimentadores 
            List<string> alimentadores = CemigFeeders.GetTodos(_parGUI.GetArqLstAlimentadores());

            // carrega dados de medicoes
            _paramGerais._medAlim.CarregaDados();

            // instancia classe ExecutaFluxo
            RunPowerFlow executaFluxoObj = new RunPowerFlow(_paramGerais);

            switch (_parGUI._tipoFluxo)
            {
                //Executa o Fluxo Snap
                case "Snap":

                    executaFluxoObj.ExecutesSnap(alimentadores);

                    break;

                //Executa o fluxo diário
                case "Daily":

                    executaFluxoObj.ExecutesDailyPowerFlow(alimentadores);

                    break;

                //Executa o fluxo diário
                case "Hourly":

                    executaFluxoObj.ExecutesDailyPowerFlow(alimentadores);
                    break;

                //Executa o fluxo mensal
                case "Monthly":

                    executaFluxoObj.ExecutesMonthlyPowerFlow(alimentadores);

                    break;

                //Executa o fluxo mensal
                case "Yearly":

                    executaFluxoObj.ExecutesAnnualPowerFlow(alimentadores);

                    break;

                default:
                    break;
            }

            //Finalização do processo
            FinalizaProcesso(false);

            // Reabilita interface
            ReabilitaInterface();
        }

        //Reabilita a interface. 
        //Como a interface está em outro processo, é necessário utilizar um Dispatcher
        private void ReabilitaInterface() 
        {
            SetButtonDelegate setar = new SetButtonDelegate(SetButton);
            _mainWindowDispatcher.BeginInvoke(setar);
        }

        //Botão para cancelar a execução 
        private void CancelaButton_Click(object sender, RoutedEventArgs e)
        {
            _cancelarExecucao = true;
            CancelaButton.IsEnabled = false;
        }

        // Se o Fluxo for Snap ou Monthly, desabilita a combobox do tipo de dia
        private void TipoFluxoCB_LostFocus(object sender, RoutedEventArgs e)
        {
            switch (tipoFluxoComboBox.Text)
            {
                case "Snap":

                    //1. desabilita a combobox do tipo de dia
                    tipoDiaComboBox.IsEnabled = false;
                  
                    //2. desabilita check-box RELACIONADOS otimiza LoadMult Energia
                    otimizaEnergiaCheckBox.IsEnabled = false;
                    otimizaEnergiaCheckBox.IsChecked = false;
                    simplificaMesComDUCheckBox.IsEnabled = false;
                    simplificaMesComDUCheckBox.IsChecked = false;

                    //4. desabilita textBox hora
                    horaTextBox.IsEnabled = false;

                    //5. habilita combo box do mes
                    mesComboBox.IsEnabled = true;

                    //6. desabilita otimiza energia
                    otimizaEnergiaCheckBox.IsEnabled = false;

                    //8. desabilita otimiza Snap 
                    otimizaCheckBox.IsEnabled = true;

                    break;

                case "Hourly":

                    //2. desabilita check-box RELACIONADOS otimiza LoadMult Energia
                    otimizaEnergiaCheckBox.IsEnabled = false;
                    otimizaEnergiaCheckBox.IsChecked = false;
                    simplificaMesComDUCheckBox.IsEnabled = false;
                    simplificaMesComDUCheckBox.IsChecked = false;

                    //5. habilita combo box do mes
                    mesComboBox.IsEnabled = true;

                    //6. desabilita otimiza energia
                    otimizaEnergiaCheckBox.IsEnabled = false;

                    //8. desabilita otimiza Snap 
                    otimizaCheckBox.IsEnabled = false;

                    // Habilitacao Interface 
                    horaTextBox.IsEnabled = true; //4. habilita o textBox da hora
                    tipoDiaComboBox.IsEnabled = true; //5. habilita combo box do tipo dia                   
                    horaTextBox.IsEnabled = true; // textBox hora

                    break;

                case "Daily":

                    //2. desabilita check-box RELACIONADOS otimiza LoadMult Energia
                    otimizaEnergiaCheckBox.IsEnabled = false;
                    otimizaEnergiaCheckBox.IsChecked = false;
                    simplificaMesComDUCheckBox.IsEnabled = false;
                    simplificaMesComDUCheckBox.IsChecked = false;

                    //4. desabilita textBox hora
                    horaTextBox.IsEnabled = false;

                    //5. habilita combo box do mes
                    mesComboBox.IsEnabled = true;

                    //6. habilita combo box do tipo dia
                    tipoDiaComboBox.IsEnabled = true;

                    //7. desabilita otimiza energia
                    otimizaEnergiaCheckBox.IsEnabled = false;

                    //8. desabilita otimiza Snap 
                    otimizaCheckBox.IsEnabled = false;

                    //9. desabilita modo horario
                    _parGUI._modoHorario = false;

                    break;

                case "Monthly":

                    //1. desabilita a combobox do tipo de dia
                    tipoDiaComboBox.IsEnabled = false;

                    //2.1 desabilita o check-box otimizacao potencia
                    otimizaCheckBox.IsEnabled = false;
                    otimizaCheckBox.IsChecked = false;

                    //4. desabilita textBox hora
                    horaTextBox.IsEnabled = false;

                    //5. habilita combo box do mes
                    mesComboBox.IsEnabled = true;

                    //8. desabilita otimiza Snap 
                    otimizaCheckBox.IsEnabled = false;

                    //7. habilita otimiza energia
                    otimizaEnergiaCheckBox.IsEnabled = true;

                    //9. desabilita modo horario
                    _parGUI._modoHorario = false;

                    break;

                case "Yearly":

                    //1. desabilita a combobox do tipo de dia
                    tipoDiaComboBox.IsEnabled = false;

                    //2.1 desabilita o check-box otimizacao potencia
                    otimizaCheckBox.IsEnabled = false;
                    otimizaCheckBox.IsChecked = false;

                    //4. desabilita textBox hora
                    horaTextBox.IsEnabled = false;

                    //5. ajusta combo box do mes
                    mesComboBox.Text = "Janeiro";
                    mesComboBox.IsEnabled = false;

                    //7. ajusta text box da hora
                    horaTextBox.IsEnabled = false;

                    //8. desabilita otimiza Snap 
                    otimizaCheckBox.IsEnabled = false;

                    //7. habilita otimiza energia
                    otimizaEnergiaCheckBox.IsEnabled = true;

                    //9. desabilita modo horario
                    _parGUI._modoHorario = false;

                    break;

                //Caso contrário, habilita a combobox do tipo de dia
                default:

                    break;
            }
        }
               
        //Verifica se o ano é válido
        private void AnoTB_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                //Tenta um parse para inteiro na caixa de texto de ano
                int ano = Int32.Parse(anoTextBox.Text);

                //Se o ano for menor ou igual a 0 ou maior que 3000, lança uma exceção de índice fora do intervalo
                if(ano<=0||ano>3000){
                    throw new IndexOutOfRangeException();
                }

                //Define a cor do texto para preto, caso não tenham ocorrido erros acima
                anoTextBox.Foreground = Brushes.Black;
                anoTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xE6, 0xE3, 0xE3));
            }
            //Caso ocorra erros no try acima:
            catch
            {
                //Define a cor do texto da caixa como vermelho
                anoTextBox.Foreground = Brushes.Red;
                anoTextBox.BorderBrush = Brushes.Red;

                //Exibe uma caixa de mensagem avisando que o ano é inválido
                MessageBox.Show("Ano inválido","Erro!");
            }
        }
              
        //Função executada quando a TextBox caminhoDSS perde o foco.
        //Valida o caminho.
        private void CaminhoDSSTB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!caminhoDSSTextBox.Text.Equals(""))
            {
                //Verifica se o caminhoDSS termina com "\". Caso contrário, acrescenta
                caminhoDSSTextBox.Text = caminhoDSSTextBox.Text.Last() == '\\' ? caminhoDSSTextBox.Text : caminhoDSSTextBox.Text + '\\';
            }
            
            //Verifica se o caminho existe
            if (Directory.Exists(caminhoDSSTextBox.Text))
            {
                //Caso exista, muda a cor do texto para preto
                caminhoDSSTextBox.Foreground = Brushes.Black;
                caminhoDSSTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xE6, 0xE3, 0xE3));
            }
            else
            {
                //Caso não exista, muda a cor do texto para vermelho
                caminhoDSSTextBox.Foreground = Brushes.Red;
                caminhoDSSTextBox.BorderBrush = Brushes.Red;

                //Exibe mensagem que o caminho não existe
                MessageBox.Show("Caminho dos arquivos DSS não existe", "Erro!");
            }
        }

        //Abre caixa de dialogo para seleção do caminho dos arquivos DSS
        private void CaminhoDSSBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                caminhoDSSTextBox.Text = dialog.SelectedPath;
                CaminhoDSSTB_LostFocus(caminhoDSSTextBox, new RoutedEventArgs());
            }
        }

        //Seleciona todo o conteúdo da caixa de texto, quando realiza-se um clique duplo
        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        //Valida valores de caixas de texto cujo tipo é ponto flutuante
        private void FloatTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox sendert = (sender as TextBox);

            sendert.Text = sendert.Text.Replace(',', '.');

            try
            {
                //Tenta um parse para float na caixa de texto
                float valor = float.Parse(sendert.Text, CultureInfo.InvariantCulture);

                //Define a cor do texto para preto, caso não tenham ocorrido erros acima
                sendert.Foreground = Brushes.Black;
                sendert.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xE6, 0xE3, 0xE3));
            }
            //Caso ocorra erros no try acima:
            catch
            {
                //Define a cor do texto da caixa como vermelho
                sendert.Foreground = Brushes.Red;
                sendert.BorderBrush = Brushes.Red;

                //Exibe uma caixa de mensagem avisando que o ano é inválido
                MessageBox.Show("Valor inválido inserido", "Erro!");
            }
        }
        
        //Desabilita/habilita a interface
        public void StatusUI(bool status)
        {
            ExecutaButton.IsEnabled = status;
            anoTextBox.IsEnabled = status;
            mesComboBox.IsEnabled = status;
            tipoFluxoComboBox.IsEnabled = status;
            caminhoDSSTextBox.IsEnabled = status;
            caminhoPermTextBox.IsEnabled = status;
            caminhoDSSBrowserButton.IsEnabled = status;
            otimizaCheckBox.IsEnabled = status;          
            usarTensoesBarramentoCheckBox.IsEnabled = status;
            tensaoSaidaBarTextBox.IsEnabled = status;
            otimizaEnergiaCheckBox.IsEnabled = status;
            simplificaMesComDUCheckBox.IsEnabled = status;
            incrementoAjusteTextBox.IsEnabled = status;
            loadMultAltTextBox.IsEnabled = status;
            horaTextBox.IsEnabled = status;
            AllowFormsCheckBox.IsEnabled = status;

            //
            CancelaButton.IsEnabled = !status;
        }        
        
        //Finalização do processo
        public void FinalizaProcesso(bool cancelamento)
        {
            if (cancelamento)
            {   
                ExibeMsgDisplay("Execução abortada.");
            }
            else
            {
                ExibeMsgDisplay("Fim da execução.");
            }

            _fimExecucao = true;
            _cancelarExecucao = false;

            // data final para calculo do tempo de execucao
            DateTime fim = DateTime.Now;

            ExibeMsgDisplay("Tempo total de execução: " + (fim - _inicio).ToString());

            // Grava Log
            GravaLog();
        }
        
        //Inicializa valores da interface
        private void InicializaValores()
        {
            _cancelarExecucao = false;

            string arqConfig = AppDomain.CurrentDomain.BaseDirectory + "Configuracoes.xml";
            XMLConfigurationFile.GetConfiguracoes(this, arqConfig);           
        }
        
        //Grava configurações
        public void GravaConfig()
        {
            // preenche variaveis da GUI antes da exeucao
            _parGUI.CopiaVariaveis(this);

            string arqConfig = AppDomain.CurrentDomain.BaseDirectory + "Configuracoes.xml";
            XMLConfigurationFile.SetConfiguracoes(this, arqConfig);
        }

        //Grava o conteúdo da caixa de texto "display" em um arquivo de log nomeado com a data e hora atuais
        public void GravaLog()
        {
            //preenche _logName
            InicializaLog();

            GetDisplayDelegate getDisplay = new GetDisplayDelegate(GetDisplay);
            
            // TODO fix me: erro se Log.txt nao existir
            using (StreamWriter file = new StreamWriter(_logName, true))
            {
                string str = _displayDispatcher.Invoke(getDisplay).ToString();
                file.Write(str);
            }
        }
        
        // Inicializa o arquivo de log "apendando" uma linha com um separador, dividindo o log anterior do atual
        private void InicializaLog()
        {
            //Define o nome do arquivo de log
            _logName = AppDomain.CurrentDomain.BaseDirectory + "Log.txt";
            
            //Apaga o arquivo de log existente
            TxtFile.SafeDelete(_logName);            
        }     

        //Funções para setar o texto do display
        public delegate void UpdateDisplayDelegate(string texto);

        //Funções para pegar o texto do display
        public delegate string GetDisplayDelegate();
        
        //Funções para habilitar/desabilitar a interface
        public delegate void SetButtonDelegate();

        //Exibir caixa de mensagem
        public delegate bool MensagemDelegate(string texto, string titulo = "");

        public void UpdateDisplay(string texto)
        {
            display.Text = texto;
            display.ScrollToEnd();
        }

        //
        public string GetDisplay()
        {
            return display.Text;
        }

        //
        public void SetButton()
        {
            StatusUI(true);
        }

        //
        public bool Mensagem(string texto, string titulo = "Aviso")
        {
            MessageBoxButton botoes = MessageBoxButton.YesNo;
            MessageBoxResult resultado = MessageBox.Show(texto, titulo, botoes);
            if (resultado == MessageBoxResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }            
        }

        //
        private void OtimizaCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // desabilita outro otimiza
            otimizaEnergiaCheckBox.IsChecked = false;

            // habilita Interfaces
            incrementoAjusteTextBox.IsEnabled = true;
            loadMultAltTextBox.IsEnabled = true;
        }

        //
        private void OtimizaCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // habilita Interfaces
            incrementoAjusteTextBox.IsEnabled = false;
            loadMultAltTextBox.IsEnabled = false;

            otimizaCheckBox.IsChecked = false;         
        }

        private void OtimizaEnergiaCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // habilita Interfaces
            incrementoAjusteTextBox.IsEnabled = true;
            simplificaMesComDUCheckBox.IsEnabled = true;
            loadMultAltTextBox.IsEnabled = true;
        }

        private void OtimizaEnergiaCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // desabilita Interfaces
            incrementoAjusteTextBox.IsEnabled = false;
            simplificaMesComDUCheckBox.IsEnabled = false;
            simplificaMesComDUCheckBox.IsChecked = false;
            loadMultAltTextBox.IsEnabled = false;
        }

        // Caminho
        private void CaminhoTB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!caminhoPermTextBox.Text.Equals(""))
            {
                //Verifica se o caminhoDSS termina com "\". Caso contrário, acrescenta
                caminhoPermTextBox.Text = caminhoPermTextBox.Text.Last() == '\\' ? caminhoPermTextBox.Text : caminhoPermTextBox.Text + '\\';
            }

            //Verifica se o caminho existe
            if (Directory.Exists(caminhoPermTextBox.Text))
            {
                //Caso exista, muda a cor do texto para preto
                caminhoPermTextBox.Foreground = Brushes.Black;
                caminhoPermTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xE6, 0xE3, 0xE3));
            }
            else
            {
                //Caso não exista, muda a cor do texto para vermelho
                caminhoPermTextBox.Foreground = Brushes.Red;
                caminhoPermTextBox.BorderBrush = Brushes.Red;

                //Exibe mensagem que o caminho não existe
                MessageBox.Show("Caminho dos arquivos DSS não existe", "Erro!");
            }
        }

        // seleciona caminho path recursos permanentes
        private void CaminhoPermBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                caminhoPermTextBox.Text = dialog.SelectedPath;
                CaminhoDSSTB_LostFocus(caminhoPermTextBox, new RoutedEventArgs());
            }
        }

        // habilita Text, caso conformo 
        private void UsarTensoesBarramentoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // habilita TextBox valor default pu
            tensaoSaidaBarTextBox.IsEnabled = true;
        }

        private void UsarTensoesBarramentoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // desahabilita TextBox valor default pu
            tensaoSaidaBarTextBox.IsEnabled = false;
        }

        // perda de foco TexBox hora do modo diario
        private void HoraTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            double horaInt = Int32.Parse(horaTextBox.Text);

            // verifica se hora eh valida
            if (horaInt < 0 || horaInt > 23)
            {
                ExibeMsgDisplay("Entre com uma hora válida, entre 0-23hrs!");

                _parGUI._hora = "0";
                horaTextBox.Text = "0";
            }
            else
            {
                _parGUI._hora = horaTextBox.Text;

                ExibeMsgDisplay("Nova hora definida");
            }
        }
       
        // Atualiza classe com mes escolhido
        private void MesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _parGUI._mes = mesComboBox.SelectedItem.ToString();
            _parGUI.SetMes( mesComboBox.SelectedIndex + 1 );
            _parGUI._mesAbrv3letras = _parGUI._mes.Substring(0, 3);
        }

        //Exibe janela de feriados
        private void FeriadosButton_Click(object sender, RoutedEventArgs e)
        {
            HolidaysWindow FW = new HolidaysWindow(_parGUI);
            FW.ShowDialog();
        }

        private void AnaliseChavesNAs_Click(object sender, RoutedEventArgs e)
        {
            // data final para calculo do tempo de execucao
            _inicio = DateTime.Now;

            // grava configuracoes
            GravaConfig();

            // Roda worker_ExecutaFluxo em background
            Task.Run((Action)Worker_NormallyOpenSwitchAnalisys);
        }

        // Executa NormallyOpenSwitchAnalisys
        void Worker_NormallyOpenSwitchAnalisys()
        {
            //Mensagem de Início
            ExibeMsgDisplay("Início análise chaves NAs");

            //Lê os alimentadores e armazena a lista de alimentadores 
            List<string> lstAlimentadores = CemigFeeders.GetTodos(_parGUI.GetArqLstAlimentadores());

            // load xlsx loadmults and xlsx Energy/month
            _paramGerais._medAlim.CarregaDados();

            // instancia classe AnaliseChavesNAs
            NOSwitchAnalysis analiseChavesNAs = new NOSwitchAnalysis( _paramGerais, lstAlimentadores);

            // Fim 
            ExibeMsgDisplay("Fim análise chaves NAs");

            // OLD CODE FIX Reabilita interface
            //ReabilitaInterface();

            // Finaliza processo
            FinalizaProcesso(false);
        }

        private void AnaliseLoops_Click(object sender, RoutedEventArgs e)
        {
            // data final para calculo do tempo de execucao
            _inicio = DateTime.Now;

            // TODO verificar necessidade
            // carrega dados de medicoes
            // _medAlim.CarregaDados();

            // grava configuracoes
            GravaConfig();

            // Roda worker_ExecutaFluxo em background
            Task.Run((Action)Worker_ExecutaAnaliseLoops);
        }

        // Executa Analise de Loops
        void Worker_ExecutaAnaliseLoops()
        {
            //Mensagem de Início
            ExibeMsgDisplay("Início Analise Loops");

            //Lê os alimentadores e armazena a lista de alimentadores 
            List<string> alimentadores = CemigFeeders.GetTodos(_parGUI.GetArqLstAlimentadores());

            // instancia classe AnaliseChavesNAs
            LoopAnalysis analiseLoops = new LoopAnalysis(_paramGerais, alimentadores);

            // Fim 
            ExibeMsgDisplay("Fim análise de loops");

            // TODO Reabilita interface
            //ReabilitaInterface();

            // Finaliza processo
            FinalizaProcesso(false);
        }

        //
        private void ComparaManobras_Click(object sender, RoutedEventArgs e)
        {
            // data final para calculo do tempo de execucao
            _inicio = DateTime.Now;

            // grava configuracoes
            GravaConfig();

            // Roda worker_ComparaManobras em background
            Task.Run((Action)Worker_ComparaManobras);
        }

        // Executa Fluxo potencia
        void Worker_ComparaManobras()
        {
            //Mensagem de Início
            ExibeMsgDisplay("Comparação Manobras");

            //Lê os alimentadores e armazena a lista de alimentadores 
            //List<string> alimentadores = CemigFeeders.GetTodos(this._parGUI.GetArqLstAlimentadores());

            // TODO eh necessario?
            // carrega dados de medicoes
            //_medAlim.CarregaDados();

            // instancia classe
            //ComparacaoManobras compManobras = new ComparacaoManobras(paramGerais, alimentadores );

            // Fim 
            ExibeMsgDisplay("Comparação Manobras");

            // TODO Reabilita interface
            //ReabilitaInterface();

            // Finaliza processo
            FinalizaProcesso(false);
        }

    }
}

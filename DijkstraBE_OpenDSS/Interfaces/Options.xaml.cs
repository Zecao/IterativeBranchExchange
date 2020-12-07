using ExecutorOpenDSS.Classes_Auxiliares;
using ExecutorOpenDSS.Classes_Principais;
using ExecutorOpenDSS.Reconfigurador;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace ExecutorOpenDSS.Interfaces
{
    /// <summary>
    /// Interaction logic for OpcoesAvancadas.xaml
    /// </summary>
    /// 
    public partial class Options : Window
    {
        public MainWindow _janelaPrincipal;
        public AdvancedParameters _parAvanRet;
        //public DateTime _inicio;

        public Options(MainWindow jan)
        {
            // janela principal
            _janelaPrincipal = jan;

            //
            InitializeComponent();

            // prenche janela ParametrosAvancados com valores da classe parAvancados
            ParamAvancados2GUI();
        }

        // preenche janela com configuracoes.XML
        private void ParamAvancados2GUI()
        {
            // parametros avancados
            calculaPUOtm.IsChecked = _janelaPrincipal._parAvan._otimizaPUSaidaSE;
            calculaDRPDRCCheckBox.IsChecked = _janelaPrincipal._parAvan._calcDRPDRC;
            calcTensaoBarTrafoCheckBox.IsChecked = _janelaPrincipal._parAvan._calcTensaoBarTrafo;
            verifCargaIsolada.IsChecked = _janelaPrincipal._parAvan._verifCargaIsolada;
            verifTapsRTs.IsChecked = _janelaPrincipal._parAvan._verifTapsRTs;
            IncluiCapMTCheckBox.IsChecked = _janelaPrincipal._parAvan._incluirCapMT;
            ModeloCargaCemig.IsChecked = _janelaPrincipal._parAvan._modeloCargaCemig;
            TBBatchEdit.Text = _janelaPrincipal._parAvan._strBatchEdit;
        }

        // preenche janela com configuracoes.XML
        private void GUI2paramAvancados()
        {
            // parametros avancados
            _janelaPrincipal._parAvan._otimizaPUSaidaSE = calculaPUOtm.IsChecked.Value;
            _janelaPrincipal._parAvan._calcDRPDRC = calculaDRPDRCCheckBox.IsChecked.Value;
            _janelaPrincipal._parAvan._calcTensaoBarTrafo = calcTensaoBarTrafoCheckBox.IsChecked.Value;
            _janelaPrincipal._parAvan._verifCargaIsolada = verifCargaIsolada.IsChecked.Value;
            _janelaPrincipal._parAvan._verifTapsRTs = verifTapsRTs.IsChecked.Value;
            _janelaPrincipal._parAvan._incluirCapMT = IncluiCapMTCheckBox.IsChecked.Value;
            _janelaPrincipal._parAvan._modeloCargaCemig = ModeloCargaCemig.IsChecked.Value;
            _janelaPrincipal._parAvan._strBatchEdit = TBBatchEdit.Text;
        }

        //Exibe janela de feriados
        private void FeriadosButton_Click(object sender, RoutedEventArgs e)
        {
            HolidaysWindow FW = new HolidaysWindow(_janelaPrincipal._parGUI);
            FW.ShowDialog();
        }

        //
        private void ConexoesSE_Button_Click(object sender, RoutedEventArgs e)
        {
            //instancia _paramGerais
            SEConnectionsParameters _paramConexoesSE = new SEConnectionsParameters(_janelaPrincipal._parGUI);

            //cria objeto conexoes SE
            SEConnections conexoes = new SEConnections(_paramConexoesSE);
        }

        // botao ok
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            // copia variaveis da GUI para objeto param avancados
            GUI2paramAvancados();

            this.DialogResult = true;
        }

        // Obtem chaves NAs alimentador
        private void GetChavesNAs_Click(object sender, RoutedEventArgs e)
        {
            // copia variaveis da GUI para objeto param avancados
            GUI2paramAvancados();

            // grava configuracoes
            _janelaPrincipal.GravaConfig();

            // data final para calculo do tempo de execucao
            _janelaPrincipal._inicio = DateTime.Now;

            // Roda worker_ExecutaFluxo em background
            Task.Run((Action)Worker_ExecutaChavesNAs);
        }

        // Executa Fluxo potencia
        void Worker_ExecutaChavesNAs()
        {
            //Mensagem de Início
            _janelaPrincipal.ExibeMsgDisplayMW("Início Chaves NAs");

            //Lê os alimentadores e armazena a lista de alimentadores 
            List<string> alimentadores = CemigFeeders.GetTodos(_janelaPrincipal._parGUI.GetArqLstAlimentadores());

            // TODO testar carrega dados de medicoes
            _janelaPrincipal._medAlim.CarregaDados();

            // instancia classe de parametros Gerais
            GeneralParameters paramGerais = new GeneralParameters(_janelaPrincipal);

            // cria objeto DSS
            ObjDSS oDSS = new ObjDSS(paramGerais);

            // instancia classe AnaliseChavesNAs
            NOSwitchAnalysis analiseChavesNAs = new NOSwitchAnalysis(_janelaPrincipal, paramGerais, alimentadores, oDSS);

            // Fim 
            _janelaPrincipal.ExibeMsgDisplayMW("Fim Chaves NAs");

            // Finaliza processo
            _janelaPrincipal.FinalizaProcesso(false);
        }

        private void ComparaManobras_Click(object sender, RoutedEventArgs e)
        {
            // copia variaveis da GUI para objeto param avancados
            GUI2paramAvancados();

            // grava configuracoes
            _janelaPrincipal.GravaConfig();

            // data final para calculo do tempo de execucao
            _janelaPrincipal._inicio = DateTime.Now;

            // Roda worker_ExecutaFluxo em background
            Task.Run((Action)Worker_ComparaManobras);
        }

        // Executa Fluxo potencia
        void Worker_ComparaManobras()
        {
            //Mensagem de Início
            _janelaPrincipal.ExibeMsgDisplayMW("Comparação Manobras");

            //Lê os alimentadores e armazena a lista de alimentadores 
            List<string> alimentadores = CemigFeeders.GetTodos(_janelaPrincipal._parGUI.GetArqLstAlimentadores());

            // TODO testar carrega dados de medicoes
            _janelaPrincipal._medAlim.CarregaDados();

            // instancia classe de parametros Gerais
            GeneralParameters paramGerais = new GeneralParameters(_janelaPrincipal);

            // cria objeto DSS
            ObjDSS oDSS = new ObjDSS(paramGerais);

            // instancia classe
            ComparacaoManobras compManobras = new ComparacaoManobras(_janelaPrincipal, paramGerais, alimentadores, oDSS);

            // Fim 
            _janelaPrincipal.ExibeMsgDisplayMW("Comparação Manobras");

            // Finaliza processo
            _janelaPrincipal.FinalizaProcesso(false);
        }

        //
        private void CancelarButton_Click(object sender, RoutedEventArgs e)
        {
            // Finaliza processo
            //_janelaPrincipal.FinalizaProcesso(true);

            // Se estiver executando algo em outra thread
            _janelaPrincipal._cancelarExecucao = true;

            //
            this.DialogResult = false;
        }

        private void AnaliseLoops_Click(object sender, RoutedEventArgs e)
        {
            // copia variaveis da GUI para objeto param avancados
            GUI2paramAvancados();

            // grava configuracoes
            _janelaPrincipal.GravaConfig();

            // carrega dados de medicoes
            _janelaPrincipal._medAlim.CarregaDados();

            // data final para calculo do tempo de execucao
            _janelaPrincipal._inicio = DateTime.Now;

            // Roda worker_ExecutaFluxo em background
            Task.Run((Action)Worker_ExecutaAnaliseLoops);
        }

        // Executa Analise de Loops
        void Worker_ExecutaAnaliseLoops()
        {
            //Mensagem de Início
            _janelaPrincipal.ExibeMsgDisplayMW("Início Analise Loops");

            //Lê os alimentadores e armazena a lista de alimentadores 
            List<string> alimentadores = CemigFeeders.GetTodos(_janelaPrincipal._parGUI.GetArqLstAlimentadores());

            // instancia classe de parametros Gerais
            GeneralParameters paramGerais = new GeneralParameters(_janelaPrincipal);

            // cria objeto DSS
            ObjDSS oDSS = new ObjDSS(paramGerais);

            // instancia classe AnaliseChavesNAs
            LoopAnalysis analiseLoops = new LoopAnalysis(_janelaPrincipal, paramGerais, alimentadores, oDSS);

            // Fim 
            _janelaPrincipal.ExibeMsgDisplayMW("Fim análise de loops");

            // Finaliza processo
            _janelaPrincipal.FinalizaProcesso(false);
        }
    }
}

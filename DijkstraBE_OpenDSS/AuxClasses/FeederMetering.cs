using System.Collections.Generic;

namespace ExecutorOpenDSS.Classes_Auxiliares
{

    public class FeederMetering
    {
        public MainWindow _janela;
        public GeneralParameters _paramGerais;

        //booleanas para o controle se arquivo Excel foi carregado
        public bool _reqEnergiaMesCarregado = false;

        //public bool _mapTensaoBarramentoCarregado = false;
        public bool _reqDemandaMaxCarregado = false;

        // map demanda max X alim
        internal Dictionary<string, double> _mapDadosDemanda;

        // map energia injetada X alim
        internal MonthlyEnergy _reqEnergiaMes;

        // map de tensoes do barramento
        // internal Dictionary<string, double> _mapTensaoBarramento; // TODO

        // Load MultMes
        internal MonthLoadMult _reqLoadMultMes;

        //contrutor
        public FeederMetering(MainWindow janela, GeneralParameters par)
        {
            _janela = janela;
            _paramGerais = par;

            _reqEnergiaMes = new MonthlyEnergy(par);
        }

        public void CarregaDados()
        {
            // Arquivo de loadMUlt sempre sera carregado
            _janela.ExibeMsgDisplayMW("Carregando arquivo de LoadMults...");

            // Carrega map com os valores dos loadMult por alimentador
            _reqLoadMultMes = new MonthLoadMult(_paramGerais);
                 
            // carrega arquivo de requisito uma unica vez
            if ((_janela._parGUI._otmPorEnergia) && (!_reqEnergiaMesCarregado))
            {
                _janela.ExibeMsgDisplayMW("Carregando arquivo de requisitos de energia...");

                // Carrega map com valores de energia mes do alimentador
                _reqEnergiaMes.CarregaMapEnergiaMesAlim();

                _reqEnergiaMesCarregado = true;
            }

            // carrega maps conforme necessidade
            if ((_janela._parGUI._otmPorDemMax) && (!_reqDemandaMaxCarregado))
            {
                _janela.ExibeMsgDisplayMW("Carregando arquivo de LoadMults demanda...");

                // Carrega map com valores de demanda maxima do alimentador
                _janela._medAlim.CarregaMapDemandaMaxAlim();

                _reqDemandaMaxCarregado = true;
            }
        }

        //Tradução da função carregaMapDemandaMaxAlim
        private void CarregaMapDemandaMaxAlim( )
        {
            // TODO refactory 
            string nomeArquivoCompleto = _paramGerais._parGUI._pathRecursosPerm + _paramGerais._arqDemandaMaxAlim;

            _janela._medAlim._mapDadosDemanda = XLSXFile.XLSX2Dictionary(nomeArquivoCompleto);
        }

        internal void AtualizaMapAlimLoadMult(double loadMult)
        {
            _reqLoadMultMes.AtualizaMapAlimLoadMult(_paramGerais.GetNomeAlimAtual(), loadMult, _paramGerais._parGUI.GetMes());
        }
    }
}

using System.Collections.Generic;

namespace ExecutorOpenDSS.Classes_Auxiliares
{

    public class FeederMetering
    {
        public GeneralParameters _paramGerais;

        //booleanas para o controle se arquivo Excel foi carregado
        public bool _reqEnergiaMesCarregado = false;

        //public bool _mapTensaoBarramentoCarregado = false;
        public bool _reqDemandaMaxCarregado = false;

        // map demanda max X alim
        internal Dictionary<string, double> _mapDadosDemanda;

        // map energia injetada X alim
        internal MonthlyEnergy _reqEnergiaMes;

        // Load MultMes
        internal MonthLoadMult _reqLoadMultMes;

        //contrutor
        public FeederMetering(GeneralParameters par)
        {
            _paramGerais = par;

            _reqEnergiaMes = new MonthlyEnergy(par);
        }

        public void CarregaDados()
        {
            // Arquivo de loadMUlt sempre sera carregado
            _paramGerais._mWindow.ExibeMsgDisplay("Carregando arquivo de LoadMults...");

            // Carrega map com os valores dos loadMult por alimentador
            _reqLoadMultMes = new MonthLoadMult(_paramGerais);
                 
            // carrega arquivo de requisito uma unica vez
            if ((_paramGerais._mWindow._parGUI._otmPorEnergia) && (!_reqEnergiaMesCarregado))
            {
                _paramGerais._mWindow.ExibeMsgDisplay("Carregando arquivo de requisitos de energia...");

                // Carrega map com valores de energia mes do alimentador
                _reqEnergiaMes.CarregaMapEnergiaMesAlim();

                _reqEnergiaMesCarregado = true;
            }

            // carrega maps conforme necessidade
            if ((_paramGerais._mWindow._parGUI._otmPorDemMax) && (!_reqDemandaMaxCarregado))
            {
                _paramGerais._mWindow.ExibeMsgDisplay("Carregando arquivo de LoadMults demanda...");

                // Carrega map com valores de demanda maxima do alimentador
                _paramGerais._medAlim.CarregaMapDemandaMaxAlim();

                _reqDemandaMaxCarregado = true;
            }
        }

        // Load map from Excel file with demand values
        private void CarregaMapDemandaMaxAlim( )
        {
            // TODO refactory 
            string nomeArquivoCompleto = _paramGerais._parGUI._pathRecursosPerm + _paramGerais._arqDemandaMaxAlim;

            _paramGerais._medAlim._mapDadosDemanda = XLSXFile.XLSX2Dictionary(nomeArquivoCompleto);
        }

        internal void AtualizaMapAlimLoadMult(double loadMult)
        {
            _reqLoadMultMes.AtualizaMapAlimLoadMult(_paramGerais.GetNomeAlimAtual(), loadMult, _paramGerais._parGUI.GetMes());
        }
    }
}

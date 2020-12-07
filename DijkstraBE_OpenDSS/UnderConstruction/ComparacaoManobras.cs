using System.Collections.Generic;
using ExecutorOpenDSS.Classes_Principais;

namespace ExecutorOpenDSS.Reconfigurador
{
    class ComparacaoManobras
    {
        private MainWindow _janela;
        private GeneralParameters _paramGerais;
        private MonthlyPowerFlow _fluxoMensal;
        private ObjDSS _oDSS;

        // lista de niveis de tensao
        private List<double> _VmagPu_antes;
        private List<double> _VmagPu_depois;
        private List<string> _NomeBarras;

        // lista de barras com DRP
        private List<string> _DRPDRC_depois;        

        public ComparacaoManobras(MainWindow janelaPrincipal, GeneralParameters paramGerais, List<string> lstAlimentadores, ObjDSS oDSS)
        {
            _janela = janelaPrincipal;
            _paramGerais = paramGerais;
            _oDSS = oDSS;

            // analisa chave NA de cada alimentador
            foreach (string nomeAlim in lstAlimentadores)
            {
                // Calcula Caso BAse
                CalculaFluxoMensalBase(nomeAlim);

                string cmdManobra = "close line.ctr128251 term=1";
                string cmdManobra2 = "open line.ctr134565 term=1";

                // Realiza manobra
                ExecutaManobra(cmdManobra);

                // Realiza manobra
                ExecutaManobra(cmdManobra2);
                
                // recalcula fluxo
                CalculaFluxo();
            }

            //DEBUG 
            //Imprime arquivo niveis de tensao antes e depois
            ImprimeNiveisTensao();
        }

        //
        public void ImprimeNiveisTensao()
        {
            //nome arquivo DRP e DRC
            string nomeArq = _paramGerais.GetNomeComp_arqBarrasDRPDRC();

            // nome alim
            string nomeAlim = _paramGerais.GetNomeAlimAtual();

            // linha  
            List<string> lstStr = new List<string>();

            //
            for (int i=0; i < _VmagPu_antes.Count; i++)
            {
                lstStr.Add(_NomeBarras[i] + "\t" + _VmagPu_antes[i].ToString() + "\t" + _VmagPu_depois[i].ToString());
            }

            //Grava em arquivo
            TxtFile.GravaListArquivoTXT(lstStr, nomeArq, _janela);
        }

        // executa manobra
        private void ExecutaManobra(string comando)
        {
            // get load bus name
            _oDSS._DSSText.Command = comando;

            //DEBUG 
            //string result = _oDSS._DSSText.Result;
        }

        private bool CalculaFluxo()
        {
            // TODO Inserir opcao de Otimizar tambem FluxoMensal
            bool ret = _fluxoMensal.ExecutaFluxoMensalSimples();

            // se covergiu
            if (ret)
            {
                // TODO Inserir opcao de Otimizar tambem FluxoMensal
                //bool ret2 = _fluxoMensal.ExecutaFluxoMaximaDiaria();

                // obtem barras DRP
                _DRPDRC_depois = _fluxoMensal.GetBarrasDRPDRC();

                // nivel de tensao depois
                _VmagPu_depois = GetAllBusVmagPu();
            }
            else
            {
                _janela.ExibeMsgDisplayMW("Caso manobrado não convergiu");
            }
            return ret;
        }

        // Obtem 
        public List<double> GetAllBusVmagPu()
        {
            double [] allBusVmagPu = _oDSS.GetActiveCircuit().AllBusVmagPu;

            return new List<double>(allBusVmagPu);
        }

        // Obtem 
        public void GetAllBusNames()
        {
            string[] allBusNames = _oDSS.GetActiveCircuit().AllBusNames;

            _NomeBarras = new List<string>(allBusNames);
        }        

        // calcula fluxo mensal sobre o "caso base" e armazena resultado de perdas em variavel da classe.
        private bool CalculaFluxoMensalBase(string nomeAlim)
        {
            // TODO criar nova flag interna 
            // seta este parametro para true para evitar a recarga dos arquivos texto
             _paramGerais._parGUI._otmPorEnergia = true;
            _paramGerais._parGUI.SetAproximaFluxoMensalPorDU(true);

            // atribui nomeAlim
            _paramGerais.SetNomeAlimAtual(nomeAlim);

            // Carrega arquivos DSS
            _fluxoMensal = new MonthlyPowerFlow(_paramGerais, _janela, _oDSS);

            // Carrega alimentador
            _fluxoMensal.CarregaAlimentador();

            // 
            _janela.ExibeMsgDisplayMW("Configuração Inicial");

            // TODO Inserir opcao de Otimizar tambem FluxoMensal
            bool ret = _fluxoMensal.ExecutaFluxoMensalSimples();

            // se covergiu
            if (ret)
            {
                // TODO
                //bool ret2 = _fluxoMensal.ExecutaFluxoMaximaDiaria();

                // nivel de tensao antes
                _VmagPu_antes = GetAllBusVmagPu();

                // nome das barras
                GetAllBusNames();
                               
                /*
                // TODO utilizar esta variavel nas analises
                // preeenche variavel cargas isoladas
                _numCargasIsoladas = _fluxoMensal.GetNumCargasIsoladas();
                */
            }
            else
            {
                _janela.ExibeMsgDisplayMW("Caso Base Não convergiu");
            }
            return ret;
        }
    }
}

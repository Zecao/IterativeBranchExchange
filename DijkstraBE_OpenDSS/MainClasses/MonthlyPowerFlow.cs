#define ENGINE
#if ENGINE
using OpenDSSengine;
#else
using dss_sharp;
#endif

using ExecutorOpenDSS.Classes;
using System.Collections.Generic;

namespace ExecutorOpenDSS.Classes_Principais
{
    class MonthlyPowerFlow
    {
        public int _nFP = 0;
        private DailyFlow _fluxoDU;
        private DailyFlow _fluxoSA;
        private DailyFlow _fluxoDO;

        private GeneralParameters _paramGerais; //TODO refactory uma vez que esta variavel ja e armazena na classe _FluxoDiario (de maneira semelhante do getObjDSS
        private MainWindow _janela;
        public PFResults _resFluxoMensal;

        public MonthlyPowerFlow(GeneralParameters paramGerais, MainWindow janela, ObjDSS oDSS)
        {
            // preenche variaveis da classe
            _paramGerais = paramGerais;
            _janela = janela;

            // fluxo dia util 
            _fluxoDU = new DailyFlow(paramGerais, janela, oDSS, "DU");

            // fluxo sabado 
            _fluxoSA = new DailyFlow(paramGerais, janela, oDSS, "SA");

            // fluxo domingo
            _fluxoDO = new DailyFlow(paramGerais, janela, oDSS, "DO");

            // instancia obj resultado Mensal
            _resFluxoMensal = new PFResults();
        }

        // calcula fluxo mensal sobre o "caso base" e armazena resultado de perdas em variavel da classe.
        public bool CalculaFluxoMensalBase()
        {
            // Load week day DSS files/struct  
            LoadFeederDSSFiles_weekDay();

            // 
            _janela.ExibeMsgDisplayMW("Configuração Inicial");

            bool ret = ExecutaFluxoMensalSimples();

            return ret;
        }


        // OBS: criada este metodo nesta classe, para chamar a funcao do fluxo diario
        // Executa fluxo diario OU horario caso seja passado string hora
        public bool ExecutaFluxoHorario(string hora = null)
        {
            // variavel de retorno;
            bool ret = _fluxoDU.ExecutaFluxoHorario(hora);           

            return ret;
        }

        // ajusta modelo de carga
        public void AjustaModeloDeCargaCemig(int sentidoBusca)
        {
            // ajusta modelos de carga
            AjustaModeloDeCargaPvt(sentidoBusca);

            // ajusta taps RT
            AjustaTapsRTs(sentidoBusca);
        }

        // TODO
        private void AjustaTapsRTs(int sentidoBusca)
        {
            if (sentidoBusca == 1)
            {
                _fluxoDU.AjustaTapsRTs(126);
                _fluxoSA.AjustaTapsRTs(126);
                _fluxoDO.AjustaTapsRTs(126);
            }
            else
            {
                _fluxoDU.AjustaTapsRTs(120);
                _fluxoSA.AjustaTapsRTs(120);
                _fluxoDO.AjustaTapsRTs(120);
            }
        }

        // ajusta modelo de carga 
        private void AjustaModeloDeCargaPvt(int sentidoBusca)
        {
            // condicao de saida
            if (sentidoBusca == -1)
            {
                return;
            }

            // se modelo de carga Cemig == true
            if (_paramGerais._parAvan._modeloCargaCemig)
            {
                List<string> lstCommandsDSSModeloCarga = new List<string>
                {
                    // modelo 100% P
                    "new load.M2constZ pf = 0.92,Vminpu = 0.92,Vmaxpu = 1.5,model = 1,status = variable",
                    "new load.M3constPsqQ pf = 0.92,Vminpu = 0.92,Vmaxpu = 1.5,model = 1,status = variable"
                };

                /* // TODO caso decida implementar outros modelos
                if (sentidoBusca == 1)
                {
                }
                else
                {
                    // modelo padrao ANEEL
                    lstCommandsDSSModeloCarga.Add("new load.M2constZ pf = 0.92,Vminpu = 0.92,Vmaxpu = 1.5,model = 2,status = variable");
                    lstCommandsDSSModeloCarga.Add("new load.M3constPsqQ pf = 0.92,Vminpu = 0.92,Vmaxpu = 1.5,model = 3,status = variable");
                }*/

                // Set novos modelos de carga
                _fluxoDU.SetModeloDeCarga(lstCommandsDSSModeloCarga);
                _fluxoSA.SetModeloDeCarga(lstCommandsDSSModeloCarga);
                _fluxoDO.SetModeloDeCarga(lstCommandsDSSModeloCarga);
            }
        }

        internal List<string> GetBarrasDRPDRC()
        {
            return _fluxoDU.GetBarrasDRPDRC();
        }


        public bool LoadFeederDSSFiles_weekDay()
        {
            // se nao carregar algum dos dias, retorna false
            if (!_fluxoDU.CarregaAlimentador())
            {
                return false;
            }
            return true;
        }

        public bool CarregaAlimentador()
        {
            // se nao carregar algum dos dias, retorna false
            if (!_fluxoDU.CarregaAlimentador())
            {
                return false;
            }
            if (!_fluxoSA.CarregaAlimentador())
            {
                return false;
            }
            if (!_fluxoDO.CarregaAlimentador())
            {
                return false;
            }
            return true;
        }

        // TODO refactory. pode ser fonte de problema retornar sempre _oDSS do DU.
        // obtem objetoDSS
        internal ObjDSS GetObjDSS()
        {
            return _fluxoDU._oDSS;
        }

        // Executa fluxo mensal
        public bool ExecutaFluxoMensal()
        {
            //Executa fluxo diário openDSS. Se alimentador não convergiu, não calcula SA e DO
            if ( ! _fluxoDU.ExecutaFluxoDiario() )
            {
                return false;
            }

            //Executa fluxo diário openDSS. Se alimentador não convergiu, não calcula DO
            if (!_fluxoSA.ExecutaFluxoDiario())
            {
                return false;
            }

            //Executa fluxo diário openDSS
            if (!_fluxoDO.ExecutaFluxoDiario())
            {
                return false;
            }

            // calcula resultados mensal 
            _resFluxoMensal.CalculaResultadoFluxoMensal(_fluxoDU._resFluxo, _fluxoSA._resFluxo, _fluxoDO._resFluxo, _paramGerais, _janela);

            //nivel pu
            string nivelTensaoPU = _fluxoDU._oDSS.GetActiveCircuit().Vsources.pu.ToString("0.###");

            //Plota perdas na tela //TODO fix me
            _janela.ExibeMsgDisplayMW(_resFluxoMensal.GetResultadoFluxoToConsole(
                nivelTensaoPU, _paramGerais.GetNomeAlimAtual()));

            return true;
        }

        // Fluxo mensal simplificado (numDiasDoMes X fluxoDiaUtil)
        internal bool ExecutaFluxoMensalSimples()
        {
            bool ret = false;

            //Executa fluxo diário openDSS
            ret = _fluxoDU.ExecutaFluxoMensalSimples();

            // se convergiu 
            if (ret)
            {
                // incrementa contador de fluxo
                _nFP++;

                // calculo perdas
                _resFluxoMensal.SetEnergiaPerdasFluxoSimples(_fluxoDU._resFluxo, _paramGerais);

                //nivel pu
                string nivelTensaoPU = _fluxoDU._oDSS.GetActiveCircuit().Vsources.pu.ToString("0.###");

                //Plota perdas na tela // TODO fix me
                _janela.ExibeMsgDisplayMW(_resFluxoMensal.GetResultadoFluxoToConsole(nivelTensaoPU
                    , _paramGerais.GetNomeAlimAtual()));
            }
            return ret;
        }

        // Conta cargas isoladas
        public int GetNumCargasIsoladas()
        {
            string[] _arrayCargasIsoladas = _fluxoDU._oDSS.GetActiveCircuit().Topology.AllIsolatedLoads;

            return _arrayCargasIsoladas.GetLength(0);
        }
    }
}

//#define ENGINE
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
        public DailyFlow _fluxoDU;
        private readonly DailyFlow _fluxoSA;
        private readonly DailyFlow _fluxoDO;

        private readonly GeneralParameters _par;
        public PFResults _resFluxoMensal;
                
        public MonthlyPowerFlow(GeneralParameters paramGerais)
        {
            _par = paramGerais;

            ObjDSS oDSS = new ObjDSS(paramGerais);

            // fluxo dia util 
            _fluxoDU = new DailyFlow(paramGerais, oDSS, "DU");

            // fluxo sabado 
            _fluxoSA = new DailyFlow(paramGerais, oDSS, "SA");

            // fluxo domingo
            _fluxoDO = new DailyFlow(paramGerais, oDSS, "DO");

            // instancia obj resultado Mensal
            _resFluxoMensal = new PFResults();
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
            if (_fluxoDU._paramGerais._parGUI._expanderPar._modeloCargaCemig)
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

        // TODO
        internal List<string> GetBarrasDRPDRC()
        {
            return _fluxoDU.GetBarrasDRPDRC();
        }

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
            _resFluxoMensal = new PFResults();
            _resFluxoMensal.CalculaResultadoFluxoMensal(_fluxoDU._resFluxo, _fluxoSA._resFluxo, _fluxoDO._resFluxo, _fluxoDU._paramGerais);

            //Plota perdas na tela //TODO fix me
            _par._mWindow.ExibeMsgDisplay(_resFluxoMensal.GetResultadoFluxoToConsole(_fluxoDU._paramGerais.GetNomeAlimAtual(), _fluxoDU._oDSS));

            return true;
        }

        // Fluxo mensal simplificado (numDiasDoMes X fluxoDiaUtil)
        internal bool ExecutaFluxoMensalAproximacaoDU()
        {
            //Executa fluxo diário openDSS
            bool ret = _fluxoDU.ExecutaFluxoDiario(); 

            //
            SetEnergiaPerdasFluxoSimples();

            return ret;
        }

        // Fluxo mensal simplificado (numDiasDoMes X fluxoDiaUtil)
        internal bool ExecutaFluxoMensalAproximacaoDU_SemRecarga()
        {
            //Executa fluxo diário openDSS
            bool ret = _fluxoDU.ExecutaFluxoDiario_SemRecarga(null);

            //
            SetEnergiaPerdasFluxoSimples();

            return ret;
        }

        internal void SetEnergiaPerdasFluxoSimples()
        {
            // incrementa contador de fluxo
            _nFP++;

            // calculo perdas
            _resFluxoMensal.EstimatesMonthEnergyAndLossesByDay(_fluxoDU._resFluxo, _fluxoDU._paramGerais);

            //Plota perdas na tela // TODO fix me
            _par._mWindow.ExibeMsgDisplay(_resFluxoMensal.GetResultadoFluxoToConsole(_fluxoDU._paramGerais.GetNomeAlimAtual(), _fluxoDU._oDSS));
        }

        // Conta cargas isoladas
        public int GetNumCargasIsoladas()
        {
            string[] _arrayCargasIsoladas = _fluxoDU._oDSS.GetActiveCircuit().Topology.AllIsolatedLoads;

            return _arrayCargasIsoladas.GetLength(0);
        }
    }
}

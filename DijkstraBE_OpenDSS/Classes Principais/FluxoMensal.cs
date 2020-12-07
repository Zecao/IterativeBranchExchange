//#define ENGINE
#if ENGINE
using OpenDSSengine;
#else
using dss_sharp;
#endif

using ExecutorOpenDSS.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutorOpenDSS.Classes_Principais
{
    class FluxoMensal
    {
        private FluxoDiario _fluxoDU;
        private FluxoDiario _fluxoSA;
        private FluxoDiario _fluxoDO;

        private ParamGeraisDSS _paramGerais; //TODO refactory uma vez que esta variavel ja e armazena na classe _FluxoDiario (de maneira semelhante do getObjDSS
        private MainWindow _janela;
        public ResultadoFluxo _resFluxoMensal;

        public FluxoMensal(ParamGeraisDSS paramGerais, MainWindow janela, ObjDSS oDSS)
        {
            // preenche variaveis da classe
            _paramGerais = paramGerais;
            _janela = janela;

            // fluxo dia util 
            _fluxoDU = new FluxoDiario(paramGerais, janela, oDSS, "DU");

            // fluxo sabado 
            _fluxoSA = new FluxoDiario(paramGerais, janela, oDSS, "SA");

            // fluxo domingo
            _fluxoDO = new FluxoDiario(paramGerais, janela, oDSS, "DO");

            // instancia obj resultado Mensal
            _resFluxoMensal = new ResultadoFluxo();
        }

        // ajusta modelo de carga
        public void AjustaModeloDeCargaCemig(int sentidoBusca)
        {
            // ajusta modelos de carga
            AjustaModeloDeCargaPvt(sentidoBusca);

            // ajusta taps RT
            AjustaTapsRTs(sentidoBusca);
        }

        //
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
                List<string> lstCommandsDSSModeloCarga = new List<string>();

                // modelo 100% P
                lstCommandsDSSModeloCarga.Add("new load.M2constZ pf = 0.92,Vminpu = 0.92,Vmaxpu = 1.5,model = 1,status = variable");
                lstCommandsDSSModeloCarga.Add("new load.M3constPsqQ pf = 0.92,Vminpu = 0.92,Vmaxpu = 1.5,model = 1,status = variable");

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
        internal ObjDSS getObjDSS()
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

            //Plota perdas na tela
            _janela.ExibeMsgDisplayMW(_resFluxoMensal.getResultadoFluxoToConsole(
                _paramGerais.GetTensaoSaidaSE(), _paramGerais.getNomeAlimAtual()));

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
                // calculo perdas
                _resFluxoMensal.setEnergiaPerdasFluxoSimples(_fluxoDU._resFluxo, _paramGerais);

                //Plota perdas na tela
                _janela.ExibeMsgDisplayMW(_resFluxoMensal.getResultadoFluxoToConsole(
                    _paramGerais.GetTensaoSaidaSE(), _paramGerais.getNomeAlimAtual()));
            }
            return ret;
        }

        // Conta cargas isoladas
        public int GetNumCargasIsoladas()
        {
            string[] _arrayCargasIsoladas = _fluxoDU._oDSS._DSSCircuit.Topology.AllIsolatedLoads;

            return _arrayCargasIsoladas.GetLength(0);
        }
    }
}

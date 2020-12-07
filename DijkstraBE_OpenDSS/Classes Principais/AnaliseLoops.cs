using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutorOpenDSS.Classes_Principais
{
    class AnaliseLoops
    {
        private MainWindow _janelaPrincipal;
        private ParamGeraisDSS _paramGerais;
        private List<string> _lstAlimentadores;
        private ObjDSS _oDSS;
        private FluxoDiario _fluxoSoMT;

        public AnaliseLoops(MainWindow janelaPrincipal, ParamGeraisDSS paramGerais, List<string> alimentadores, ObjDSS oDSS)
        {
            _janelaPrincipal = janelaPrincipal;
            this._paramGerais = paramGerais;
            this._lstAlimentadores = alimentadores;
            this._oDSS = oDSS;

            //Limpa Arquivos
            _paramGerais.deletaArqResultados();

            // analisa cada alimentador
            foreach (string nomeAlim in alimentadores)
            {
                AnaliseLoopsPvt(nomeAlim);
            }

            // Grava Log
            _janelaPrincipal.GravaLog();
        }

        private void AnaliseLoopsPvt(string nomeAlim)
        {
            // atribui nomeAlim
            _paramGerais.setNomeAlimAtual(nomeAlim);

            // Carrega arquivos DSS so MT
            _fluxoSoMT = new FluxoDiario(_paramGerais, _janelaPrincipal, _oDSS, true);

            // Carrega arquivos DSS
            if ( _fluxoSoMT.CarregaAlimentador() )
            {
                // SE executou fluxo snap
                if (_fluxoSoMT.ExecutaFluxoSnap() )
                {
                    // verifica cancelamento usuario 
                    if (_janelaPrincipal._cancelarExecucao)
                    {
                        return;
                    }

                    //
                    AnaliseLoops2();
                }            
            }
        }

        // Analise de Loops
        private void AnaliseLoops2()
        {
            // Obtem loops
            string[] loops = _oDSS._DSSCircuit.Topology.AllLoopedPairs;

            // armazena loops em lista
            List<string> lstLoops = new List<string>(loops);

            //Plota Looped Pairs 
            PlotaLoopedPairs(lstLoops);
        }

        //Plota niveis tensao nas barras dos trafos
        public void PlotaLoopedPairs(List<string> lstLoops)
        {
            // nome alim
            string nomeAlim = _paramGerais.getNomeAlimAtual();

            // linha 
            String linha = "";

            // para cada key value
            foreach (string loop in lstLoops)
            {
                // TODO tratar retirada \n ultima linha 
                linha += nomeAlim + "\t" + loop + "\n";
            }
            ArqManip.GravaEmArquivoAsync(linha, _paramGerais.getNomeCompArqLoops(), _janelaPrincipal);
        }
    }
}

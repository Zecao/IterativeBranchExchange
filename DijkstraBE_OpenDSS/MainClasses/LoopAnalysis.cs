using System;
using System.Collections.Generic;
using System.Linq;

namespace ExecutorOpenDSS.Classes_Principais
{
    class LoopAnalysis
    {
        private MainWindow _janelaPrincipal;
        private GeneralParameters _paramGerais;
        private readonly List<string> _lstAlimentadores;
        private ObjDSS _oDSS;
        private DailyFlow _fluxoSoMT;

        public LoopAnalysis(MainWindow janelaPrincipal, GeneralParameters paramGerais, List<string> alimentadores, ObjDSS oDSS)
        {
            _janelaPrincipal = janelaPrincipal;
            this._paramGerais = paramGerais;
            this._lstAlimentadores = alimentadores;
            this._oDSS = oDSS;

            //Limpa Arquivos
            _paramGerais.DeletaArqResultados();

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
            _paramGerais.SetNomeAlimAtual(nomeAlim);

            // Carrega arquivos DSS so MT
            _fluxoSoMT = new DailyFlow(_paramGerais, _janelaPrincipal, _oDSS, null, true);

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
            string[] loops = _oDSS.GetActiveCircuit().Topology.AllLoopedPairs;

            // armazena loops em lista
            List<string> lstLoops = new List<string>(loops);

            //Plota Looped Pairs 
            PlotaLoopedPairs(lstLoops);
        }

        //Plota niveis tensao nas barras dos trafos
        public void PlotaLoopedPairs(List<string> lstLoops)
        {
            // nome alim
            string nomeAlim = _paramGerais.GetNomeAlimAtual();

            // linha 
            String linha = "";

            // para cada key value
            foreach (string loop in lstLoops)
            {
                //armazena  nomeALim e loop
                linha += nomeAlim + "\t" + loop;

                //adiciona quebra de linha
                if (loop != lstLoops.Last())
                {
                    linha += "\n";
                }
            }
            TxtFile.GravaEmArquivoAsync(linha, _paramGerais.GetNomeCompArqLoops(), _janelaPrincipal);
        }
    }
}

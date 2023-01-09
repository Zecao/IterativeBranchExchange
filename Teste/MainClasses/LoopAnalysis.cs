using System;
using System.Collections.Generic;
using System.Linq;

namespace ExecutorOpenDSS.Classes_Principais
{
    class LoopAnalysis
    {
        private readonly GeneralParameters _paramGerais;
        private DailyFlow _fluxoSoMT;

        public LoopAnalysis(GeneralParameters paramGerais, List<string> alimentadores)
        {
            _paramGerais = paramGerais;

            //Limpa Arquivos
            _paramGerais.DeletaArqResultados();

            // analisa cada alimentador
            foreach (string nomeAlim in alimentadores)
            {
                AnaliseLoopsPvt(nomeAlim);
            }

            // Grava Log // TODO
            //paramGerais._mWindow.GravaLog();
        }

        private void AnaliseLoopsPvt(string nomeAlim)
        {
            // atribui nomeAlim
            _paramGerais.SetNomeAlimAtual(nomeAlim);

            // Carrega arquivos DSS so MT
            _fluxoSoMT = new DailyFlow(_paramGerais, "DU", true);

            // SE executou fluxo snap
            if (_fluxoSoMT.ExecutaFluxoSnap() )
            {
                // verifica cancelamento usuario 
                if (_paramGerais._mWindow._cancelarExecucao)
                {
                    return;
                }

                //
                AnaliseLoops2();
            }            

        }

        // Analise de Loops
        private void AnaliseLoops2()
        {
            // Obtem loops
            string[] loops = _fluxoSoMT._oDSS.GetActiveCircuit().Topology.AllLoopedPairs;

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
            TxtFile.GravaEmArquivo(linha, _paramGerais.GetNomeCompArqLoops(), _paramGerais._mWindow);
        }
    }
}

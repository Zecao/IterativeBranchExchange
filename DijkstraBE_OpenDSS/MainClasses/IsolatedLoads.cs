#define ENGINE
#if ENGINE
using OpenDSSengine;
#else
using dss_sharp;
#endif

using System;
using System.Collections.Generic;

namespace ExecutorOpenDSS.Classes_Principais
{
    class IsolatedLoads
    {
        private Circuit _circuit;
        private GeneralParameters _param;
        readonly List<string> _lstCargasIsoladas;

        // construtor
        public IsolatedLoads(Circuit cir, GeneralParameters paramGerais)
        {
            // preenche variaveis da classe
            _circuit = cir;
            _param = paramGerais;

            string[] _arrayCargasIsoladas = cir.Topology.AllIsolatedLoads; 
                       
            //
            _lstCargasIsoladas = new List<string>(_arrayCargasIsoladas);
        }

        //Plota niveis tensao nas barras dos trafos
        public void PlotaCargasIsoladasArq(MainWindow janela)
        {
            // se convergiu 
            if (_circuit.Solution.Converged)
            {
                // nome alim
                string nomeAlim = _param.GetNomeAlimAtual();

                // linha 
                String linha = "";

                // para cada key value
                foreach (string carga in _lstCargasIsoladas)
                {
                    // evita plotagem do null em _lstCargasIsoladas
                    if (carga== null)
                        break;

                    // TODO tratar retirada \n ultima linha 
                    linha += nomeAlim + "\t" + carga + "\n";
                }
                TxtFile.GravaEmArquivo2(linha, _param.GetNomeArqBarraTrafoLocal(), janela);
            }
        }
    }
}

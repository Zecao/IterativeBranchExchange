#define ENGINE
#if ENGINE
using OpenDSSengine;
#else
using dss_sharp;
#endif

using System.Collections.Generic;

namespace ExecutorOpenDSS.ClassesPrincipais
{
    class VoltageReguladorAnalysis
    {
        private Circuit _circuit;
        private Transformers _trafosDSS;
        private List<string> _tapsRT;
        private GeneralParameters _param;

        //constructor 
        public VoltageReguladorAnalysis(Text txt, Circuit cir, GeneralParameters paramGerais) 
        {
            _circuit = cir;
            _trafosDSS = cir.Transformers;
            //_DSSText = txt;
            _param = paramGerais;

            _tapsRT = new List<string>();
        }
        
        //Plota niveis tensao nas barras dos trafos
        public void PlotaTapRTs(MainWindow janela)
        {
            // se convergiu 
            if (_circuit.Solution.Converged)
            {
                // Calcula num Clientes com DRP e DRC 
                GetTapRTs();

                // Grava arquivo
                GravaTapRTsArq(janela);
            }           
        }

        // calcula tensao barra trafos 
        public void GetTapRTs()
        {
            int iTrafo = _trafosDSS.First;

            // para cada carga
            while ( iTrafo != 0  )
            {
                // nome trafo
                string trafoName = _trafosDSS.Name;

                //skipa banco de reguladores
                if (trafoName.Contains("rt"))
                {
                    //add
                    _tapsRT.Add(_param.GetNomeAlimAtual()+ "\t" + trafoName + "\t" + _trafosDSS.Tap );
                }          

                // itera
                iTrafo = _trafosDSS.Next;    
            }
        }

        // 
        public void GravaTapRTsArq(MainWindow janela)
        {
            TxtFile.GravaListArquivoTXT(_tapsRT, _param.GetNomeArqTapsRTs(), janela);
        }
    }
}

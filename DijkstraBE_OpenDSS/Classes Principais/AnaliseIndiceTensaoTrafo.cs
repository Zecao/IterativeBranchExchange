//#define ENGINE
#if ENGINE
using OpenDSSengine;
#else
using dss_sharp;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutorOpenDSS.Classes_Principais
{
    class AnaliseIndiceTensaoTrafo
    {
        private Circuit _circuit;
        private Transformers _trafosDSS;
        private Text _DSSText;
        private Dictionary<string, double> _nivelTensaoBarra;
        private ParamGeraisDSS _param;

        //construtor 
        public AnaliseIndiceTensaoTrafo(Text txt, Circuit cir , ParamGeraisDSS paramGerais) 
        {
            _circuit = cir;
            _trafosDSS = cir.Transformers;
            _DSSText = txt;
            _param = paramGerais;

            _nivelTensaoBarra = new Dictionary<string, double>();
        }
        
        //Plota niveis tensao nas barras dos trafos
        public void PlotaNiveisTensaoBarras(MainWindow janela)
        {
            // se convergiu 
            if (_circuit.Solution.Converged)
            {
                // Calcula num Clientes com DRP e DRC 
                CalcTensaoBarraTrafos();

                // Grava arquivo
                GravaTensaoBarraTrafos(janela);
            }           
        }

        // calcula tensao barra trafos 
        public void CalcTensaoBarraTrafos()
        {
            double nivelTensaoPU;

            int iTrafo = _trafosDSS.First;

            // para cada carga
            while ( iTrafo != 0  )
            {
                // nome trafo
                string trafoName = _trafosDSS.Name;
                
                // verifica nivel tensao
                nivelTensaoPU = getNivelTensaoBarraTrafo(trafoName);

                //add
                _nivelTensaoBarra.Add(trafoName, nivelTensaoPU);

                // itera
                iTrafo = _trafosDSS.Next;    
            }
        }

        // 
        public void GravaTensaoBarraTrafos(MainWindow janela)
        {
            // nome alim
            string nomeAlim = _param.getNomeAlimAtual();

            // linha 
            String linha = "";

            // para cada key value
            foreach (KeyValuePair<string, double> kvp in _nivelTensaoBarra)
	        {
                // TODO tratar retirada \n ultima linha 
                linha += nomeAlim + "\t" + kvp.Key + "\t" + kvp.Value.ToString() + "\n";
	        }
            //
            ArqManip.GravaEmArquivoAsync(linha, _param.getNomeArqBarraTrafo(), janela);
        }

        // verificaNivelTensaoBarra
        private double getNivelTensaoBarraTrafo(string trafoName)
        {
            // nome Barra
            string nomeBarra = getTrafoBusName(trafoName);

            // seta activebus a barra da carga 
            _circuit.SetActiveBus(nomeBarra);

            // obtem a barra, apos ativada
            Bus barraDSS = _circuit.ActiveBus;

            // kvbase
            double kVbase = barraDSS.kVBase;

            // array de tensoes
            double[] tensaoPU = barraDSS.puVoltages;
            
            return tensaoPU[0];
        }

        // obtem barra da carga por meio da interface de Text
        private string getTrafoBusName(string trafoName)
        {
            // get load bus name
            _DSSText.Command = "? transformer." + trafoName + ".Bus"; //Wdg=1

            // nome Barra
            string busName = _DSSText.Result;
     
            return busName;
        }
    }
}

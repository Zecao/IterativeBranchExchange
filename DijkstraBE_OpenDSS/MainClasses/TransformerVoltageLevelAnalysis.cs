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
    class TransformerVoltageLevelAnalysis
    {
        private Circuit _circuit;
        private Transformers _trafosDSS;
        private Text _DSSText;
        private Dictionary<string, double> _nivelTensaoBarra;
        private GeneralParameters _param;

        //construtor 
        public TransformerVoltageLevelAnalysis(Text txt, Circuit cir , GeneralParameters paramGerais) 
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

                //skipa banco de reguladores
                if (!trafoName.Contains("rt"))
                {
                    // verifica nivel tensao
                    nivelTensaoPU = GetNivelTensaoBarraTrafo(trafoName);

                    //add
                    _nivelTensaoBarra.Add(trafoName, nivelTensaoPU);
                }          

                // itera
                iTrafo = _trafosDSS.Next;    
            }
        }

        // 
        public void GravaTensaoBarraTrafos(MainWindow janela)
        {
            // nome alim
            string nomeAlim = _param.GetNomeAlimAtual();

            // linha 
            String linha = "";

            // para cada key value
            foreach (KeyValuePair<string, double> kvp in _nivelTensaoBarra)
	        {
                // TODO tratar retirada \n ultima linha 
                linha += nomeAlim + "\t" + kvp.Key + "\t" + kvp.Value.ToString() + "\n";
	        }
            TxtFile.GravaEmArquivo2(linha, _param.GetNomeArqBarraTrafo(), janela);
        }

        // verificaNivelTensaoBarra
        private double GetNivelTensaoBarraTrafo(string trafoName)
        {
            // nome Barra
            string nomeBarra = GetTrafoBusName(trafoName);

            // seta activebus a barra da carga 
            _circuit.SetActiveBus(nomeBarra);

            // obtem a barra, apos ativada
            Bus barraDSS = _circuit.ActiveBus;

            // kvbase
            double kVbase = barraDSS.kVBase;

            // array de tensoes
            //double[] tensaoPU = barraDSS.puVoltages;
            double[] tensaoPU = barraDSS.puVmagAngle;

            return tensaoPU[0];
        }

        // obtem barra da carga por meio da interface de Text
        private string GetTrafoBusName(string trafoName)
        {
            // OBS: codigo antigo que parou de funcionar
            //_DSSText.Command = "? transformer." + trafoName + ".Bus";

            // query buses do transformador
            _DSSText.Command = "? transformer." + trafoName + ".Buses"; //ok

            // nome Barra
            string busNames = _DSSText.Result;

            // separa barras
            string[] barras = busNames.Split(',');

            // remove o "[" no inicio da barra de MT
            string barraMT = barras[0].Remove(0, 1);

            return barraMT;
        }
    }
}

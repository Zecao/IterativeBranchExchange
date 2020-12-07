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
    class VoltageLevelAnalysis
    {
        private static int _numClientesDRP;
        private static int _numClientesDRC;
        private static int _numClientesOK;
        private static int _numBarras;
        private static int _numClientesTotal;
        private static int _numClientesIP;
        private Circuit _circuit;
        private Loads _loadsDSS;
        private Text _DSSText;

        public static List<string> _lstBarrasDRCeDRP = new List<string>();

        //construtor 
        public VoltageLevelAnalysis(Circuit cir, Text txt ) 
        {
            _circuit = cir;
            _loadsDSS = cir.Loads;
            _DSSText = txt;

            _numBarras = cir.NumBuses;
            _numClientesTotal = _loadsDSS.Count;
            _numClientesDRP = 0;
            _numClientesDRC = 0;
            _numClientesIP = 0;
        }

        // Cria arquivo texto cabecalho DRP DRC 
        public static void CriaArqCabecalho(GeneralParameters paramGerais, MainWindow janela)
        {                           
            // 
            string nomeArq = paramGerais.GetNomeComp_arquivoDRPDRC();

            //Grava cabecalho
            string linha = "Alim\tF.A.\tDRP:\tDRC:\tTotal:";
                
            //Grava em arquivo
            TxtFile.GravaEmArquivo(linha, nomeArq, janela);  
        }

        // obtem barra da carga por meio da interface de Text
        private string GetLoadBusName(string loadName)
        {
            // get load bus name
            _DSSText.Command = "? Load." + loadName + ".Bus1";

            // nome Barra
            string busName = _DSSText.Result;

            return busName;  
        }

        // verificaNivelTensaoBarra
        // OBS: necessario receber o kVcarga, uma vez que o kvBase da barra eh alterado com a mudanca nos 
        // taps dos transformadores
        private void VerificaNivelTensaoBarra(string nomeBarra, double kVcarga)
        {
            // seta activebus a barra da carga 
            _circuit.SetActiveBus(nomeBarra);

            /*// DEBUG
            if (nomeBarra.Equals("r1200553.1.2.0"))
            {
                int debug=0;
            }*/

            // obtem a barra, apos ativada
            Bus barraDSS = _circuit.ActiveBus;

            // array de tensoes
            double[] tensaoPU = barraDSS.VMagAngle;
            double tensaoFaseA = tensaoPU[0];

            // se tensao igual igual a 0, retorna
            if (tensaoFaseA == 0)
            {
                return;
            }

            // tensao fase A em pu
            double tensoaFaseApu = tensaoFaseA / (kVcarga * 1000 / Math.Sqrt(3));

            //BT OBS: assume que qquer tensao abaixo de 500Volts eh BT
            if (Math.Round(kVcarga, 3) <= 0.220)
            {
                VerificaNivelTensaoBarraBT(tensoaFaseApu, nomeBarra );
            }
            else
            {
                VerificaNivelTensaoBarraMT(tensoaFaseApu, nomeBarra);
            }                           
        }

        // verificaNivelTensaoBarraBT e incrementa contador de clientes
        private void VerificaNivelTensaoBarraBT(double tensaoPU, string nomeBarra)
        {
            //DRC para cada carga, checa o tensao
            if (tensaoPU < 0.8661)
            {
                _numClientesDRC++;
                _lstBarrasDRCeDRP.Add(nomeBarra + "\t" + tensaoPU.ToString());

                return;
            }
            //DRP para cada carga, checa o tensao
            if ((tensaoPU < 0.9213 ) && (tensaoPU >= 0.8661))
            {
                _numClientesDRP++;
                _lstBarrasDRCeDRP.Add(nomeBarra + "\t" + tensaoPU.ToString());
            }
        }

        // verificaNivelTensaoBarraMT e incrementa contador de clientes
        private void VerificaNivelTensaoBarraMT(double tensaoPU, string nomeBarra)
        {
            //DRC para cada carga, checa o tensao
            if (tensaoPU < 0.90)
            {
                _numClientesDRC++;

                _lstBarrasDRCeDRP.Add(nomeBarra + "\t" + tensaoPU.ToString());

                return;
            }

            //DRP para cada carga, checa o tensao
            if ((tensaoPU >= 0.90) && (tensaoPU < 0.93 ))
            {
                _numClientesDRP++;

                _lstBarrasDRCeDRP.Add(nomeBarra + "\t" + tensaoPU.ToString());
            }
        }

        // imprime numero de clientes DPRPDR
        private void ImprimeNumClientesDRPDRC(MainWindow janela)
        {
            // numero clientes DRP
            janela.ExibeMsgDisplayMW("Clientes com DRP: " + _numClientesDRP.ToString() );
            janela.ExibeMsgDisplayMW("Clientes com DRC: " + _numClientesDRC.ToString() );
            janela.ExibeMsgDisplayMW("Clientes totais: " + _numClientesTotal.ToString() );
        }

        // grava numero clientes com DRP e DRC no arquivo
        public void ImprimeNumClientesDRPDRC(GeneralParameters paramGerais, MainWindow _janela )
        {
            //nome arquivo DRP e DRC
            string nomeArq = paramGerais.GetNomeComp_arquivoDRPDRC();

            // nome alim
            string nomeAlim = paramGerais.GetNomeAlimAtual();
                
            // linha //ALim DRP DRC totais    
            string linha = nomeAlim + "\t" + _numClientesOK.ToString() + "\t" + _numClientesDRP.ToString() + "\t" + _numClientesDRC.ToString() + "\t" + _numClientesTotal.ToString();

            //Grava em arquivo
            TxtFile.GravaEmArquivo(linha, nomeArq, _janela);
        }

        // grava numero clientes com DRP e DRC no arquivo
        public void ImprimeBarrasDRPDRC(GeneralParameters paramGerais, MainWindow _janela)
        {
            //nome arquivo DRP e DRC
            string nomeArq = paramGerais.GetNomeComp_arqBarrasDRPDRC();

            // nome alim
            string nomeAlim = paramGerais.GetNomeAlimAtual();

            // linha  
            List<string> lstStr = new List<string>(); 
            
            //
            foreach (string barras in _lstBarrasDRCeDRP) 
            {
                lstStr.Add(nomeAlim + "\t" + barras);
            }

            //Grava em arquivo
            TxtFile.GravaListArquivoTXT(lstStr, nomeArq, _janela);
        }
        
        // ajusta numero de clientes totais, subtraindo os clientes de IP
        private void AjustaNumClientes()
        {
            _numClientesTotal = _numClientesTotal - _numClientesIP;      
        }

        // public
        // CalculaNumClientesDRPDRC
        public void CalculaNumClientesDRPDRC()
        {
            // nomeBarra
            string nomeBarra;

            // para cada carga
            for ( int i = 1; i <= _loadsDSS.Count; i++  )
            {
                /*
                ///DEBUG
                if (i == 48)
                {
                    int debug = 0;
                }*/

                // go to the load "i"
                _loadsDSS.idx = i;

                // obtem nome da carga
                string loadName = _loadsDSS.Name;
                double kVcarga = _loadsDSS.kV;

                // se load name comeca com LU = iluminacao publica
                if (loadName.Contains("ip"))
                {
                    // incrementa numero de clientes IP
                    _numClientesIP++;

                    continue;
                }

                // nome Barra
                nomeBarra = GetLoadBusName(loadName);

                // verifica nivel tensao
                VerificaNivelTensaoBarra(nomeBarra, kVcarga);
            }
            // ajusta numero de clientes
            AjustaNumClientes();

            // calcula numero de clientes faixa adequada
            CalculaNumClientesFaixaAdequada();
        }

        // calcula numclientes Faixa adequada
        private void CalculaNumClientesFaixaAdequada()
        {
            _numClientesOK = _numClientesTotal - _numClientesDRP - _numClientesDRC;
        }
    }
}

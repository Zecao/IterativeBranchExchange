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
    class AnaliseIndiceTensao
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

        List<string> _lstBarrasDRCeDRP = new List<string>();

        //construtor 
        public AnaliseIndiceTensao(Circuit cir, Text txt ) 
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
        public static void CriaArqCabecalho(ParamGeraisDSS paramGerais, MainWindow janela)
        {                           
            // 
            string nomeArq = paramGerais.getNomeComp_arquivoDRPDRC();

            //Grava cabecalho
            string linha = "Alim\tF.A.\tDRP:\tDRC:\tTotal:";
                
            //Grava em arquivo
            ArqManip.GravaEmArquivo(linha, nomeArq, janela);  
        }

        // obtem barra da carga por meio da interface de Text
        private string getLoadBusName(string loadName)
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
        private void verificaNivelTensaoBarra(string nomeBarra, double kVcarga)
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
                verificaNivelTensaoBarraBT(tensoaFaseApu, nomeBarra );
            }
            else
            {
                verificaNivelTensaoBarraMT(tensoaFaseApu, nomeBarra);
            }                           
        }

        // verificaNivelTensaoBarraBT e incrementa contador de clientes
        private void verificaNivelTensaoBarraBT(double tensaoPU, string nomeBarra)
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
        private void verificaNivelTensaoBarraMT(double tensaoPU, string nomeBarra)
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
        private void imprimeNumClientesDRPDRC(MainWindow janela)
        {
            // numero clientes DRP
            janela.ExibeMsgDisplayMW("Clientes com DRP: " + _numClientesDRP.ToString() );
            janela.ExibeMsgDisplayMW("Clientes com DRC: " + _numClientesDRC.ToString() );
            janela.ExibeMsgDisplayMW("Clientes totais: " + _numClientesTotal.ToString() );
        }

        // grava numero clientes com DRP e DRC no arquivo
        public void imprimeNumClientesDRPDRC(ParamGeraisDSS paramGerais, MainWindow _janela )
        {
            //nome arquivo DRP e DRC
            string nomeArq = paramGerais.getNomeComp_arquivoDRPDRC();

            // nome alim
            string nomeAlim = paramGerais.getNomeAlimAtual();
                
            // linha //ALim DRP DRC totais    
            string linha = nomeAlim + "\t" + _numClientesOK.ToString() + "\t" + _numClientesDRP.ToString() + "\t" + _numClientesDRC.ToString() + "\t" + _numClientesTotal.ToString();

            //Grava em arquivo
            ArqManip.GravaEmArquivo(linha, nomeArq, _janela);
        }

        // grava numero clientes com DRP e DRC no arquivo
        public void imprimeBarrasDRPDRC(ParamGeraisDSS paramGerais, MainWindow _janela)
        {
            //nome arquivo DRP e DRC
            string nomeArq = paramGerais.getNomeComp_arqBarrasDRPDRC();

            // nome alim
            string nomeAlim = paramGerais.getNomeAlimAtual();

            // linha  
            List<string> lstStr = new List<string>(); 
            
            //
            foreach (string barras in _lstBarrasDRCeDRP) 
            {
                lstStr.Add(nomeAlim + "\t" + barras);
            }

            //Grava em arquivo
            ArqManip.GravaListArquivoTXT(lstStr, nomeArq, _janela);
        }
        
        /*// 
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
            ArqManip.GravaEmArquivoTensaoBarraTrafos(linha, _param.getNomeArqBarraTrafo(), janela);
        } */


        // ajusta numero de clientes totais, subtraindo os clientes de IP
        private void ajustaNumClientes()
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
                ///DEBUG
                if (i == 48)
                {
                    int debug = 0;
                }

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
                nomeBarra = /*getLoadBusName*/(loadName);

                /*
                // DEBUG
                if (nomeBarra.Equals("r1200553.1.2.0"))
                {
                    int debug = 0;
                }*/

                // verifica nivel tensao
                verificaNivelTensaoBarra(nomeBarra, kVcarga);
            }
            // ajusta numero de clientes
            ajustaNumClientes();

            // calcula numero de clientes faixa adequada
            calculaNumClientesFaixaAdequada();
        }

        // calcula numclientes Faixa adequada
        private void calculaNumClientesFaixaAdequada()
        {
            _numClientesOK = _numClientesTotal - _numClientesDRP - _numClientesDRC;
        }
    }
}

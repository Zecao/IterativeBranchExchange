//#define ENGINE
#if ENGINE
using OpenDSSengine;
#else
using dss_sharp;
#endif

namespace ExecutorOpenDSS.Classes_Principais
{
    class Switch
    {
        public string nome;
        public string bus1;
        public string bus2;
        public int phases;
        public bool _estaAberta; // true = aberto

        public bool chaveOtimizada;
        public double perda_kWh;
        internal double energiaForn_kWh;
        internal double perdaPercentual;
        internal double redPerda_kWh;

        public Switch(Circuit dSSCircuit)
        {
            //variavel chave
            nome = dSSCircuit.Lines.Name;

            /*
            // DEBUG 
            if (nome.Equals("ctr72314"))
            {
                int debug=0;
            }*/

            phases = dSSCircuit.Lines.Phases;
            bus1 = FiltraBarrasStr(dSSCircuit.Lines.Bus1);
            bus2 = FiltraBarrasStr(dSSCircuit.Lines.Bus2);

            //preenche status aberto fechado
            SetStatusAberto(dSSCircuit);
        }

        //
        private string FiltraBarrasStr(string barra)
        {
            if (phases == 3)
            {
                barra = barra.Replace(".1.2.3", "");
            }
            return barra;
        }

        // TODO
        public Switch()
        {
            //variavel chave
            nome = "null";
            bus1 = "null";
            bus2 = "null";
            phases = 0;
        }

        // obtem status da chave 
        private void SetStatusAberto(Circuit dSSCircuit)
        {
            // documentacao
            // https://sourceforge.net/p/electricdss/discussion/861976/thread/6c0c5113/

            //%Here the sw1 is set as active element
            dSSCircuit.SetActiveElement("Line." + nome);

            //%Here we ask if the sw1 is open in the term 2
            _estaAberta = dSSCircuit.get_CktElements("Line." + nome).IsOpen(1, 0);
        }

        // OLD CODE altered to direct access to member 
        // obtem status da chave 
        public bool GetStatusAberto(Circuit dSSCircuit)
        {
            //%Here the sw1 is set as active element
            dSSCircuit.SetActiveElement("Line." + nome);

            //%Here we ask if the sw1 is open in the term 2
            bool statusAberto = dSSCircuit.get_CktElements("Line." + nome).IsOpen(1, 0);

            return statusAberto;
        }

        // OBS: altered to direct access to member 
        // TODO debugar
        // verifica se eh linha chave
        public static bool IsChave(Circuit dSSCircuit, string aresta)
        {
            // verifies if its line (as it can also receive transformer/voltage regulator)
            if (aresta.Contains("Line"))
            {
                // 
                dSSCircuit.SetActiveElement(aresta);

                // #if ENGINE
                //bool ehChave = true;
                bool ehChave = dSSCircuit.Lines.IsSwitch;

                return ehChave;
            }
            return false;
        }

        // abre chave
        public void Abre(Text dssText, string nomeCompChave)
        {
            // open line.CTRR30535 term=1
            dssText.Command = "open " + nomeCompChave + " term=1";

            // abre objeto
            _estaAberta = true;
        }

        // fecha chaveNF
        public void FechaPorNomeComp(Text dssText, string nomeCompChave)
        {
            // open line.CTRR30535 term=1
            dssText.Command = "close " + nomeCompChave + " term=1";

            // abre objeto
            _estaAberta = false;
        }

        // fecha chave
        public void Fecha(Text dssText, string nomeChave)
        {
            // open line.CTRR30535 term=1
            dssText.Command = "close line." + nomeChave + " term=1";

            // fecha objeto
            _estaAberta = false;
        }
        
        // nome composto adicionando prefixo do OpenDSS
        public string GetNomeCompostoChave()
        {
            //Adiciona prefixo do OpenDSS
            return "Line." + nome;
        }
    }
}

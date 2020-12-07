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
    class ObjDSS
    {
        public DSS _DSSObj;
        public Text _DSSText;
        public Circuit _DSSCircuit; 
        public ParamGeraisDSS _paramGerais;

        public ObjDSS(ParamGeraisDSS par)
        {
            //
            _paramGerais = par;

            //
            inicializaServDSS();
        }

        // inicializa serv DSS
        private void inicializaServDSS()
        {
            //Inicializa o servidor COM
            _DSSObj = new DSS();

            // Inicializa servidor COM
            _DSSObj.Start(0);

            // configuracoes gerais OpenDSS
            _DSSObj.AllowForms = _paramGerais._parGUI._allowForms;

            // preenche circuitos
            _DSSCircuit = _DSSObj.ActiveCircuit;

            // interface texto
            _DSSText = _DSSObj.Text;
        }

        public void SetActiveCircuit()
        {
            // preenche circuitos
            _DSSCircuit = _DSSObj.ActiveCircuit;
        }
    }
}

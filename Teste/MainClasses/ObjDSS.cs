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
        public GeneralParameters _paramGerais;

        public ObjDSS(GeneralParameters par)
        {
            //
            _paramGerais = par;

            //
            InicializaServDSS();
        }

        // retorna o DSSCircuit 
        public Circuit GetActiveCircuit()
        {
            return _DSSObj.ActiveCircuit;
        }

        // inicializa serv DSS
        private void InicializaServDSS()
        {
            //Inicializa o servidor COM
            _DSSObj = new DSS();

            // Inicializa servidor COM
            _DSSObj.Start(0);

            /* TODO dss_sharp.DSSException: 'Cannot activate output with no console available! If you want to use a message output callback, register it before enabling AllowForms.'
            // configuracoes gerais OpenDSS
            _DSSObj.AllowForms = _paramGerais._parGUI._allowForms;
            */

            // interface texto
            _DSSText = _DSSObj.Text;
        }
    }
}

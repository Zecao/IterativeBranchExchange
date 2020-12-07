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
    class OperacoesDSS
    {
        //computa cargas dos alimentador
        public static double getSomaCargasAlim(DSS DSSObj)
        {
            // Load-Interface
            Loads DSSLoads = DSSObj.ActiveCircuit.Loads;

            //sets first element to be active
            int f = DSSLoads.First;

            //pega primeira carga
            double somakW = DSSLoads.kW;

            //
            while (DSSLoads.Next != 0)
            {
                //
                double kW = DSSLoads.kW;

                //
                somakW = somakW + kW;
            }
            return somakW;
        }

        /*
        //get a soma das cargas calculadas apos a execucao do fluxo
        public static double getSomaCargasCalculadas(DSS DSSObj)
        {
            Loads DSSLoads = DSSObj.ActiveCircuit.Loads;

            Bus DSSBus = DSSObj.ActiveCircuit.ActiveBus;

            //
            while (DSSLoads.Next != 0)
            {
                //
                double kW = DSSLoads.kW;

                //
                somakW = somakW + kW;
            }
        }*/

    }
}

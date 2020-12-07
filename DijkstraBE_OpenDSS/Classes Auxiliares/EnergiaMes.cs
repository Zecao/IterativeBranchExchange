using ExecutorOpenDSS.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutorOpenDSS.Classes_Auxiliares
{
    class EnergiaMes
    {
        // map (mes,alim) X alim       
        public Dictionary<string, double> _mapDadosEnergiaMes = new Dictionary<string, double>();

        //Tradução da função carregaMapDemandaMaxAlim
        public void carregaMapEnergiaMesAlim(ParamGeraisDSS par)
        {
            string nomeArqEnergiaCompl = par._parGUI._pathRecursosPerm + par._arqEnergia;

            string[,] energiaMes = LeXLSX.LeTudo(nomeArqEnergiaCompl);

            // para cada alim
            // OBS: nAlim comeca em 1 por causa do cabecalho
            for (int nAlim = 1; nAlim < energiaMes.GetLength(0); nAlim ++  )
            {
                // alim 
                string alim = energiaMes[nAlim,0];

                // carrega requisitos para todos os meses
                for (int mes = 1; mes < 13; mes++)
                {
                    double energia = double.Parse(energiaMes[nAlim,mes]);

                    // chave tmp
                    string chave = mes.ToString() + "_" + alim;

                    //adiciona na variavel da classe
                    _mapDadosEnergiaMes.Add(chave, energia);
                }
            }
        }

        // obtem a referencia de energia para um dado mes
        public double getRefEnergia(string nomeAlim, int mes)
        {
            // chave tmp
            string chave = mes.ToString() + "_" + nomeAlim;

            //verifica se nome 
            if (_mapDadosEnergiaMes.ContainsKey(chave))
            {
                return _mapDadosEnergiaMes[chave];
            }

            return double.NaN;
        }
    }
}

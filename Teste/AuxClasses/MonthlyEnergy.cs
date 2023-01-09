using System.Collections.Generic;

namespace ExecutorOpenDSS.Classes_Auxiliares
{
    class MonthlyEnergy
    {
        public GeneralParameters _paramGeraisDSS;

        // map (mes,alim) X alim       
        public Dictionary<string, double> _mapDadosEnergiaMes = new Dictionary<string, double>();

        public MonthlyEnergy(GeneralParameters par)
        {
            _paramGeraisDSS = par;
        }

        // Load map from Excel file with month energy measurements 
        public void CarregaMapEnergiaMesAlim()
        {
            string nomeArqEnergiaCompl = _paramGeraisDSS._parGUI._pathRecursosPerm + _paramGeraisDSS._arqEnergia;

            string[,] energiaMes = XLSXFile.LeTudo(nomeArqEnergiaCompl);

            // para cada alim
            // OBS: nAlim comeca em 1 por causa do cabecalho
            for (int nAlim = 1; nAlim < energiaMes.GetLength(0); nAlim ++  )
            {
                // alim 
                string alim = energiaMes[nAlim,0];

                // DEBUG
                /*
                if (alim.Equals("LAVD18"))
                {
                    int debug = 0;
                }*/

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
        public double GetRefEnergia(string nomeAlim, int mes)
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

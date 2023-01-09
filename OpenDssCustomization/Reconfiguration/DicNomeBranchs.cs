using System.Collections.Generic;
using System.Linq;

namespace ExecutorOpenDSS.Reconfigurador
{
    class DicNomeBranchs
    {
        private readonly string _Rows = "_Inc_Matrix_Rows.csv";
        private readonly string _prefix = "alim"; //TODO FIX ME

        // nome Composto (i.e. nome OpenDSS) para Indice 
        public Dictionary<string, string> _mapNomeBranchsXIndice = new Dictionary<string, string>();

        // map Indice (comecando em 0) para nome do branch
        private readonly Dictionary<string, string> _mapIndiceXNomeBranchs = new Dictionary<string, string>();

        // preenche map nome Branchs 
        public void PreencheMapNomeBranchs(string dir)
        {
            //
            string nomeArqBranchs = _prefix + dir + _Rows;

            // Le uma coluna
            List<string> nomeBranchs = XLSXFile.Le1ColunaCSV(nomeArqBranchs);

            /* // DEBUG
            int debug = nomeBranchs.Count;
            */

            // OBS: comeca a contar de 1, porque 0 eh o cabecalho (nome da coluna) exportado pelo OpenDSS
            for (int i = 1; i < nomeBranchs.Count(); i++)
            {
                // OBS: subtraio 1 do indice "i", uma vez que a matriz exportada pelo openDSS, o primeiro branch eh "0" 
                _mapNomeBranchsXIndice.Add(nomeBranchs[i],      (i - 1).ToString());
                _mapIndiceXNomeBranchs.Add((i - 1).ToString(), nomeBranchs[i]);
            }
        }

        // retorna NomeBranch por INdice
        public string GetNomeBranchByIndice(string indice)
        {
            if (_mapIndiceXNomeBranchs.ContainsKey(indice))
                return _mapIndiceXNomeBranchs[indice];
            else
                return null;
        }
    }
}

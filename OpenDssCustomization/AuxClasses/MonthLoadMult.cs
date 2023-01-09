using System.Collections.Generic;

namespace ExecutorOpenDSS.Classes_Auxiliares
{
    class MonthLoadMult
    {
        // map energia injetada X alim
        public Dictionary<int, Dictionary<string, double>> _mapAlimLoadMult = new Dictionary<int, Dictionary<string, double>>();
        private readonly GeneralParameters _paramGerais;

        // construtor
        public MonthLoadMult(GeneralParameters par)
        {
            _paramGerais = par;

            CarregaMapAjusteLoadMult();
        }

        public void CarregaMapAjusteLoadMult()
        {
            // carrega requisitos para todos os meses
            for (int mes = 1; mes < 13; mes++)
            {
                //obtem nome ajuste compl
                string arqAjusteCompl = _paramGerais.GetNomeArqAjuste(mes);

                //adiciona na variavel da classe
                _mapAlimLoadMult.Add(mes, XLSXFile.XLSX2Dictionary(arqAjusteCompl));
            }
        }

        // escreve map alim loadMult com novos loadMults
        public void AtualizaMapAlimLoadMult(string alim, double loadMult, int mes)
        {
            //atualiza alimentador
            _mapAlimLoadMult[mes][alim] = loadMult;
        }

        //
        internal double GetLoadMult()
        {
            // mes
            int mes = _paramGerais._parGUI.GetMes();
            
            //alim
            string alimTmp = _paramGerais.GetNomeAlimAtual();

            if (_mapAlimLoadMult[mes].ContainsKey(alimTmp))
            {
                return _mapAlimLoadMult[mes][alimTmp];
            }
            return double.NaN;
        }
    }
}

using System;
using System.Xml.Linq;

namespace ExecutorOpenDSS.Classes_Auxiliares
{
    public class AdvancedParameters
    {
        public bool _otimizaPUSaidaSE = false; // otimizacao PU saida SE
        public bool _calcDRPDRC = false;
        public bool _calcTensaoBarTrafo = false;
        public bool _verifCargaIsolada = false;
        public bool _incluirCapMT = false;
        public bool _modeloCargaCemig = false;
        public bool _verifTapsRTs = false;
        public string _strBatchEdit = ""; //batchEdit opendDss style string

        public AdvancedParameters(Interfaces.Options jan )
        {
            // parametros avancados
            _otimizaPUSaidaSE = jan.calculaPUOtm.IsChecked.Value;
            _calcDRPDRC = jan.calculaDRPDRCCheckBox.IsChecked.Value;
            _calcTensaoBarTrafo = jan.calcTensaoBarTrafoCheckBox.IsChecked.Value;
            _verifCargaIsolada = jan.verifCargaIsolada.IsChecked.Value;
            _incluirCapMT = jan.IncluiCapMTCheckBox.IsChecked.Value;
            _modeloCargaCemig = jan.ModeloCargaCemig.IsChecked.Value;
            _verifTapsRTs = jan.verifTapsRTs.IsChecked.Value;
            _strBatchEdit = jan.TBBatchEdit.Text;
        }

        // construtor baseado em XML
        public AdvancedParameters(XElement raiz)
        {
            _calcDRPDRC = Convert.ToBoolean(raiz.Element("CalculaDRPDRC").Value);
            _otimizaPUSaidaSE = Convert.ToBoolean(raiz.Element("CalculaPUOtm").Value);
            _calcTensaoBarTrafo = Convert.ToBoolean(raiz.Element("CalcTensaoBarTrafo").Value);
            _verifCargaIsolada = Convert.ToBoolean(raiz.Element("VerifCargaIsolada").Value);
            _incluirCapMT = Convert.ToBoolean(raiz.Element("IncluirCapMT").Value);
            _modeloCargaCemig = Convert.ToBoolean(raiz.Element("ModeloCargaCemig").Value);
            _verifTapsRTs = Convert.ToBoolean(raiz.Element("RelatorioTapsRTs").Value);
            _strBatchEdit = raiz.Element("StringBatchEdit").Value;
        }

        // construtor default
        public AdvancedParameters()
        {
        }
    }
}

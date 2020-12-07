using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ExecutorOpenDSS.Classes;

namespace ExecutorOpenDSS.Classes_Auxiliares
{
    public class ParamAvancados
    {
        public bool _otimizaPUSaidaSE = false; // otimizacao PU saida SE
        public bool _calcDRPDRC = false;
        public bool _calcTensaoBarTrafo = false;
        public bool _verifCargaIsolada = false;
        public bool _incluirCapMT = false;
        public bool _modeloCargaCemig = false;

        public ParamAvancados(Interfaces.OpcoesAvancadas jan )
        {
            // parametros avancados
            _otimizaPUSaidaSE = jan.calculaPUOtm.IsChecked.Value;
            _calcDRPDRC = jan.calculaDRPDRCCheckBox.IsChecked.Value;
            _calcTensaoBarTrafo = jan.calcTensaoBarTrafoCheckBox.IsChecked.Value;
            _verifCargaIsolada = jan.verifCargaIsolada.IsChecked.Value;
            _incluirCapMT = jan.IncluiCapMTCheckBox.IsChecked.Value;
            _modeloCargaCemig = jan.ModeloCargaCemig.IsChecked.Value;
        }

        // construtor baseado em XML
        public ParamAvancados(XElement raiz)
        {
            _calcDRPDRC = Convert.ToBoolean(raiz.Element("CalculaDRPDRC").Value);
            _otimizaPUSaidaSE = Convert.ToBoolean(raiz.Element("CalculaPUOtm").Value);
            _calcTensaoBarTrafo = Convert.ToBoolean(raiz.Element("CalcTensaoBarTrafo").Value);
            _verifCargaIsolada = Convert.ToBoolean(raiz.Element("VerifCargaIsolada").Value);
            _incluirCapMT = Convert.ToBoolean(raiz.Element("IncluirCapMT").Value);
            _modeloCargaCemig = Convert.ToBoolean(raiz.Element("ModeloCargaCemig").Value);
        }

        // construtor default
        public ParamAvancados()
        {
        }
    }
}

using System;
using System.Xml.Linq;

namespace ExecutorOpenDSS.Classes_Auxiliares
{
    public class ExpanderParameters
    {
        public bool _otimizaPUSaidaSE = false; // otimizacao PU saida SE
        public bool _calcDRPDRC = false;
        public bool _calcTensaoBarTrafo = false;
        public bool _verifCargaIsolada = false;
        public bool _incluirCapMT = false;
        public bool _modeloCargaCemig = false;
        public bool _verifTapsRTs = false;
        public string _strBatchEdit = ""; //batchEdit opendDss style string
        public bool _allowForms = false;

        public ExpanderParameters(MainWindow jan)
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
            _allowForms = jan.AllowFormsCheckBox.IsChecked.Value;
        }

        // construtor baseado em XML
        public ExpanderParameters(XElement raiz, MainWindow jan)
        {
            _calcDRPDRC = Convert.ToBoolean(raiz.Element("CalculaDRPDRC").Value);
            _otimizaPUSaidaSE = Convert.ToBoolean(raiz.Element("CalculaPUOtm").Value);
            _calcTensaoBarTrafo = Convert.ToBoolean(raiz.Element("CalcTensaoBarTrafo").Value);
            _verifCargaIsolada = Convert.ToBoolean(raiz.Element("VerifCargaIsolada").Value);
            _incluirCapMT = Convert.ToBoolean(raiz.Element("IncluirCapMT").Value);
            _modeloCargaCemig = Convert.ToBoolean(raiz.Element("ModeloCargaCemig").Value);
            _verifTapsRTs = Convert.ToBoolean(raiz.Element("RelatorioTapsRTs").Value);
            _strBatchEdit = raiz.Element("StringBatchEdit").Value;
            _allowForms = Convert.ToBoolean(raiz.Element("AllowForms").Value);

            // atualiza interface grafica 
            Data2GUI(jan);
        }

        internal void Data2GUI(MainWindow janela)
        {
            janela.calculaPUOtm.IsChecked = _calcDRPDRC;
            janela.calculaDRPDRCCheckBox.IsChecked = _otimizaPUSaidaSE;
            janela.calcTensaoBarTrafoCheckBox.IsChecked = _calcTensaoBarTrafo;
            janela.verifCargaIsolada.IsChecked = _verifCargaIsolada; 
            janela.IncluiCapMTCheckBox.IsChecked = _incluirCapMT;
            janela.ModeloCargaCemig.IsChecked = _modeloCargaCemig;
            janela.verifTapsRTs.IsChecked = _modeloCargaCemig;
            janela.TBBatchEdit.Text = _strBatchEdit;
            janela.AllowFormsCheckBox.IsChecked = _allowForms;
        }
    }
}

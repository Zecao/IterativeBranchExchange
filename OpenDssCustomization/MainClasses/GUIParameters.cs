using ExecutorOpenDSS.Classes_Auxiliares;
using System;
using System.Globalization;

namespace ExecutorOpenDSS.Classes
{
    // Classe que armazena parametros (do usuario) de otmizacao do fluxo de potencia e seus resp. metodos. 
    public class GUIParameters
    {
        //variaveis 
        public string _hora;
        public string _ano = "2021";
        public string _mes;
        public string _tipoDia;
        private int _mesNum;
        public string _tipoFluxo;
        public bool _allowForms;
        public string _pathRecursosPerm;
        public string _pathRaizGUI;
        public bool _usarTensoesBarramento;
        public string _tensaoSaidaBarUsuario;
        public bool _otmPorDemMax = false; // 
        public bool _otmPorEnergia = false; // booleanda que seta o tipo de otimizacao (Potencia ou Energia)
        private readonly double _precisao = 500; // Precisao % do ajuste
        private float _incremento = 1.05F; // Passo % do incremento
        private bool _aproximaFluxoMensalPorDU = false; //booleana que seta decide o tipo de fluxo Mensal
        public bool _modoAnual = false;
        public bool _modoHorario = false;
        public double _loadMultAlternativo; // loadMult alternativo inserido pelo usuário

        // variaveis temporaria nao presentes na GUI
        public double loadMultDefault = 1; // LoadMult retornado qnd alim nao eh encontrado no arquivo de ajuste.
        public double loadMultAtual = 1;
        public string _mesAbrv3letras;
        private readonly string _lstAlim = "lstAlimentadores.m"; // TODO 

        // expander parameters
        public ExpanderParameters _expanderPar;

        // set Mes 
        internal void SetMes(int mes)
        {
            _mesNum = mes;

            //atualiza abreviatua mes
            _mesAbrv3letras = TipoDiasMes.GetMesAbrv(mes);
        }

        internal int GetMes()
        {
            return _mesNum;
        }

        // get arquivo lista Alimentadores 
        public string GetArqLstAlimentadores()
        { 
            return _pathRecursosPerm + _lstAlim; 
        }

        // Utilizado por feriadosWindow
        public string GetNomeArquivoFeriadosCompleto()
        {
            return _pathRecursosPerm + "Feriados" + _ano + ".txt";
        }

        // 
        public void CopiaVariaveis(MainWindow jan)
        {
            //Armazena os valores da interface
            _hora = jan.horaTextBox.Text;
            _ano = jan.anoTextBox.Text;
            _mes = jan.mesComboBox.Text;

            _tipoDia = jan.tipoDiaComboBox.Text;
            _mesNum = jan.mesComboBox.SelectedIndex + 1;
            _mesAbrv3letras = _mes.Substring(0, 3); //OBS: Alternativa _mesAbrv3letras = TipoDiasMes.getMesAbrv(_mesNum);
            _tipoFluxo = jan.tipoFluxoComboBox.Text;
            _pathRecursosPerm = jan.caminhoPermTextBox.Text;
            _pathRaizGUI = jan.caminhoDSSTextBox.Text;
            _usarTensoesBarramento = jan.usarTensoesBarramentoCheckBox.IsChecked.Value;
            _tensaoSaidaBarUsuario = jan.tensaoSaidaBarTextBox.Text;
            _otmPorDemMax = jan.otimizaCheckBox.IsChecked.Value;
            _otmPorEnergia = jan.otimizaEnergiaCheckBox.IsChecked.Value;
            _aproximaFluxoMensalPorDU = jan.simplificaMesComDUCheckBox.IsChecked.Value;

            // preenche incremento
            SetIncremento(jan.incrementoAjusteTextBox.Text);

            // transforma texto para double
            _loadMultAlternativo = Double.Parse(jan.loadMultAltTextBox.Text);

            //
            _expanderPar = new ExpanderParameters(jan);
        }

        public void SetIncremento(string s)
        {
            float i = float.Parse(s, CultureInfo.InvariantCulture);
            _incremento = i / 100 + 1;
        }

        public float GetIncremento()
        {
            return _incremento;
        }

        internal double GetPrecisao()
        {
            return _precisao;
        }

        internal bool GetAproximaFluxoMensalPorDU()
        {
            return _aproximaFluxoMensalPorDU;
        }

        // traducao do tipo do dia da combobox para o codigo string necessario
        public void PreencheTipoDia()
        {
            switch (_tipoDia)
            {
                case "Dia útil":
                    _tipoDia = "DU";
                    break;
                case "Domingo":
                    _tipoDia = "DO";
                    break;
                case "Sábado":
                    _tipoDia = "SA";
                    break;
            }
        }
    }
}

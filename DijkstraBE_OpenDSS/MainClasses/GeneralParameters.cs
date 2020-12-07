using ExecutorOpenDSS.Classes;
using ExecutorOpenDSS.Classes_Auxiliares;
using System.Collections.Generic;
using System.IO;

namespace ExecutorOpenDSS
{
    public class GeneralParameters
    {
        // nome alim atual
        private string _nomeAlimAtual;

        //
        internal TipoDiasMes _objTipoDeDiasDoMes;

        // arquivos resultados
        public string _nomeArqCurvasDeCarga = "CurvasDeCarga";
        private readonly string _arquivoResPerdasHorario = "perdasAlimHorario.txt";
        private readonly string _arquivoResPerdasDiario = "perdasAlimDiario.txt";
        private readonly string _arquivoResPerdasMensal = "perdasAlimMensal.txt";
        private readonly string _arquivoResPerdasAnual = "perdasAlimAnual.txt";
        private readonly string _arquivoResAlimNaoConvergiram = "AlimentadoresNaoConvergentes.txt";
        private readonly string _arquivoDRPDRC = "AlimentadoresDRPDRC.txt";
        private readonly string _arqBarrasDRPDRC = "BarrasDRPDRC.txt";
        private readonly string  _arqBarraTrafo = "BarrasTrafo.txt";
        private readonly string  _arqTapsRTs = "TapsRTs.txt";
        public string _arqDemandaMaxAlim = "demandaMaxAlim_" + "Jan" + ".xlsx";
        public string _arqTensoesBarramento = "tensoesBarramento.xlsx";
        private readonly string _arqLoops = "loops.txt";
        public string _arqEnergia = "energiaMesAlim.xlsx";
        private readonly string _arqCargaIsoladas = "CargasIsoladas.txt";

        //caminhos
        private readonly string _pathCurvasTxt;

        // tipo fluxo Normal
        public string _AlgoritmoFluxo;

        // paramentros da interface GUI
        internal GUIParameters _parGUI;
        internal AdvancedParameters _parAvan;

        public MainWindow _janelaPrincipal;

        // construtor
        public GeneralParameters(MainWindow janelaPrincipal)
        {
            _janelaPrincipal = janelaPrincipal;

            //Paramentros Otimizacao.
            _parGUI = janelaPrincipal._parGUI;

            //Caminho curvas TXT 
            _pathCurvasTxt = _parGUI._pathRecursosPerm + "NovasCurvasTxt\\";

            //Parametros Avancados
            _parAvan = janelaPrincipal._parAvan;

            // preenche variavel _objTipoDeDiasDoMes
            _objTipoDeDiasDoMes = new TipoDiasMes(_parGUI, janelaPrincipal);

            // tipo fluxo
            _AlgoritmoFluxo = "Normal";
        }

        // 
        internal string GetNomeArqBarraTrafoLocal()
        {
            return _parGUI._pathRecursosPerm + _arqCargaIsoladas;
        }

        internal string GetNomeCompArqLoops()
        {
            return _parGUI._pathRecursosPerm + _arqLoops;
        }

        // grava _mapAlimLoadMult no arquivo excel
        internal void GravaMapAlimLoadMultExcel()
        {
            //Novo arquivo de ajuste
            string arqAjuste = "Ajuste_" + _parGUI._mesAbrv3letras + ".xlsx";

            //Re-escreve xls de ajuste
            TxtFile.SafeDelete(_parGUI._pathRecursosPerm + arqAjuste);

            var file = new FileInfo(_parGUI._pathRecursosPerm + arqAjuste);

            //
            int mes = _parGUI.GetMes();

            // 
            Dictionary<string, double> mapAlimLoadMult = _janelaPrincipal._medAlim._reqLoadMultMes._mapAlimLoadMult[mes];

            // Grava arquivo Excel
            XLSXFile.GravaDictionaryExcel(file, mapAlimLoadMult);
        }

        // obtem o nome do arquivo de perdas
        internal string GetNomeArquivoPerdas()
        {
            string tipoFluxo = _parGUI._tipoFluxo;

            string arquivo = null;

            switch (tipoFluxo)
            {
                case "Snap":

                    arquivo = _parGUI._pathRecursosPerm + _arquivoResPerdasHorario;

                    break;

                case "Daily":

                    arquivo = _parGUI._pathRecursosPerm + _arquivoResPerdasDiario;

                    break;

                default:
                    arquivo = _parGUI._pathRecursosPerm + _arquivoResPerdasDiario;

                    break;
            }
            return arquivo;
        }

        // Deleta Arquivos Resultados
        internal void DeletaArqResultados()
        {           
            TxtFile.SafeDelete( GetNomeComp_arquivoResAlimNaoConvergiram() );
            TxtFile.SafeDelete( GetNomeComp_arquivoResPerdasHorario());
            TxtFile.SafeDelete( GetNomeComp_arquivoResPerdasDiario());
            TxtFile.SafeDelete( GetNomeComp_arquivoResPerdasMensal());
            TxtFile.SafeDelete( GetNomeComp_arquivoResPerdasAnual());

            TxtFile.SafeDelete(GetNomeComp_arqBarrasDRPDRC());
            TxtFile.SafeDelete(GetNomeComp_arquivoDRPDRC());
            TxtFile.SafeDelete(GetNomeArqBarraTrafo());
            TxtFile.SafeDelete(GetNomeCompArqLoops());
            TxtFile.SafeDelete(GetNomeArqTapsRTs());

            TxtFile.SafeDelete(GetArqRmatrix());
            TxtFile.SafeDelete(GetArqXmatrix());
        }

        public string GetNomeComp_arquivoResPerdasAnual()
        {
            return _parGUI._pathRecursosPerm + _arquivoResPerdasAnual;
        }

        public string GetNomeComp_arquivoResPerdasMensal()
        {
            return _parGUI._pathRecursosPerm + _arquivoResPerdasMensal;
        }

        private string GetNomeComp_arquivoResPerdasDiario()
        {
            return _parGUI._pathRecursosPerm + _arquivoResPerdasDiario;
        }

        private string GetNomeComp_arquivoResPerdasHorario()
        {
            return _parGUI._pathRecursosPerm + _arquivoResPerdasHorario;
        }

        public string GetNomeComp_arquivoResAlimNaoConvergiram()
        {
            return _parGUI._pathRecursosPerm + _arquivoResAlimNaoConvergiram;
        }

        internal string GetNomeArqTapsRTs()
        {
            return _parGUI._pathRecursosPerm  + _arqTapsRTs;
        }

        // nome arquivo Rmatrix
        internal string GetArqRmatrix()
        {
            return _parGUI._pathRecursosPerm + "\\Rmatrix.txt";
        }

        // nome arquivo Xmatrix
        internal string GetArqXmatrix()
        {
            return _parGUI._pathRecursosPerm + "\\Xmatrix.txt";
        }

        // nome arquivo condutor
        internal string GetNomeArqCondutor()
        {
            return _parGUI._pathRecursosPerm + "Condutores.dss";
        }

        //
        internal string GetNomeCargaBT_mes()
        {
            return _nomeAlimAtual + "CargaBT_" + _parGUI._mesAbrv3letras + ".dss";
        }

        // OBS: funcao redundante, caso no futuro exporte arquivo de carga com parametros (ex: tipo do modelo de carga) diferente.
        internal string GetNomeCargaBTCemig_mes()
        {
            return _nomeAlimAtual + "CargaBT_" + _parGUI._mesAbrv3letras + ".dss";
        }

        internal string GetNomeCargaMT_mes()
        {
            return _nomeAlimAtual + "CargaMT_" + _parGUI._mesAbrv3letras + ".dss";
        }

        //Seta nome do alim atual
        internal void SetNomeAlimAtual(string nome)
        {
            _nomeAlimAtual = nome;
        }

        //Get _nomeAlimAtual 
        internal string GetNomeAlimAtual()
        {
            return _nomeAlimAtual;
        }

        // Seta tensao saidaSE
        internal void SetTensaoSaidaSE(string nome)
        {
            _parGUI._tensaoSaidaBarUsuario = nome;
        }

        // get nome do arquivo DRPDRC
        internal string GetNomeComp_arquivoDRPDRC()
        {
            return _parGUI._pathRecursosPerm + _arquivoDRPDRC;
        }

        // get nome do arquivo DRPDRC
        internal string GetNomeComp_arqBarrasDRPDRC()
        {
            return _parGUI._pathRecursosPerm + _arqBarrasDRPDRC;
        }

        // get nome do arquivo BarraTraf
        internal string GetNomeArqBarraTrafo()
        {
            return _parGUI._pathRecursosPerm + _arqBarraTrafo;
        }

        // get dataPath OpenDSS
        internal string GetDataPathAlimOpenDSS()
        {
            return _parGUI._pathRaizGUI + _nomeAlimAtual;
        }

        // get nome arquivo ajuste de acordo com o mes
        public string GetNomeArqAjuste(int mes)
        {
            return _parGUI._pathRecursosPerm + "Ajuste_" + TipoDiasMes.GetMesAbrv(mes) + ".xlsx";
        }

        // Tradução da função criaDiretorioDSS
        private string GetDirAlimentadorDSS(string nomeAlm)
        {
            return _parGUI._pathRaizGUI + nomeAlm + "\\";
        }

        // get nome arquivo alimentador DSS 
        internal string GetNomeArquivoAlimentadorDSS()
        {
            return GetDirAlimentadorDSS(_nomeAlimAtual) + _nomeAlimAtual + ".dss";
        }

        // get Nome e Path CurvasTxtCompleto
        internal string GetPathCurvasTxtCompleto()
        {
            return _pathCurvasTxt;
        }
        
        // nome arquivo gerador MT
        internal string GetNomeGeradorMT_mes()
        {
            return _nomeAlimAtual + "GeradorMT_" + _parGUI._mesAbrv3letras + ".dss";
        }

        // get Nome e Path CurvasTxtCompleto
        internal string GetNomeEPathCurvasTxtCompleto(string tipoDia)
        {
            return _pathCurvasTxt + _nomeArqCurvasDeCarga + tipoDia + ".dss";
        }

        // get nome arquivo capacitor
        internal string GetNomeCapacitorMT()
        {
            return _nomeAlimAtual + "CapacitorMT.dss";
        }

        // nome completo arquivo AnualD.dss
        internal string GetNomeArquivoB()
        {
            return _nomeAlimAtual + "AnualB.dss";
        }

        // diretorio alimenta
        internal string GetDiretorioAlim()
        {
            return _parGUI._pathRaizGUI + _nomeAlimAtual + "\\";
        }
        
        //Tradução da função getLoadMult
        public double GetLoadMult()
        {
            // alimTmp
            string alimTmp = GetNomeAlimAtual();

            // modo de otimizacao de energia OU potencia 
            if (_parGUI._otmPorEnergia || _parGUI._otmPorDemMax)
            {
                return _parGUI.loadMultAtual;
            }
            // obtem loadMult do map de loadMults
            else
            {
                // TODO verificar se mes foi setado corretamente no modo anual
                int mes = _parGUI.GetMes();

                // se loadMult existe no map
                if (_janelaPrincipal._medAlim._reqLoadMultMes._mapAlimLoadMult[mes].ContainsKey(alimTmp))
                {
                    return _janelaPrincipal._medAlim._reqLoadMultMes._mapAlimLoadMult[mes][alimTmp];
                }
                else
                {
                    _janelaPrincipal.ExibeMsgDisplayMW(alimTmp + ": Alimentador não encontrado no arquivo de ajuste");

                    // retorna loadMult Default
                    return _parGUI.loadMultDefault;
                }
            }
        }
    }
}

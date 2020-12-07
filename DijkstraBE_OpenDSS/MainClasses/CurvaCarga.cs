using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutorOpenDSS.Classes_Principais
{
    class CurvaCarga
    {
        //Tradução da função criaArquivosTXTCurvaDeCargaPU e criaArquivosTXTCurvaDeCargaPUPvt
        public static void CriaTxtPU(ParamGeraisDSS paramGerais, MainWindow janela)
        {
            List<string> conteudo = new List<string>();
            string arquivo;
            string caminho = paramGerais.getPathCurvasTxtCompleto();

            string tipoDia = paramGerais._parGUI._tipoDia;
            //Para cada faixa
            for (int i = 1; i <= 7; i++)
            {
                //A4
                conteudo = MatrizDadosPU.abreXLSCurvaDeCargaNormalizada(i, "A4", tipoDia, paramGerais._pathCurvasXLS, janela);
                arquivo = caminho + "arqCurvaNormA4-" + i + tipoDia;
                ArqManip.GravaListArquivoTXT(conteudo, arquivo, janela);

                //Classes que vão até a faixa 6
                if (i <= 6)
                {
                    //Residencial
                    conteudo = MatrizDadosPU.abreXLSCurvaDeCargaNormalizada(i, "RES", tipoDia, paramGerais._pathCurvasXLS, janela);
                    arquivo = caminho + "arqCurvaNormRES" + i + tipoDia;
                    ArqManip.GravaListArquivoTXT(conteudo, arquivo, janela);
                }

                //Classes que vão até a faixa 4
                if (i <= 4)
                {
                    //Industrial
                    conteudo = MatrizDadosPU.abreXLSCurvaDeCargaNormalizada(i, "IND", tipoDia, paramGerais._pathCurvasXLS, janela);
                    arquivo = caminho + "arqCurvaNormIND" + i + tipoDia;
                    ArqManip.GravaListArquivoTXT(conteudo, arquivo, janela);

                    //Rural
                    conteudo = MatrizDadosPU.abreXLSCurvaDeCargaNormalizada(i, "RUR", tipoDia, paramGerais._pathCurvasXLS, janela);
                    arquivo = caminho + "arqCurvaNormRUR" + i + tipoDia;
                    ArqManip.GravaListArquivoTXT(conteudo, arquivo, janela);

                    //Comercial
                    conteudo = MatrizDadosPU.abreXLSCurvaDeCargaNormalizada(i, "COM", tipoDia, paramGerais._pathCurvasXLS, janela);
                    arquivo = caminho + "arqCurvaNormCOM" + i + tipoDia;
                    ArqManip.GravaListArquivoTXT(conteudo, arquivo, janela);
                }

            }

            //IP
            conteudo = MatrizDadosPU.abreXLSCurvaDeCargaNormalizada(1, "IP", tipoDia, paramGerais._pathCurvasXLS, janela);
            arquivo = caminho + "CurvaTipicaIP" + tipoDia;
            ArqManip.GravaListArquivoTXT(conteudo, arquivo, janela);

            //GDBT
            conteudo = MatrizDadosPU.abreXLSCurvaDeCargaNormalizada(1, "GD", tipoDia, paramGerais._pathCurvasXLS, janela);
            arquivo = caminho + "arqCurvaGeracaoGDBT" + tipoDia;
            ArqManip.GravaListArquivoTXT(conteudo, arquivo, janela);

            //A3a
            conteudo = MatrizDadosPU.abreXLSCurvaDeCargaNormalizada(1, "A3a", tipoDia, paramGerais._pathCurvasXLS, janela);
            arquivo = caminho + "arqCurvaNormA3a-1" + tipoDia;
            ArqManip.GravaListArquivoTXT(conteudo, arquivo, janela);
        }

        //Tradução da função EscreveCurvasDeCarga
        public static void EscreveCurvas(ParamGeraisDSS paramGerais, MainWindow janela)
        {
            //Nome do arquivo
            string nome = paramGerais.getNomeEPathCurvasTxtCompleto();

            //Escreve a descrição do arquivo
            File.WriteAllText(nome, "! Definicao da curva de carga do alimentador\n\n");

            //Para cada curva na lista, escreve no arquivo
            foreach (string curva in paramGerais._listaNomeArquivosCurvasCarga.Values.ToList())
            {
                EscreveCurvaDiaria(nome, curva, paramGerais, janela);
            }

        }

        //Tradução da função escreveCurvaDeCargaDiaria
        private static void EscreveCurvaDiaria(string fid, string nomeCurvaDeCarga, ParamGeraisDSS paramGerais, MainWindow janela)
        {
            //Arquivo curva de carga
            string arquivoCurvaDeCarga = nomeCurvaDeCarga + paramGerais._parGUI._tipoDia;

            //Cria o load shape
            string loadShape = "new loadshape." + nomeCurvaDeCarga + " npts=24 interval=1.0";

            //Cria linha de curva de carga
            string linhaCSV = "~ csvfile=\"" + paramGerais.getPathCurvasTxtCompleto() + arquivoCurvaDeCarga + '\"';

            //Escreve no arquivo, appendando
            using (StreamWriter file = new StreamWriter(fid, true))
            {
                try
                {
                    file.WriteLine(loadShape);
                    file.WriteLine(linhaCSV);
                    file.WriteLine("");
                }
                catch
                {
                    janela.Disp("Não foi possível escrever no arquivo " + fid, "E");
                }
            }
        }

        // TODO veriricar repetição de codigo  e DELETAR
        //Pega o número de dias do mês, separado por tipo de dia (Dia útil, sábado e domingo e feriado)
        public static Dictionary<string, int> GetNumTipoDiasMes(ParamGeraisDSS paramGerais)
        {                       
            Dictionary<string, int> tipoDia = new Dictionary<string, int>();
            int ano = Int32.Parse(paramGerais._parGUI._ano);
            int mes = paramGerais._parGUI._mesNum;


            //Pega o número de dias do mês
            int numDiasMes = DateTime.DaysInMonth(ano, mes);

            //Inicializa a contagem dos dias
            int DU = 0; //Dia útil
            int DO = 0; //Domingo e feriado
            int SA = 0; //Sábado

            //Verifica o tipo de dia para cada dia do mês
            for (int dia = 1; dia <= numDiasMes; dia++)
            {
                //Verifica se é feriado
                if (paramGerais._objTipoDeDiasDoMes._listaDiasDeFeriadoAno[mes - 1].Contains(dia))
                {
                    DO++;
                }
                //Caso contrário, verifica o tipo de dia
                else
                {
                    switch (new DateTime(ano, mes, dia).DayOfWeek)
                    {
                        case DayOfWeek.Saturday:
                            SA++;
                            break;
                        case DayOfWeek.Sunday:
                            DO++;
                            break;
                        default:
                            DU++;
                            break;
                    }
                }

            }

            // Prepara a variável de saída e retorna
            tipoDia.Add("DU", DU);
            tipoDia.Add("SA", SA);
            tipoDia.Add("DO", DO);
            return tipoDia;
        }

        //TODO nao esta sendo usado
        //Soma das curvas
        public static Dictionary<string, double> GetSomaPUCurvasMensais(ParamGeraisDSS _paramGerais)
        {
            //TODO testar e substituir
            //Pega a quantidade de dias úteis, sábados e domingos e feriados do mês
            Dictionary<string, int> qtDias2 = _paramGerais._objTipoDeDiasDoMes._quantidadeTipoDia;
            
            Dictionary<string, int> qtDias = CurvaCarga.GetNumTipoDiasMes(_paramGerais);

            //Dictionary<string, int> qtDias = CurvaCarga.GetNumTipoDiasMes(_paramGerais);

            // Lista com os tipos de dia: dia util 'DU', sabado 'SA' ou domingo 'DO'
            List<string> dias = new List<string>() { "DO", "SA", "DU" };

            // caminho
            string caminho = _paramGerais.getPathCurvasTxtCompleto();

            // lista curvas de carga
            List<string> listaCurvasCarga = _paramGerais._listaNomeArquivosCurvasCarga.Values.ToList();

            // armazena a soma em PU da curva de carga mensal
            Dictionary<string, double> somaPUcurvaMensal = new Dictionary<string, double>();

            // para cada curva
            foreach (string curvaCarga in listaCurvasCarga)
            {
                double somaPUMes = 0;

                // para cada tipo dia
                foreach (string tipoDia in dias)
                {
                    double somaParc = 0;
                    string[] linhas = File.ReadAllLines(caminho + curvaCarga + tipoDia);

                    // para cada linha
                    foreach (string linha in linhas)
                    {
                        somaParc += double.Parse(linha, CultureInfo.InvariantCulture);
                    }
                    //end refactory

                    // 
                    somaPUMes += somaParc * qtDias[tipoDia];
                }

                // adiciona somaPUMes no map
                somaPUcurvaMensal.Add(curvaCarga, somaPUMes);
            }

            return somaPUcurvaMensal;
        }

        //Cria uma lista com os nomes dos arquivos de curva de carga
        public static Dictionary<string, string> GetNomeArquivosCurvasCarga()
        {
            //Inicializa a lista
            Dictionary<string, string> listaCurvasCarga = new Dictionary<string, string>();

            //Curvas únicas
            listaCurvasCarga.Add("GDBT1", "arqCurvaGeracaoGDBT");
            listaCurvasCarga.Add("IP1", "curvaTipicaIP");
            listaCurvasCarga.Add("A3a1", "arqCurvaNormA3a-1");

            //Cruvas com múltiplas faixas
            for (int i = 1; i <= 7; i++)
            {
                //4 faixas
                if (i <= 4)
                {
                    listaCurvasCarga.Add("COM" + i, "arqCurvaNormCOM" + i);
                    listaCurvasCarga.Add("IND" + i, "arqCurvaNormIND" + i);
                    listaCurvasCarga.Add("RUR" + i, "arqCurvaNormRUR" + i);
                }

                //6 faixas
                if (i <= 6)
                {
                    listaCurvasCarga.Add("RES" + i, "arqCurvaNormRES" + i);
                }

                //7 faixas
                listaCurvasCarga.Add("A4" + i, "arqCurvaNormA4-" + i);

            }

            return listaCurvasCarga;
        }

    }
}

using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutorOpenDSS
{
    class MatrizDadosPU
    {
        public float[,] ip;
        public float[,] gd;
        public Res res;
        public Com com;
        public Ind ind;
        public Rur rur;
        public A4 a4;

        public MatrizDadosPU()
        {
            ip = abreXLSCurvaDeCargaNormalizada(1, "IP");
            gd = abreXLSCurvaDeCargaNormalizada(1, "GD");
            res = new Res();
            com = new Com();
            ind = new Ind();
            rur = new Rur();
            a4 = new A4();
        }

        public class Res
        {
            public float[,] faixa1;
            public float[,] faixa2;
            public float[,] faixa3;
            public float[,] faixa4;
            public float[,] faixa5;
            public float[,] faixa6;

            public Res()
            {
                faixa1 = abreXLSCurvaDeCargaNormalizada(1, "RES");
                faixa2 = abreXLSCurvaDeCargaNormalizada(2, "RES");
                faixa3 = abreXLSCurvaDeCargaNormalizada(3, "RES");
                faixa4 = abreXLSCurvaDeCargaNormalizada(4, "RES");
                faixa5 = abreXLSCurvaDeCargaNormalizada(5, "RES");
                faixa6 = abreXLSCurvaDeCargaNormalizada(6, "RES");
            }
        }
        public class Com
        {
            public float[,] faixa1;
            public float[,] faixa2;
            public float[,] faixa3;
            public float[,] faixa4;

            public Com()
            {
                faixa1 = abreXLSCurvaDeCargaNormalizada(1, "COM");
                faixa2 = abreXLSCurvaDeCargaNormalizada(2, "COM");
                faixa3 = abreXLSCurvaDeCargaNormalizada(3, "COM");
                faixa4 = abreXLSCurvaDeCargaNormalizada(4, "COM");
            }
        }
        public class Ind
        {
            public float[,] faixa1;
            public float[,] faixa2;
            public float[,] faixa3;
            public float[,] faixa4;

            public Ind()
            {
                faixa1 = abreXLSCurvaDeCargaNormalizada(1, "IND");
                faixa2 = abreXLSCurvaDeCargaNormalizada(2, "IND");
                faixa3 = abreXLSCurvaDeCargaNormalizada(3, "IND");
                faixa4 = abreXLSCurvaDeCargaNormalizada(4, "IND");
            }
        }
        public class Rur
        {
            public float[,] faixa1;
            public float[,] faixa2;
            public float[,] faixa3;
            public float[,] faixa4;

            public Rur()
            {
                faixa1 = abreXLSCurvaDeCargaNormalizada(1, "RUR");
                faixa2 = abreXLSCurvaDeCargaNormalizada(2, "RUR");
                faixa3 = abreXLSCurvaDeCargaNormalizada(3, "RUR");
                faixa4 = abreXLSCurvaDeCargaNormalizada(4, "RUR");
            }
        }
        public class A4
        {
            public float[,] faixa1;
            public float[,] faixa2;
            public float[,] faixa3;
            public float[,] faixa4;
            public float[,] faixa5;
            public float[,] faixa6;
            public float[,] faixa7;

            public A4()
            {
                faixa1 = abreXLSCurvaDeCargaNormalizada(1, "A4");
                faixa2 = abreXLSCurvaDeCargaNormalizada(2, "A4");
                faixa3 = abreXLSCurvaDeCargaNormalizada(3, "A4");
                faixa4 = abreXLSCurvaDeCargaNormalizada(4, "A4");
                faixa5 = abreXLSCurvaDeCargaNormalizada(5, "A4");
                faixa6 = abreXLSCurvaDeCargaNormalizada(6, "A4");
                faixa7 = abreXLSCurvaDeCargaNormalizada(7, "A4");
            }
        }

        //TODO testar: 
        public static List<string> abreXLSCurvaDeCargaNormalizada(int faixa, string classe, string dia, string caminho, MainWindow janela)
        {
            string arquivoCurvas;
            List<string> retorno = new List<string>();
            int coluna;

            //Verifica o tipo de dia e armazena a coluna correspondente
            switch (dia)
            {
                case "DU":
                    coluna = 1;
                    break;
                case "SA":
                    coluna = 2;
                    break;
                case "DO":
                    coluna = 3;
                    break;
                default:
                    return null;
            }

            //DEfine o nome do arquivo de curvas de carga
            if (classe.Equals("PODER PUBLICO") || classe.Equals("OUTROS"))
            {
                classe = "COM";
            }
            switch (classe)
            {
                case "IP":
                case "GD":
                    arquivoCurvas = "tip" + classe + ".xlsx";
                    break;
                default:
                    arquivoCurvas = "tip" + classe + "_faixa" + faixa + ".xlsx";
                    break;
            }

            //Lê o arquivo
            if (File.Exists(caminho + arquivoCurvas))
            {
                try
                {
                    var file = new FileInfo(caminho + arquivoCurvas);
                    using (var exc = new ExcelPackage(file))
                    {
                        //Pega a primeira planilha do arquivo
                        ExcelWorksheet plan = exc.Workbook.Worksheets.First();
                        //Lê 24 linhas, da linha 3 a 26, adicionando-as à lista de retorno
                        for (int l = 3; l <= 26; l++)
                        {
                            try
                            {
                                retorno.Add(plan.Cells[l, coluna].Value.ToString().Replace(',', '.'));
                            }
                            catch
                            {
                                //Retorna null caso haja algum problema na leitura
                                janela.ExibeMsgDisplayMW("Problema na leitura do arquivo " + caminho + arquivoCurvas + '.', "E");
                                return null;
                            }
                        }

                    }
                    //Retorna a lista
                    return retorno;
                }
                catch
                {
                    janela.ExibeMsgDisplayMW("Problema na leitura do arquivo " + caminho + arquivoCurvas + ". Verifique se o mesmo encontra-se aberto em outro programa.", "E");
                    return null;
                }

            }
            else
            {
                janela.ExibeMsgDisplayMW("Arquivo " + caminho + arquivoCurvas + " não encontrado.", "E");
                return null;
            }

        }



        public static float[,] abreXLSCurvaDeCargaNormalizada(int faixa, string classe)
        {
            string arquivoCurvas;
            string caminho=""; //Verificar
            float[,] retorno = new float[24,3];
            if (classe.Equals("PODER PUBLICO") || classe.Equals("OUTROS"))
            {
                classe = "COM";
            }
            switch (classe)
            {
                case "IP":
                case "GD":
                    arquivoCurvas = "tip" + classe + ".xlsx";
                    break;
                default:
                    arquivoCurvas = "tip" + classe + "_faixa" + faixa + ".xlsx";
                    break;
            }
            if (File.Exists(caminho+"curvas\\"+arquivoCurvas))
            {
                var file = new FileInfo(caminho+"curvas\\"+arquivoCurvas);
                using (var exc = new ExcelPackage(file))
                {
                    ExcelWorksheet plan = exc.Workbook.Worksheets.First();
                    for (int l = 0; l < 24; l++)
                    {
                        for (int c = 0; c < 3; c++)
                        {
                            retorno[l, c] = float.Parse(plan.Cells[l + 3, c + 1].Value.ToString());
                        }
                    }
                }
                return retorno;
            }
            else
            {
                return null;
            }
        }
    }
}

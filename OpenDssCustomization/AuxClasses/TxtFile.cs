using ExecutorOpenDSS.Classes;
using System;
using System.Collections.Generic;
using System.IO;

namespace ExecutorOpenDSS
{
    class TxtFile
    {
        //Verifica se o arquivo existe antes de deletá-lo
        public static void SafeDelete(string arquivo)
        {
            if (File.Exists(arquivo))
            {
                File.Delete(arquivo);
            }
        }
        //Grava CONTEUDO em arquivo FID 
        public static void GravaEmArquivo2(string conteudo, string fid, MainWindow janela)
        {
            try
            {
                File.AppendAllText(fid, conteudo);
            }
            catch
            {
                janela.ExibeMsgDisplay("Não foi possível escrever no arquivo " + fid);
            }
        }

        //Escreve uma lista de strings em um arquivo
        public static void GravaListArquivoTXT(List<string> conteudo, string arquivo, MainWindow jan)
        {
            //Verifica se existe conteudo
            if (conteudo != null)
            {
                //Tenta escrever no arquivo
                try
                {
                    File.AppendAllLines(arquivo, conteudo);
                }

                //Caso não consiga, exibe mensagem de erro
                catch
                {
                    
                    jan.ExibeMsgDisplay("Não foi possível escrever no arquivo " + arquivo);
                }
            }
        }
        
        // OLD CODE
        // Grava CONTEUDO em arquivo FID 
        public static void GravaEmArquivoAsync(string conteudo, string fid, MainWindow jan)
        {
            //
            while (true)
            {
                try
                {
                    // TODO consertar esquema de bloqueio de arquivo
                    string arq = jan._indiceArq == 0 ? fid : fid + jan._indiceArq + ".txt";

                    using (StreamWriter file = new StreamWriter(arq, true))
                    {
                        file.WriteLineAsync(conteudo);
                    }
                    break;
                }
                catch
                {
                    jan.ExibeMsgDisplay("Arquivo " + fid + " bloqueado. Tentando outro nome de arquivo");
                    jan._indiceArq++;
                }
                if (jan._indiceArq > 10)
                {
                    jan.ExibeMsgDisplay("Não foi possível escrever o arquivo " + fid + ". Verifique se possui permissão de escrita no local");
                    break;
                }
            }
        }

        //Grava CONTEUDO em arquivo FID 
        public static void GravaEmArquivo(string conteudo, string fid, MainWindow jan )
        {
            //
            while(true)
            {
                try
                {
                    // TODO consertar esquema de bloqueio de arquivo
                    string arq = jan._indiceArq == 0 ? fid : fid + jan._indiceArq + ".txt";
                    
                    using (StreamWriter file = new StreamWriter(arq, true))
                    {
                        file.WriteLineAsync(conteudo);
                    }
                    break;
                }
                catch 
                {
                        jan.ExibeMsgDisplay("Arquivo " + fid + " bloqueado. Tentando outro nome de arquivo");
                        jan._indiceArq++;
                }
                if (jan._indiceArq > 10)
                {
                    jan.ExibeMsgDisplay("Não foi possível escrever o arquivo " + fid + ". Verifique se possui permissão de escrita no local");
                    break;
                }
            }       
        }

        //funcao gravaPerdas em arquivo
        static public void GravaPerdas(PFResults perdasTotais, string nomeAlim, string fid, MainWindow jan)
        {
            // cria string com o formato de saida das perdas
            string conteudo = perdasTotais._energyMeter.FormataResultado(nomeAlim);

            //Grava em arquivo
            TxtFile.GravaEmArquivo(conteudo, fid, jan);
        }
 
        //Grava a lista de alimentadores não convergentes em um txt
        static public void GravaLstAlimNaoConvergiram(GeneralParameters paramGerais)
        {
            string nomeAlim = paramGerais.GetNomeAlimAtual();

            while (true)
            {
                try
                {
                    // 
                    string arquivoNome = paramGerais.GetNomeComp_arquivoResAlimNaoConvergiram();
                    
                    using (StreamWriter file = new StreamWriter(arquivoNome, true))
                    {
                        file.WriteLineAsync(nomeAlim);
                    }
                    break;
                }
                catch
                {
                    paramGerais._mWindow.ExibeMsgDisplay("Arquivo dos alimentadores não convergentes bloqueado!");
                }
            }       
        }
    
    }
}

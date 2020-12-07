using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutorOpenDSS
{
    class AlimentadoresCemig
    {
        //Pega a lista com todos os alimentadores
        public static List<string> getTodos(string arquivo)
        {
            //Variável que armazenará a lista com os alimentadores
            List<string> alimentadores = new List<string>();
            
            //Bloco que trata o arquivo, abrindo e fechando-o
            if (File.Exists(arquivo))
            {
                using (StreamReader sr = new StreamReader(arquivo))
                {
                    //Variável para armazenar a linha atual do arquivo
                    String linha;
                    //Lê a próxima linha até o fim do arquivo
                    while ((linha = sr.ReadLine()) != null)
                    {
                        //Caso haja uma linha em branco, linha[0] retornará um erro.
                        //O try/catch ignora o erro e passa para a próxima linha
                        try
                        {
                            //Se a linha começa com %, ignorar pois é comentário
                            if (!linha[0].Equals('%'))
                            {
                                //Adiciona a linha para a lista
                                alimentadores.Add(linha.Split('%')[0].Trim());
                            }

                        }
                        catch { }
                    }
                }
                return alimentadores;
            }
            else
            {
                throw new FileNotFoundException("Arquivo " + arquivo + " não encontrado.");
            }
            
        }
    }
}

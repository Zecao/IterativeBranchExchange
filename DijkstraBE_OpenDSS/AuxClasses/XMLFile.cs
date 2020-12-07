using System.IO;
using System.Xml.Linq;

namespace ExecutorOpenDSS.Classes_Auxiliares
{
    class XMLFile
    {
        //Lê arquivo XML e retorna o elemento raiz. Retorna null caso não exista o arquivo
        public static XElement LeXML(string arquivo, MainWindow janela)
        {
            //Verifica se o arquivo existe
            if (File.Exists(arquivo))
            {
                //Lê o documento
                XDocument xDoc = XDocument.Load(arquivo);

                //Retorna o elemento raiz
                return xDoc.Root;
            }

            //Caso não exista
            else
            {
                //Exibe mensagem de erro
                janela.ExibeMsgDisplayMW("Arquivo " + arquivo + " não encontrado.");

                //Retorna null
                return null;
            }
        }

        //Lê arquivo XML e retorna o elemento raiz. Retorna null caso não exista o arquivo
        public static XElement LeXML(string arquivo)
        {
            //Verifica se o arquivo existe
            if (File.Exists(arquivo))
            {
                //Lê o documento
                XDocument xDoc = XDocument.Load(arquivo);

                //Retorna o elemento raiz
                return xDoc.Root;
            }

            //Caso não exista
            else
            {
                //Retorna null
                return null;
            }
        }

    }
}

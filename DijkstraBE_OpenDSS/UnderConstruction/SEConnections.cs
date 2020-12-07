using ExcelAux;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ExecutorOpenDSS.Classes_Auxiliares
{
    public class SEConnections
    {
        private SEConnectionsParameters _paramConexoesSE;
        private List<String> _lstSEs;
        private StringBuilder _matrizInterconexao = new StringBuilder();

        //Construtor 
        public SEConnections(SEConnectionsParameters param)
        {
            // preenche variavel de classe
            _paramConexoesSE = param;

            // preenche variavel de classe lstSEs
            PreencheLstSEs();

            // cria matriz interconexoes
            CriaMatrizInterconexoes();

            // plota matriz de interconexoes
            GravaMatrizInterconexoes();
        }

        // 
        private void GravaMatrizInterconexoes()
        {
            // cria nome arquivo CSV
            String pathArquivo = _paramConexoesSE.getNomeEPathMatrizInterconexoesCSV();

            // Deleta arquivo se existir
            TxtFile.SafeDelete(pathArquivo);

            // escreve CSV
            File.WriteAllText(pathArquivo, _matrizInterconexao.ToString());

            //Grava planilha 
            GravaExcel.CsvToExcel(pathArquivo, _paramConexoesSE.getNomeEPathMatrizInterconexoes(), ',', "Plan1", true);
        }

        // 
        private void CriaMatrizInterconexoes()
        {
            // para cada SE
            foreach (string SE in _lstSEs)
            {
                // Le xml SE
                XElement xmlSE = XMLFile.LeXML(_paramConexoesSE.getNomeArquivoSEXML(SE));

                // Se SE nao existe 
                if (xmlSE == null)
                {
                    continue;
                }

                // obtem lista de alimentadores da SE
                List<string> lstAlimSE = GetLstAlimSE(SE, xmlSE);

                // obtem lista de alimentadores de interconexao
                List<string> lstAlimInterconexaoSE = GetLstAlimInterconexaoSE(xmlSE);

                // para cada alimentador 
                foreach (string alim in lstAlimSE)
                {
                    // OBS: comentado por performance
                    //int numChaves = getNumeroInterconexoes(alim, xmlSE);

                    //
                    foreach (string alim2 in lstAlimInterconexaoSE)
                    {
                        // 
                        int numChavesAlim1e2 = GetNumeroInterconexoesAlim1e2(alim, alim2, xmlSE);

                        // grava informacao se numChavesAlim1e2 diferente de 0
                        if (numChavesAlim1e2 != 0)
                        {
                            // linha matriz
                            string linha = alim + "," + alim2 + "," + numChavesAlim1e2.ToString();

                            // adiciona a linha a matriz de interconexao
                            _matrizInterconexao.AppendLine(linha);
                        }
                    }
                }
            }
        }

        // obtem o numero de interconexoes de um dado alimentador 
        private int GetNumeroInterconexoes(string alim, XElement xmlSE)
        {
            // obtem lista chaves socorro de toda a SE
            XElement lstChavesXML = xmlSE.Element("List_SOCORRO");

            // obtem a lista de chaves socorro do alimentador 
            IEnumerable<XElement> lstChavesDoAlimXML = lstChavesXML.Elements("SOCORRO").Where(
                x => x.Element("CIRC1_CODIGO").Value.Equals(alim) || x.Element("CIRC2_CODIGO").Value.Equals(alim));

            // conta numero de chaves
            int numChaves = lstChavesDoAlimXML.Count();

            return numChaves;
        }

        // obtem o numero de interconexoes de um dado alimentador 
        private int GetNumeroInterconexoesAlim1e2(string alim1, string alim2, XElement xmlSE)
        {
            //condicao de retorno
            if (alim1 == alim2)
            {
                return 0;
            }

            // obtem lista chaves socorro de toda a SE
            XElement lstChavesXML = xmlSE.Element("List_SOCORRO");

            // obtem a lista de chaves socorro do alimentador 
            IEnumerable<XElement> lstChavesDoAlimXML = lstChavesXML.Elements("SOCORRO").Where(
                x => x.Element("CIRC1_CODIGO").Value.Equals(alim1) && x.Element("CIRC2_CODIGO").Value.Equals(alim2));

            // conta numero de chaves
            int numChaves = lstChavesDoAlimXML.Count();

            // numChaves igual a 0 analisa situacao inversa
            if (numChaves == 0)
            {
                // obtem a lista de chaves socorro do alimentador 
                lstChavesDoAlimXML = lstChavesXML.Elements("SOCORRO").Where(
                x => x.Element("CIRC1_CODIGO").Value.Equals(alim2) && x.Element("CIRC2_CODIGO").Value.Equals(alim1));

                // conta numero de chaves
                numChaves = lstChavesDoAlimXML.Count();
            }

            //OBS: a rigor, deveria analisar a situacao inversa acima e fazer a intersecao dos dois conjuntos, 
            // mas acredito que devido a maneira que o XML eh formado esta analise nao sera necessaria. 

            return numChaves;
        }

        // obtem lista de alimentadores 
        private List<string> GetLstAlimInterconexaoSE(XElement xmlSE)
        {
            // obtem lista alimentadores
            XElement lstAlimXML = xmlSE.Element("List_SOCORRO");

            // lista de alim1  
            IEnumerable<XElement> lstNomeAlimXML1 = lstAlimXML.Elements("SOCORRO").Elements("CIRC1_CODIGO");

            // lista de alimentadores
            List<string> lstAlimInterconexao = new List<string>();

            // para cada alim da lstNomeAlim1XML
            foreach (XElement alim in lstNomeAlimXML1)
            {
                string nomeAlim = alim.Value;

                // adiciona na lista, caso nao contenha
                if (!lstAlimInterconexao.Contains(nomeAlim))
                {
                    lstAlimInterconexao.Add(nomeAlim);
                }
            }

            // lista de alim1  
            IEnumerable<XElement> lstNomeAlimXML2 = lstAlimXML.Elements("SOCORRO").Elements("CIRC2_CODIGO");

            // para cada alim da lstNomeAlim1XML
            foreach (XElement alim in lstNomeAlimXML2)
            {
                string nomeAlim = alim.Value;

                // adiciona na lista, caso nao contenha
                if (!lstAlimInterconexao.Contains(nomeAlim))
                {
                    lstAlimInterconexao.Add(nomeAlim);
                }
            }

            return lstAlimInterconexao;
        }

        // obtem lista de alimentadores 
        private List<string> GetLstAlimSE(string SE, XElement xmlSE)
        {
            // obtem lista alimentadores
            XElement lstAlimXML = xmlSE.Element("List_CIRCUITO");

            // lista de  
            IEnumerable<XElement> lstNomeAlimXML = lstAlimXML.Elements("CIRCUITO").Elements("NOME");

            // lista de alimentadores
            List<string> lstAlim = new List<string>();

            // para cada elemento
            foreach (XElement alim in lstNomeAlimXML)
            {
                string nomeAlim = alim.Value;

                lstAlim.Add(nomeAlim);
            }

            return lstAlim;
        }

        // preenche variavel de classe lstSEs
        private void PreencheLstSEs()
        {
            _lstSEs = new List<string>();

            try
            {
                _lstSEs = TxtFile.LeAlimentadoresArquivoTXT(_paramConexoesSE.getNomeEPathListaSEs());

                // TODO
                // Disp(lstAlimentadores.Count + " SEs listados.");
            }
            catch
            {
                // TODO
                //Caso ocorra erro, retorna aviso e solicita o cancelamento
                //Disp("Não foi possível ler " + _paramGerais.getNomeEPathListaSEs() );

                // TODO
                //_solicitaCancel = true;
                return;
            }

        }
    }
}

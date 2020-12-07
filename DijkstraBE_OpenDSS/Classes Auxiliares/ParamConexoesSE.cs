using ExecutorOpenDSS.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutorOpenDSS.Classes_Auxiliares
{
    public class ParamConexoesSE
    {
        private string _pathRecursosPerm;
        private string _pathXML;
        private string _path;

        // construtor
        public ParamConexoesSE(ParametrosGUI parGUI)
        {
            _pathRecursosPerm = parGUI._pathRecursosPerm;
            _pathXML = parGUI._pathRaizGUI + "XMLs\\";
            _path = parGUI._pathRaizGUI;
        }

        // retorna lista de SEs
        internal string getNomeEPathListaSEs()
        {
            return _pathRecursosPerm + "\\lstSEs.txt";
        }

        // retorna lista de SEs
        internal string getNomeArquivoSEXML(string SE)
        {
            return _pathXML + SE + ".xml";
        }

        // retorna nome e path matrizInterconexoes
        internal string getNomeEPathMatrizInterconexoes()
        {
            return _path + "matrizInterconexoes.xlsx";
        }

        // retorna nome e path matrizInterconexoes
        internal string getNomeEPathMatrizInterconexoesCSV()
        {
            return _path + "matrizInterconexoes.csv";
        }


    }
}

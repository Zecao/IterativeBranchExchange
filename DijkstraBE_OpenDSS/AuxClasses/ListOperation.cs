using System.Collections.Generic;

namespace ExecutorOpenDSS
{
    class ListOperation
    {
        //Multiplica listas
        public static List<double> Multiplica(List<double> lista, int numero)
        {
            List<double> resultado = new List<double>();
            foreach (double d in lista)
            {
                resultado.Add(d * numero);
            }
            return resultado;
        }

        //Soma elementos das listas
        public static List<double> Soma(List<double> lista1, List<double> lista2)
        {
            List<double> saida = new List<double>();
            if (lista1.Count == lista2.Count)
            {
                for (int i = 0; i < lista1.Count;i++ )
                {
                    saida.Add(lista1[i] + lista2[i]);
                }
            }
            else
            {
                return null;
            }
            return saida;
        }

        //Retorna o maior valor de um determinado índice de duas listas
        public static double Max(List<double> lista1, List<double> lista2, int indice)
        {
            if (lista1[indice] > lista2[indice])
            {
                return lista1[indice];
            }
            else
            {
                return lista2[indice];
            }
        }
    }
}

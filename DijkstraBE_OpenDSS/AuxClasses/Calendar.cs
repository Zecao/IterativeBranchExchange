using ExecutorOpenDSS.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutorOpenDSS.Classes_Auxiliares
{
    class TipoDiasMes
    {
        GUIParameters _parGUI;
        
        // armazena dias de feriados do ano
        public List<List<int>> _listaDiasDeFeriadoAno;

        // 
        public Dictionary<int, Dictionary<string, int>> _qntTipoDiasMes = new Dictionary<int, Dictionary<string, int>>();
        public Dictionary<string, int> _quantidadeTipoDia;

        public static string GetMesAbrv(int mes)
        { 
            switch (mes) {
                case 1:
                    return "Jan";
                case 2:
                    return "Fev";
                case 3:
                    return "Mar";
                case 4:
                    return "Abr";
                case 5:
                    return "Mai";
                case 6:
                    return "Jun";
                case 7:
                    return "Jul";
                case 8:
                    return "Ago";
                case 9:
                    return "Set";
                case 10:
                    return "Out";
                case 11:
                    return "Nov";
                case 12:
                    return "Dez";
                default: return "Jan";
            }
        }

        // Construtor
        public TipoDiasMes(GUIParameters _par, MainWindow jan)
        {
            _parGUI = _par;

            _listaDiasDeFeriadoAno = GetFeriados(_parGUI._ano, jan);

            //Para cada mes
            for (int mes = 1; mes <= 12; mes++)
            {
                CarregaQuantidadeTipoDiasMes(mes);
            }
        }

        //Obtem o numero dias do mes
        public int GetNumDiasMes(int mes)
        {
            int ano = Int32.Parse(_parGUI._ano);

            //Pega o número de dias do mês
            int numDiasMes = DateTime.DaysInMonth(ano, mes);

            return numDiasMes;
        }

        //Pega o número de dias do mês, separado por tipo de dia (Dia útil, sábado e domingo e feriado)
        // utiliza Struct _diasDeFeriado para transformar feriados em sabados ou dias util em domingo 
        public void CarregaQuantidadeTipoDiasMes(int mes)
        {
            //Pega o número de dias do mês
            int numDiasMes = GetNumDiasMes(mes);

            //Inicializa a contagem dos dias
            int DU = 0; //Dia útil
            int DO = 0; //Domingo e feriado
            int SA = 0; //Sábado

            int ano = Int32.Parse(_parGUI._ano);

            //Verifica o tipo de dia para cada dia do mês
            for (int dia = 1; dia <= numDiasMes; dia++)
            {
                //Verifica se é feriado
                if (this._listaDiasDeFeriadoAno[mes - 1].Contains(dia))
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

            // Preenche variável da classe
            _quantidadeTipoDia = new Dictionary<string, int>();
            _quantidadeTipoDia.Add("DU", DU);
            _quantidadeTipoDia.Add("SA", SA);
            _quantidadeTipoDia.Add("DO", DO);

            _qntTipoDiasMes.Add(mes, _quantidadeTipoDia);
        }

        //Pega os dias de feriado do ano como uma List<List<int>> o primeiro índice (de base 0) indica o mês.
        //Assim, diasDeFeriado[0] retorna uma lista com todos os feriados de janeiro
        private List<List<int>> GetFeriados(string ano, MainWindow janela)
        {
            //
            string arquivo = _parGUI._pathRecursosPerm + "Feriados" + ano + ".txt";

            //
            List<List<int>> feriados = new List<List<int>>();

            string line;
            for (int i = 0; i < 12; i++)
            {
                feriados.Add(new List<int>());
            }

            if (File.Exists(arquivo))
            {
                using (StreamReader file = new StreamReader(arquivo))
                {
                    for (int linha = 0; linha < 12; linha++)
                    {
                        line = file.ReadLine();
                        if (!line.Equals(""))
                        {
                            string[] dias = line.Split(';');
                            foreach (string dia in dias)
                            {

                                try
                                {
                                    int aux = Int32.Parse(dia.Trim());
                                    feriados[linha].Add(aux);
                                }
                                catch { }
                            }
                        }
                        else
                        {
                            feriados.Add(new List<int>());
                        }
                    }
                }
            }
            else
            {
                janela.ExibeMsgDisplay("Arquivo " + arquivo + " não encontrado");

                MainWindow.MensagemDelegate mensagem = new MainWindow.MensagemDelegate(janela.Mensagem);

                // TODO desabilitei pq a ausencia de feriado nao eh erro grave
                //janela._cancelarExecucao = true;

                // TODO FIX ME
                feriados[0].Add(1);
                feriados[2].Add(1);
                feriados[3].Add(1);
                feriados[4].Add(1);
                feriados[5].Add(1);
                feriados[6].Add(1);
                feriados[7].Add(1);
                feriados[8].Add(1);
                feriados[9].Add(1);
                feriados[10].Add(1);
                feriados[11].Add(1);
            }
            return feriados;
        }
    }
}

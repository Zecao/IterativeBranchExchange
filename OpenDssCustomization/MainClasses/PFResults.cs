//#define ENGINE
#if ENGINE
using OpenDSSengine;
#else
using dss_sharp;
#endif

using ExecutorOpenDSS.Classes_Auxiliares;
using ExecutorOpenDSS.Classes_Principais;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExecutorOpenDSS.Classes
{ 
    // Class Power Flow results
    class PFResults
    {
        // membros
        public bool _convergiuBool;
        public MyEnergyMeter _energyMeter;
        private Dictionary<string, double> _lossesMap; // = new Dictionary<string, double>(); 

        //Construtor por copia 
        public PFResults(PFResults rf)
        {
            _convergiuBool = rf._convergiuBool;
            _energyMeter = new MyEnergyMeter(rf._energyMeter);
        }

        //Construtor vazio
        public PFResults()
        {
            _convergiuBool = false;
            _energyMeter = new MyEnergyMeter();
        }

        // formata resultado do fluco para console
        public string GetResultadoFluxoToConsole(string nomeAlim, ObjDSS oDSS)
        {
            //nivel pu
            string tensao = oDSS.GetActiveCircuit().Vsources.pu.ToString("0.###");

            double totalKWh = _energyMeter.KWh;
            string gd = "";

            // verifies Distributed Genaration 
            if (_energyMeter.KWhGD != 0)
            {
                // adds DG genarator to totalkWH
                totalKWh += _energyMeter.KWhGD;
                gd = " (GD)";
            }

            string tmp = "Alim\\PU SaidaSE:\\Energia(KWh)\\Perdas(KWh)\\Perdas(%)" +
                "\n" + nomeAlim +
                " \t" + tensao +
                " \t" + totalKWh.ToString("0.0") + gd +
                " \t" + _energyMeter.LossesKWh.ToString("0.0") +
                " \t" + CalcPercentualPerdas() + "%";
            return tmp;
        }

        // Calculates monthly Power flow results from 3 daily Power Flow (Sunday, Saturday and common days)
        // O resultado no fluxo mensal eh armazenado na variavel da classe
        public void CalculaResultadoFluxoMensal(PFResults perdasDU, PFResults perdasSA, PFResults perdasDO, GeneralParameters paramGerais)
        {
            // Limpa medidor atual.
            _energyMeter = new MyEnergyMeter();

            // calcula geracao e perdas maximas entre os 3 dias tipicos
            CalcGeracaoEPerdasMax(perdasDU, perdasSA, perdasDO);

            //Obtem mes
            int mes = paramGerais._parGUI.GetMes();

            // cria curva de carga dados: numero de dias do mes e matriz de consumo em PU
            Dictionary<string, int> numTipoDiasMes = paramGerais._objTipoDeDiasDoMes._qntTipoDiasMes[mes];
            
            // DIAS UTEIS
            perdasDU._energyMeter.MultiplicaEnergia(numTipoDiasMes["DU"]);        

            // multiplica pelo Num dias
            perdasSA._energyMeter.MultiplicaEnergia(numTipoDiasMes["SA"]);

            // multiplica pelo Num dias
            perdasDO._energyMeter.MultiplicaEnergia(numTipoDiasMes["DO"]);

            // perdas energia
            SomaEnergiaDiasTipicos(perdasDU, perdasSA, perdasDO);

            // setMes
            _energyMeter.SetMesEM(mes);

            // grava LoadMult do DU
            _energyMeter.GravaLoadMult(perdasDU._energyMeter.loadMultAlim);

            // cria string com o formato de saida das perdas
            string conteudo = _energyMeter.FormataResultado(paramGerais.GetNomeAlimAtual());
            
            // se modo otimiza nao grava perdas arquivo
            if ( !paramGerais._parGUI._otmPorEnergia )
            {
                // grava perdas alimentador em arquivo
                TxtFile.GravaEmArquivo(conteudo, paramGerais.GetNomeComp_arquivoResPerdasMensal(), paramGerais._mWindow);
            }

            // Se chegou ate aqui, seta convergencia para true
            _convergiuBool = true;
        }

        // Soma energia calculada dias tipicos 
        private void SomaEnergiaDiasTipicos(PFResults perdasDU, PFResults perdasSA, PFResults perdasDO)
        {
            _energyMeter.Soma(perdasDU._energyMeter);

            _energyMeter.Soma(perdasSA._energyMeter);

            _energyMeter.Soma(perdasDO._energyMeter);
        }

        // set energia e perdas mes para o fluxo simplificado
        public void EstimatesMonthEnergyAndLossesByDay(PFResults resFluxo, GeneralParameters par)
        {
            // month
            int mes = par._parGUI.GetMes();

            // number of days in the month
            int numDias = par._objTipoDeDiasDoMes.GetNumDiasMes(mes);
            
            // Multiplies the Daily solution by the number of days
            resFluxo._energyMeter.MultiplicaEnergia(numDias);

            // copia novos valores 
            _energyMeter = new MyEnergyMeter();
            _energyMeter = resFluxo._energyMeter;

            // seta fluxo mensal igual a true
            _convergiuBool = true;
        }

        // calcula geracao e perdas maximas entre os 3 dias tipicos
        private void CalcGeracaoEPerdasMax(PFResults perdasDU, PFResults perdasSA, PFResults perdasDO)
        {
            // DIAS UTEIS perdasDU MAX
            List<double> perdasDUMax = perdasDU.GetGeracaoEPerdaPotencia();

            // SABADO MAX
            List<double> perdasSAMax = perdasSA.GetGeracaoEPerdaPotencia();

            // DOMINGO MAX
            List<double> perdasDOMax = perdasDO.GetGeracaoEPerdaPotencia();

            // geracao maxima
            double geracaoMax = Math.Max(Math.Max(perdasDUMax[0], perdasSAMax[0]), perdasDOMax[0]);

            // perdas maxima
            double perdasMax = Math.Max(Math.Max(perdasDUMax[1], perdasSAMax[1]), perdasDOMax[1]);

            // atribui variavel da classe 
            _energyMeter.MaxkW = geracaoMax;
            _energyMeter.MaxkWLosses = perdasMax;
        }

        //get Energia
        public double GetEnergia()
        {
            return _energyMeter.KWh;
        }

        // get Geracao
        public double GetMaxKW()
        {
            return _energyMeter.MaxkW;
        }

        // get Geracao
        public double GetPerdasEnergia()
        {
            return _energyMeter.LossesKWh;
        }

        // get Geracao e Perda de Potencia
        public List<double> GetGeracaoEPerdaPotencia()
        {
            // TODO refactory
            List<double> ret = new List<double>
            {
                _energyMeter.MaxkW,
                _energyMeter.MaxkWLosses
            };

            return ret;
        }

        // verifica se excedeu a geracao maxima
        private bool ExcedeuGeracaoMaxima()
        {
            //MAXENERGIA alim igual 40.000.000 kWh/mes 
            double MAXENERGIA = 40000000;

            // condicao de erro no fluxo diario 
            if (Math.Abs(GetEnergia()) > MAXENERGIA)
            {
                return true;
            }
            return false;
        }

        // verifica se excedeu a geracao maxima
        private bool ExcedeuRequisitoMaximo()
        {
            //MAXREQUISITO alim igual 40.000 kWh 
            double MAXREQUISITO = 40000;

            // condicao de erro no fluxo diario 
            if (Math.Abs(GetMaxKW()) > MAXREQUISITO)
            {
                return true;
            }
            return false;
        }

        // Calcula percentual de perdas
        public string CalcPercentualPerdas()
        {
            double energia = _energyMeter.KWh + _energyMeter.KWhGD;
            double perdas = _energyMeter.LossesKWh;
            double percentual = perdas * 100 / energia;
            return percentual.ToString("0.000");
        }

        // Get Feeder losses
        public bool GetPerdasAlim(Circuit DSSCircuit)
        {
            // DEBUG
            //string[] registersNames = DSSCircuit.Meters.RegisterNames;

            // %%% Valor maximo de requisito e perdas
            _energyMeter.MaxkW = DSSCircuit.Meters.RegisterValues[2];
            _energyMeter.MaxkWLosses = DSSCircuit.Meters.RegisterValues[14];

            // %%% Energia Ativa e Reativa
            _energyMeter.KWh = DSSCircuit.Meters.RegisterValues[0];
            _energyMeter.kvarh = DSSCircuit.Meters.RegisterValues[1];
            _energyMeter.LossesKWh = DSSCircuit.Meters.RegisterValues[12];

            SetLossesMap(DSSCircuit);

            // OLD CODE
            // 34.5 kV line losses
            //_energyMeter.MTLineLosses34KV = lossesMap["34.5 kV Line Loss"];

            // LV line Losses
            _energyMeter.BTLineLosses = _lossesMap["0.22 kV Line Loss"] + _lossesMap["0.24 kV Line Loss"];

            // MV line losses
            _energyMeter.MTLineLosses = _lossesMap["13.8 kV Line Loss"] + _lossesMap["22 kV Line Loss"] + _lossesMap["34.5 kV Line Loss"];

            // Trafo 34.5 
            _energyMeter.TransformerAllLosses34KV = _lossesMap["34.5 kV Load Loss"] + _lossesMap["34.5 kV No Load Loss"];

            // perdas em transformadores de 13.8kV/220/127V
            // OBS: subtrai as perdas nos trafos de 34.5KV 
            _energyMeter.TransformerLosses = DSSCircuit.Meters.RegisterValues[23] - _energyMeter.TransformerAllLosses34KV;

            // Reg 19 NoLoadLosseskWh
            _energyMeter.NoLoadLosseskWh = DSSCircuit.Meters.RegisterValues[18];

            // %%% Perdas de Sequencia OBS: 
            // perdas seq. pos (25) e zero (26) e circuitos bi e monofasicos (28)
            //_energyMeter.lineLossesPosMode = DSSCircuit.Meters.RegisterValues[24];
            _energyMeter.lineLossesZeroMode = DSSCircuit.Meters.RegisterValues[25];
            // lineLossesoneTwoPhase = DSSCircuit.Meters.RegisterValues[27];

            //            
            _energyMeter.MTEnergy = _lossesMap["13.8 kV Load Energy"] + _lossesMap["22 kV Load Energy"] + _lossesMap["34.5 kV Load Energy"];
            _energyMeter.BTEnergy = _lossesMap["0.22 kV Load Energy"] + _lossesMap["0.24 kV Load Energy"];
            
            // obtem energia gerada por GDs
            GetGeracaoGD(DSSCircuit);
            
            // verifica convergencia
            return (VerificaConvergencia());
        }

        // obtem energia gerada por GDs
        private void GetGeracaoGD(Circuit DSSCircuit)
        {
            //int debug = DSSCircuit.Generators.Count;

            int iterGer = DSSCircuit.Generators.First ;

            while (iterGer != 0)
            {
                _energyMeter.KWhGD += DSSCircuit.Generators.RegisterValues[0];

                iterGer = DSSCircuit.Generators.Next;  
            }
        }

        private void SetLossesMap(Circuit DSSCircuit)
        {
            string[] registersNames = DSSCircuit.Meters.RegisterNames;

            _lossesMap = new Dictionary<string, double>(); 

            //preenche map com zeros para evitar erro
            _lossesMap.Add("34.5 kV Line Loss", 0);
            _lossesMap.Add("22 kV Line Loss", 0); //OBS: string without decimal "22 kV"
            _lossesMap.Add("13.8 kV Line Loss", 0);
            _lossesMap.Add("0.22 kV Line Loss", 0);
            _lossesMap.Add("0.24 kV Line Loss", 0);
            _lossesMap.Add("34.5 kV Load Loss", 0);
            _lossesMap.Add("22 kV Load Loss", 0); //OBS: string without decimal "22 kV"
            _lossesMap.Add("13.8 kV Load Loss", 0);
            _lossesMap.Add("0.22 kV Load Loss", 0);
            _lossesMap.Add("0.24 kV Load Loss", 0);
            _lossesMap.Add("34.5 kV No Load Loss", 0);
            _lossesMap.Add("22 kV No Load Loss", 0); //OBS: string without decimal "22 kV"
            _lossesMap.Add("13.8 kV No Load Loss", 0);
            _lossesMap.Add("0.22 kV No Load Loss", 0);
            _lossesMap.Add("0.24 kV No Load Loss", 0);
            _lossesMap.Add("13.8 kV Load Energy", 0);
            _lossesMap.Add("22 kV Load Energy", 0); //OBS: string without decimal "22 kV"
            _lossesMap.Add("34.5 kV Load Energy", 0);
            _lossesMap.Add("0.24 kV Load Energy", 0);
            _lossesMap.Add("0.22 kV Load Energy", 0);

            // search register and put the values in the lossesMap 
            for (int i=39;i<67;i++)
            {
                // se registrador nao eh nulo
                if (registersNames[i] != null )
                {
                    // adiciona perda no map 
                    if (_lossesMap.ContainsKey(registersNames[i]))
                    {
                        _lossesMap[registersNames[i]] = DSSCircuit.Meters.RegisterValues[i];
                    }
                }
            }
        }

        /*
        [0]	kWh
        [1]	kvarh
        [2]	Max kW
        [3]	Max kVA
        [4]	Zone kWh
        [5]	Zone kvarh
        [6]	Zone Max kW
        [7]	Zone Max kVA
        [8]	Overload kWh Normal
        [9]	Overload kWh Emerg
        [10]	Load EEN
        [11]	Load UE
        [12]	Zone Losses kWh
        [13]	Zone Losses kvarh
        [14]	Zone Max kW Losses
        [15]	Zone Max kvar Losses
        [16]	Load Losses kWh
        [17]	Load Losses kvarh
        [18]	No Load Losses kWh
        [19]	No Load Losses kvarh
        [20]	Max kW Load Losses
        [21]	Max kW No Load Losses
        [22]	Line Losses
        [23]	Transformer Losses
        [24]	Line Mode Line Losses
        [25]	Zero Mode Line Losses
        [26]	3-phase Line Losses
        [27]	1- and 2-phase Line Losses
        [28]	Gen kWh
        [29]	Gen kvarh
        [30]	Gen Max kW
        [31]	Gen Max kVA
        [32]	13.8 kV Losses
        [33]	0.22 kV Losses
        [34]	34.5 kV Losses
        [35]	Aux1
        [36]	Aux6
        [37]	Aux11
        [38]	Aux16
        [39]	13.8 kV Line Loss
        [40]	0.22 kV Line Loss
        [41]	34.5 kV Line Loss
        [42]	Aux2
        [43]	Aux7
        [44]	Aux12
        [45]	Aux17
        [46]	13.8 kV Load Loss
        [47]	0.22 kV Load Loss
        [48]	34.5 kV Load Loss
        [49]	Aux3
        [50]	Aux8
        [51]	Aux13
        [52]	Aux18
        [53]	13.8 kV No Load Loss
        [54]	0.22 kV No Load Loss
        [55]	34.5 kV No Load Loss
        [56]	Aux4
        [57]	Aux9
        [58]	Aux14
        [59]	Aux19
        [60]	13.8 kV Load Energy
        [61]	0.22 kV Load Energy
        [62]	34.5 kV Load Energy
        [63]	Aux5
        [64]	Aux10
        [65]	Aux15
        [66]	Aux20
        */

        // verifica convergencia, de acordo com 3 criterios
        internal bool VerificaConvergencia()
        {
            // verifica se excedeu a geracao maxima
            if ( ExcedeuGeracaoMaxima() || ExcedeuRequisitoMaximo() )
            {
                _convergiuBool = false;
            } 
            else
            {
                _convergiuBool = true;
            }
            return _convergiuBool;
        }

        //calcula resultado ano
        internal void CalculaResAno(List<PFResults> lstResultadoFluxo, string alim, string arquivo, MainWindow jan)
        {
            // obtem 1mês
            PFResults res1 = lstResultadoFluxo.First();

            //usa variavel da classe para armazenar a soma
            _energyMeter = res1._energyMeter;
                 
            //remove     
            lstResultadoFluxo.Remove(res1);

            // obtem medidores do 2mes em diante e soma com o 1mes
            foreach (PFResults res in lstResultadoFluxo) 
            {
                //medidor 
                MyEnergyMeter emMes = res._energyMeter;
            
                //soma
                _energyMeter.Soma(emMes);
            }

            // cria string com o formato de saida das perdas
            string conteudo = _energyMeter.FormataResultado(alim);

            // grava perdas alimentador em arquivo 
            TxtFile.GravaEmArquivo(conteudo, arquivo, jan);
        }
    }     
}


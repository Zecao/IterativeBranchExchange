//#define ENGINE
#if ENGINE
using OpenDSSengine;
#else
using dss_sharp;
#endif

using ExecutorOpenDSS.Classes;
using System.Collections.Generic;
using System.IO;
using ExecutorOpenDSS.Classes_Auxiliares;
using System;

namespace ExecutorOpenDSS.Classes_Principais
{
    class FluxoDiario
    {
        private ParamGeraisDSS _paramGerais;
        private MainWindow _janela;
        private bool _DSSObjCarregadoOtm;
        private string _nomeAlim;
        private List<string> _lstCommandsDSS;
        private List<int> _lstOfIndexModeloDeCarga = new List<int>();

        private bool _soMT;
        private string _tipoDia = "DU"; 

        public ResultadoFluxo _resFluxo = new ResultadoFluxo();
        public ObjDSS _oDSS;

        public FluxoDiario(ParamGeraisDSS paramGerais, MainWindow janela, ObjDSS objDSS, string tipoDia, bool soMT = false)
        {
            //
            InitVars(paramGerais,  janela,  objDSS, soMT);

            //
            _tipoDia = tipoDia;
        }

        public FluxoDiario(ParamGeraisDSS paramGerais, MainWindow janela, ObjDSS objDSS, bool soMT = false)
        {
            InitVars(paramGerais, janela, objDSS, soMT);
        }

        // inicializa variaveis da classe 
        internal void InitVars(ParamGeraisDSS paramGerais, MainWindow janela, ObjDSS objDSS, bool soMT)
        {
            // variaveis da classe
            _paramGerais = paramGerais;
            _janela = janela;
            _oDSS = objDSS;

            //_oDSS._DSSObj.Reset();

            // TODO FIX ME da pau quando executa a segunda vez
            // OBS: datapath setado por alim
            _oDSS._DSSObj.DataPath = _paramGerais.getDataPathAlimOpenDSS();

            // TODO testando
            _oDSS.SetActiveCircuit();

            // nome alim
            _nomeAlim = _paramGerais.getNomeAlimAtual();

            // seta variavel
            _soMT = soMT;
        }

        //
        internal void AjustaTapsRTs(int Vreg)
        {
            //iterador
            int iReg = _oDSS._DSSCircuit.RegControls.First;

            while (iReg !=0 )
            {
                _oDSS._DSSCircuit.RegControls.ForwardVreg = Vreg;

                // itera
                iReg = _oDSS._DSSCircuit.RegControls.Next;
            }            
        }

        /* //DEBUG
        static bool modPFunc(string str)
        {
            return str.Equals("new load.M2constZ pf = 0.92,Vminpu = 0.92,Vmaxpu = 1.5,model = 2,status = variable");
        }
        */

        // altera modelo de carga
        public void SetModeloDeCarga(List<string> novoModeloCarga)
        {
            //DEBUG
            //System.Predicate<string> modeloP = modPFunc;
            //string debug = _lstCommandsDSS.Find(modeloP);

            //DEBUG
            //string debug = _lstCommandsDSS.Find( str => str.Equals("new load.M2constZ pf = 0.92,Vminpu = 0.92,Vmaxpu = 1.5,model = 2,status = variable") );

            _lstCommandsDSS[_lstOfIndexModeloDeCarga[0]] = novoModeloCarga[0];
            _lstCommandsDSS[_lstOfIndexModeloDeCarga[1]] = novoModeloCarga[1];
        }

        // Carrega Alimentador
        public bool CarregaAlimentador()
        {
            bool ret;

            // cria arquivo
            if (_soMT)
            {
                ret = CarregaArquivoDSS_SoMT();
            }
            else
            {
                ret = CarregaArquivoDSSemMemoria();
            }
            return ret;
        }

        // Cria string com o arquivo DSS na memoria
        private bool CarregaArquivoDSSemMemoria()
        {
            _lstCommandsDSS = new List<string>();

            // limpa circuito de objeto recem criado
            _lstCommandsDSS.Add("clear");

            // nome arquivo dss completo
            string nomeArqDSScomp = _paramGerais.getNomeArquivoAlimentadorDSS();

            //Verifica existencia do arquivo DSS
            if (File.Exists(nomeArqDSScomp))
            {
                // Obtem linhas do arquivo cabecalho
                string[] lines = System.IO.File.ReadAllLines(nomeArqDSScomp);

                // TODO refactory. Da pau caso tenha so uma linha. 

                // Obtem 2 linha do arquivo  + nivel de tensao em PU
                _lstCommandsDSS.Add(lines[1] + _paramGerais.GetTensaoSaidaSE());
            }
            else
            {
                _janela.ExibeMsgDisplayMW(_nomeAlim + ": Arquivos *.dss não encontrados");

                return false;
            }

            // Redirect arquivo Curva de Carga, OBS: de acordo com o TIPO do dia 
            if (File.Exists(_paramGerais.getNomeEPathCurvasTxtCompleto()))
            {
                _paramGerais._parGUI._tipoDia = _tipoDia;
                _lstCommandsDSS.Add("Redirect " + _paramGerais.getNomeEPathCurvasTxtCompleto());
            }

            // condutores
            {
                _lstCommandsDSS.Add("Redirect " + _paramGerais.GetNomeArqCondutor());
            }

            string dir = _paramGerais.getDiretorioAlim();

            // SegmentosMT
            string nArqSegmentosMT = _nomeAlim + "SegmentosMT.dss";
            if (File.Exists(dir + nArqSegmentosMT))
            {
                _lstCommandsDSS.Add("Redirect " + nArqSegmentosMT);
            }

            // ChavesMT
            string nArqChavesMT = _nomeAlim + "ChavesMT.dss";
            if (File.Exists(dir + nArqChavesMT))
            {
                _lstCommandsDSS.Add("Redirect " + nArqChavesMT);
            }

            // PTs
            string nArqPTs = _nomeAlim + "PTs.dss";
            if (File.Exists(dir + nArqPTs))
            {
                _lstCommandsDSS.Add("Redirect " + nArqPTs);
            }

            // Reguladores.dss
            string nArqRTs = _nomeAlim + "Reguladores.dss";
            if (File.Exists(dir + nArqRTs))
            {
                _lstCommandsDSS.Add("Redirect " + nArqRTs);
            }

            // Transformadores
            string nArqTrafo = _nomeAlim + "Transformadores.dss";
            if (File.Exists(dir + nArqTrafo))
            {
                _lstCommandsDSS.Add("Redirect " + nArqTrafo);
            }

            // SegmentosBT
            string nArqSegmentosBT = _nomeAlim + "SegmentosBT.dss";
            if (File.Exists(dir + nArqSegmentosBT))
            {
                _lstCommandsDSS.Add("Redirect " + nArqSegmentosBT);
            }

            // Ramais
            string nArqRamais = _nomeAlim + "Ramais.dss";
            if (File.Exists(dir + nArqRamais))
            {
                _lstCommandsDSS.Add("Redirect " + nArqRamais);
            }

            // redirect arquivo CargaMT
            if (File.Exists(dir + _paramGerais.getNomeCargaMT_mes()))
            {
                _lstCommandsDSS.Add("Redirect " + _paramGerais.getNomeCargaMT_mes());
            }

            // se modelo carga Cemig 
            if (_paramGerais._parAvan._modeloCargaCemig)
            {
                // redirect arquivo CargaBT
                if (File.Exists(dir + _paramGerais.getNomeCargaBTCemig_mes()))
                {
                    // armazena indice do modelo de Carga 
                    _lstOfIndexModeloDeCarga.Add(_lstCommandsDSS.Count);

                    // adiciona comando de definicao do MOdelo de Carga. Defaul 50% Z
                    _lstCommandsDSS.Add("new load.M2constZ pf = 0.92,Vminpu = 0.92,Vmaxpu = 1.5,model = 2,status = variable");

                    // armazena indice do modelo de Carga 
                    _lstOfIndexModeloDeCarga.Add(_lstCommandsDSS.Count);

                    // adiciona comando de definicao do MOdelo de Carga. Defaul 50% Z
                    _lstCommandsDSS.Add("new load.M3constPsqQ pf = 0.92,Vminpu = 0.92,Vmaxpu = 1.5,model = 3,status = variable");

                    _lstCommandsDSS.Add("Redirect " + _paramGerais.getNomeCargaBTCemig_mes());
                }
            }
            else //MODELO antigo
            {
                // redirect arquivo CargaBT
                if (File.Exists(dir + _paramGerais.getNomeCargaBT_mes()))
                {
                    _lstCommandsDSS.Add("Redirect " + _paramGerais.getNomeCargaBT_mes());
                }
            }

            // redirect arquivo GeradorMT
                if (File.Exists(dir + _paramGerais.getNomeGeradorMT_mes()))
            {
                _lstCommandsDSS.Add("Redirect " + _paramGerais.getNomeGeradorMT_mes());
            }

            // redirect arquivo Capacitor
            if (_paramGerais._parAvan._incluirCapMT && File.Exists(dir + _paramGerais.getNomeCapacitorMT()))
            {
                _lstCommandsDSS.Add("Redirect " + _paramGerais.getNomeCapacitorMT());
            }

            // nome arquivo dss completo
            string nomeArqBcomp = _paramGerais.getNomeArquivoB();

            // adiciona comandos secundários
            if (File.Exists(dir + nomeArqBcomp))
            {
                // le linhas do arquivo "B" e adiciona na lista de comandos
                string[] tmp2 = System.IO.File.ReadAllLines(dir + nomeArqBcomp);

                // 
                for (int i = 0; i < tmp2.Length; i++)
                {
                    string tmp = tmp2[i];

                    // skipa linhas vazias
                    if (tmp.Equals(""))
                        continue;

                    // skipa linhas de comentario
                    if (tmp.StartsWith("!"))
                        continue;

                    // adiciona na lista de comandos
                    _lstCommandsDSS.Add(tmp);
                }
            }
            else 
            {
                _janela.ExibeMsgDisplayMW("Arquivo " + nomeArqBcomp + " não encontrado");
                return false;
            }


            return true;
        }

        // Cria string com o arquivo DSS na memoria
        private bool CarregaArquivoDSS_SoMT()
        {
            _lstCommandsDSS = new List<string>();

            // limpa circuito de objeto recem criado
            _lstCommandsDSS.Add("clear");

            // nome arquivo dss completo
            string nomeArqDSScomp = _paramGerais.getNomeArquivoAlimentadorDSS();

            //Verifica existencia do arquivo DSS
            if (File.Exists(nomeArqDSScomp))
            {
                // Obtem linhas do arquivo cabecalho
                string[] lines = System.IO.File.ReadAllLines(nomeArqDSScomp);

                // TODO refactory. Da pau caso tenha so uma linha. 

                // Obtem 2 linha do arquivo  + nivel de tensao em PU
                _lstCommandsDSS.Add(lines[1] + _paramGerais.GetTensaoSaidaSE());
            }
            else
            {
                _janela.ExibeMsgDisplayMW(_nomeAlim + ": Arquivos *.dss não encontrados");

                return false;
            }

            // Redirect arquivo Curva de Carga, OBS: de acordo com o TIPO do dia 
            if (File.Exists(_paramGerais.getNomeEPathCurvasTxtCompleto()))
            {
                _lstCommandsDSS.Add("Redirect " + _paramGerais.getNomeEPathCurvasTxtCompleto());
            }

            // redirect condutores
            {
                _lstCommandsDSS.Add("Redirect " + "..\\PermRes\\Condutores.dss");
            }

            string dir = _paramGerais.getDiretorioAlim();

            // SegmentosMT
            string nArqSegmentosMT = _nomeAlim + "SegmentosMT.dss";
            if (File.Exists(dir + nArqSegmentosMT))
            {
                _lstCommandsDSS.Add("Redirect " + nArqSegmentosMT);
            }

            // ChavesMT
            string nArqChavesMT = _nomeAlim + "ChavesMT.dss";
            if (File.Exists(dir + nArqChavesMT))
            {
                _lstCommandsDSS.Add("Redirect " + nArqChavesMT);
            }

            // PTs
            string nArqPTs = _nomeAlim + "PTs.dss";
            if (File.Exists(dir + nArqPTs))
            {
                _lstCommandsDSS.Add("Redirect " + nArqPTs);
            }

            // Reguladores.dss
            string nArqRTs = _nomeAlim + "Reguladores.dss";
            if (File.Exists(dir + nArqRTs))
            {
                _lstCommandsDSS.Add("Redirect " + nArqRTs);
            }

            // Transformadores
            string nArqTrafo = _nomeAlim + "Transformadores.dss";
            if (File.Exists(dir + nArqTrafo))
            {
                _lstCommandsDSS.Add("Redirect " + nArqTrafo);
            }

            // redirect arquivo CargaMT
            if (File.Exists(dir + _paramGerais.getNomeCargaMT_mes()))
            {
                _lstCommandsDSS.Add("Redirect " + _paramGerais.getNomeCargaMT_mes());
            }

            // redirect arquivo GeradorMT
            if (File.Exists(dir + _paramGerais.getNomeGeradorMT_mes()))
            {
                _lstCommandsDSS.Add("Redirect " + _paramGerais.getNomeGeradorMT_mes());
            }

            // redirect arquivo Capacitor
            if (_paramGerais._parAvan._incluirCapMT && File.Exists(dir + _paramGerais.getNomeCapacitorMT()))
            {
                _lstCommandsDSS.Add("Redirect " + _paramGerais.getNomeCapacitorMT());
            }

            // nome arquivo dss completo
            string nomeArqBcomp = _paramGerais.getNomeArquivoB();

            // le linhas do arquivo "B" e adiciona na lista de comandos
            string[] tmp2 = System.IO.File.ReadAllLines(dir + nomeArqBcomp);

            // 
            for (int i = 0; i < tmp2.Length; i++)
            {
                string tmp = tmp2[i];

                // skipa linhas vazias
                if (tmp.Equals(""))
                    continue;

                // skipa linhas de omentario
                if (tmp.StartsWith("!"))
                    continue;

                // adiciona na lista de comandos
                _lstCommandsDSS.Add(tmp);
            }

            //
            return true;
        }

        // executa fluxo mensal Simples 
        internal bool ExecutaFluxoMensalSimples()
        {
            // carrega objeto OpenDSS
            CarregaDSS();

            bool ret = false;

            // Executa fluxo
            ret = ExecutaFluxoDiarioOpenDSSPvt(null);

            //informa usuario convergencia
            if (ret)
            {
                _janela.ExibeMsgDisplayMW(getMsgConvergencia(null, _nomeAlim));
            }
            return ret;
        }

        // Executa fluxo diario
        public bool ExecutaFluxoDiario()
        {
            //Verifica se foi solicitado o cancelamento.
            if (_janela._cancelarExecucao)
            {
                return false;
            }

            // carrega objeto OpenDSS
            CarregaDSS();

            // variavel de retorno;
            bool ret = false;

            // _modoFluxo
            // TODO : SE modo _calcDRPDRC ou _calcTensaoBarTrafo necessario passar a hora 
            if (_paramGerais._parGUI._tipoFluxo.Equals("Hourly"))
            {
                ret = ExecutaFluxoDiarioOpenDSSPvt(_paramGerais._parGUI._hora);
            }
            else
            {
                ret = ExecutaFluxoDiarioOpenDSSPvt(null);
            }

            //informa usuario convergencia
            if (ret)
            {
                _janela.ExibeMsgDisplayMW(getMsgConvergencia(null, _nomeAlim));
            }
            return ret;
        }

        // Executa comando no objDSS
        internal void CarregaDSS()
        {
            // se esta no modo otimiza && aproximacao diario, carrega alimenador somente 1 vez
            if (_paramGerais._parGUI._otmPorEnergia && _paramGerais._parGUI.GetAproximaFluxoMensalPorDU()) //_paramGerais._parGUI._otmPorDemMax)
            {
                CarregaDSSOtm();
            }
            else
            {
                CarregaDSSPvt();
            }
        }

        // Executa comando no objDSS
        internal void CarregaDSSPvt()
        {
            // carrega objeto OpenDSS
            foreach (string comando in _lstCommandsDSS)
            {
                _oDSS._DSSText.Command = comando;
            }
        }

        // Executa comando no objDSS
        internal void CarregaDSSOtm()
        {
            // Se NAO carregou DSSObj (E esta no modo otimiza), carrega somente 1 vez
            if (!_DSSObjCarregadoOtm)
            {
                // carrega objeto OpenDSS
                CarregaDSSPvt();

                // seta variavel booleana
                _DSSObjCarregadoOtm = true;
            }
        }

        // Executa fluxo Snap
        public bool ExecutaFluxoSnap()
        {
            // carrega alimentador no objDSS
            CarregaDSS();

            // Executa fluxo Snap PVT
            bool ret = ExecutaFluxoSnapPvt();

            //informa usuario convergencia
            if (ret)
            {
                _janela.ExibeMsgDisplayMW(getMsgConvergencia(null, _nomeAlim));
            }

            //Plota perdas na tela
            _janela.ExibeMsgDisplayMW(_resFluxo.getResultadoFluxoToConsole(
                _paramGerais.GetTensaoSaidaSE(), _paramGerais.getNomeAlimAtual()));

            return ret;
        }

        // Executa fluxo snap 
        private bool ExecutaFluxoSnapPvt()
        {
            // Interfaces
            Circuit DSSCircuit = _oDSS._DSSObj.ActiveCircuit;
            Solution DSSSolution = _oDSS._DSSObj.ActiveCircuit.Solution;

            // realiza ajuste das cargas 
            DSSSolution.LoadMult = _paramGerais.GetLoadMult(_janela);

            // seta algorithm Normal ou Newton
            _oDSS._DSSText.Command = "Set Algorithm = " + _paramGerais._tipoFluxo;

            // seta modo snap.
            _oDSS._DSSText.Command = "Set mode=snap";

            // resolve circuito 
            DSSSolution.Solve();

            if (DSSCircuit.Solution.Converged)
            {
                // Obtem dados para o medidor 
                _oDSS._DSSText.Command = "energymeter.carga.action=take";

                // Obtem valores de pot e energia dos medidores 
                GetValoresEnergyMeter();

                // verifica saida e grava perdas em arquivo OU alimentador que nao tenha convergido 
                GravaPerdasArquivo();

                return true;
            }
            return false;
        }

        //Tradução da função executaFluxoDiarioOpenDSSNativoOpenDSS
        private bool ExecutaFluxoDiarioOpenDSSPvt(string hora)
        {
            //% Interfaces
            Circuit DSSCircuit = _oDSS._DSSObj.ActiveCircuit;
            Solution DSSSolution = _oDSS._DSSCircuit.Solution;

            //get loadMult da struct paramGerais
            double loadMult = _paramGerais.GetLoadMult(_janela);
            DSSSolution.LoadMult = loadMult;

            // Seta modo diario 
            if (hora == null)
            {
                _oDSS._DSSText.Command = "Set mode=daily  hour=0 number=24 stepsize=1h";
            }
            else
            {
                _oDSS._DSSText.Command = "Set mode=daily hour=" + hora + " number=1 stepsize=1h";
            }

            // resolve circuito 
            DSSSolution.Solve();

            // se nao convergiu, retorna
            if (!DSSCircuit.Solution.Converged)
            {
                return false;
            }

            //DEBUG 
            double debug = DSSCircuit.Meters.RegisterValues[2];

            // grava valores do EnergyMeter 
            bool ret = GetValoresEnergyMeter();

            if (ret)
            {
                _resFluxo._energyMeter.GravaLoadMult(loadMult);
            }

            // se valores EnergyMeter estao consistentes
            if (ret)
            {
                // verifica saida e grava perdas em arquivo OU alimentador que nao tenha convergido 
                GravaPerdasArquivo();

                // verifica geracao de relatorios
                GeraRelatorios();
            }
            return ret;
        }

        // 
        private void GeraRelatorios()
        {
            if (_paramGerais._parAvan._calcDRPDRC)
            {
                //
                CalculaDRPDRC();
            }

            // calcula tensao PU no primario trafos
            if (_paramGerais._parAvan._calcTensaoBarTrafo)
            {
                // obtem indices de tensao nos trafos
                AnaliseIndiceTensaoTrafo obj2 = new AnaliseIndiceTensaoTrafo(_oDSS._DSSText, _oDSS._DSSObj.ActiveCircuit, _paramGerais);
                obj2.PlotaNiveisTensaoBarras(_janela);
            }

            // verifica cargas isoladas
            if (_paramGerais._parAvan._verifCargaIsolada)
            {
                //analise cargas isoladas
                AnaliseCargasIsoladas obj = new AnaliseCargasIsoladas(_oDSS._DSSObj.ActiveCircuit, _paramGerais);
                obj.PlotaCargasIsoladasArq(_janela);
            }

            // TODO criar interface
            //queryLineCode(_DSSObj.ActiveCircuit);

            // TODO criar interface 
            //queryLine(_DSSObj.ActiveCircuit);
        }

        // get mensagem convergencia 
        public string getMsgConvergencia(string hora, string nomeAlim)
        {
            string str;

            if (hora != null)
            {
                str = nomeAlim + " Hour: " + add1toHour(hora) + " -> Solution Converged";
            }
            else
            {
                str = nomeAlim + " -> Solution Converged";
            }
            return str;
        }

        //adiciona 1hora a string hora
        private static string add1toHour(string hora)
        {
            int dHora = int.Parse(hora);
            dHora++;
            return dHora.ToString();
        }

        // verifica saida e grava perdas em arquivo OU alimentador que nao tenha convergido
        public void GravaPerdasArquivo()
        {
            //Se modo otmiza não grava arquivo
            if (_paramGerais._parAvan._otimizaPUSaidaSE || _paramGerais._parGUI._otmPorEnergia || _paramGerais._parGUI._otmPorDemMax)
            {
                return;
            }

            // Se alim Nao Convergiu 
            if (!_resFluxo._convergiuBool)
            {
                //Grava a lista de alimentadores não convergentes em um txt
                ArqManip.gravaLstAlimNaoConvergiram(_paramGerais, _janela);
            }
            // Grava Perdas de acordo com o tipo de fluxo
            else
            {
                string nomeAlim = _paramGerais.getNomeAlimAtual();

                // obtem o nome do arquivo de perdas, conforme o tipo do fluxo 
                string arquivo = _paramGerais.getNomeArquivoPerdas();

                // Grava Perdas
                ArqManip.GravaPerdas(_resFluxo, nomeAlim, arquivo, _janela);
            }
        }

        // Calcula DRP e DRC
        private void CalculaDRPDRC()
        {
            // Interfaces
            Circuit DSSCircuit = _oDSS._DSSObj.ActiveCircuit;
            Text DSSText = _oDSS._DSSObj.Text;

            // se convergiu 
            if (DSSCircuit.Solution.Converged)
            {
                // cria objeto indice tensao
                AnaliseIndiceTensao indTensao = new AnaliseIndiceTensao(DSSCircuit, DSSText);

                // Calcula num Clientes com DRP e DRC 
                indTensao.CalculaNumClientesDRPDRC();

                // grava em arquivo
                indTensao.imprimeNumClientesDRPDRC(_paramGerais, _janela);

                //
                indTensao.imprimeBarrasDRPDRC(_paramGerais, _janela);
            }
        }

        /*
         *SampleEnergyMeters ={YES/TRUE | NO/FALSE} 
         
        Overrides default value for sampling EnergyMeter objects at the end of the solution loop. Normally Time and Duty modes do not
        sample EnergyMeters whereas Daily, Yearly, M!, M2, LD1 and LD2 modes do. 

        Use this Option to turn sampling on or off.
          */
        // Tradução da Função gravaValoresEnergyMeter
        public bool GetValoresEnergyMeter()
        {
            // preenche saida com as perdas do alimentador e verifica se dados estao corretos (ie. convergencia)
            bool ret = _resFluxo.GetPerdasAlim(_oDSS._DSSObj.ActiveCircuit);

            // verifica geracao das usinas (i.e. se estao conectadas)
            verifGerUsinasGDMT();

            return ret;
        }

        // verifica se usina do alimentador gerou energia, informando no display da tela.
        private void verifGerUsinasGDMT()
        {
            // Se existe gerador, obtem gerador
            if (File.Exists(_paramGerais.getDiretorioAlim() + _paramGerais.getNomeGeradorMT_mes()))
            {
                // para cada gerador do alimentador
                do
                {
                    string[] nomeGen = _oDSS._DSSCircuit.Generators.AllNames;

                    //
                    string[] genRegisterNames = _oDSS._DSSCircuit.Generators.RegisterNames;

                    // para cada gerador
                    double[] genRegisterValues = _oDSS._DSSCircuit.Generators.RegisterValues;

                    if (genRegisterValues == null)
                    {
                        _janela.ExibeMsgDisplayMW("Usina desconectada!");
                    }

                } while (_oDSS._DSSCircuit.Generators.Next != 0);

            }
        }

        // TODO refactory 
        private void queryLineCode(Circuit dssCircuit)
        {
            List<string> lstCabos = new List<string>();
            lstCabos.Add("CAB102");
            lstCabos.Add("CAB103");
            lstCabos.Add("CAB104");
            lstCabos.Add("CAB107");
            lstCabos.Add("CAB108");
            lstCabos.Add("CAB202");
            lstCabos.Add("CAB203");
            lstCabos.Add("CAB204");
            lstCabos.Add("CAB207");
            lstCabos.Add("CAB208");
            lstCabos.Add("CABA06");
            lstCabos.Add("CABA08");
            lstCabos.Add("CAB2021");
            lstCabos.Add("CAB1031");
            lstCabos.Add("CAB1021");
            lstCabos.Add("CAB2031");
            lstCabos.Add("CABA061");
            lstCabos.Add("CABBT106");
            lstCabos.Add("CABBT107");
            lstCabos.Add("CABBT108");
            lstCabos.Add("CABBT803");
            lstCabos.Add("CABBT805");
            lstCabos.Add("CABBT809");
            lstCabos.Add("CABBT810");
            lstCabos.Add("CABBT801");
            lstCabos.Add("CABBT807");
            lstCabos.Add("CABBT808");

            Text textDSS = _oDSS._DSSObj.Text;

            List<string> resRmatrix = new List<string>();
            List<string> resXmatrix = new List<string>();

            foreach (string lineCode in lstCabos)
            {
                textDSS.Command = "? LineCode." + lineCode + ".Rmatrix";

                resRmatrix.Add(lineCode + "\tRmatrix=" + textDSS.Result);

                textDSS.Command = "? LineCode." + lineCode + ".Xmatrix";

                resXmatrix.Add("\tXmatrix=" + textDSS.Result);
            }

            ArqManip.GravaListArquivoTXT(resRmatrix, _paramGerais.getArqRmatrix(), _janela);

            ArqManip.GravaListArquivoTXT(resXmatrix, _paramGerais.getArqXmatrix(), _janela);
        }

        // TODO refactory 
        private void queryLine(Circuit dssCircuit)
        {
            List<string> lstLinhas = new List<string>();
            lstLinhas.Add("CAB102");
            lstLinhas.Add("CAB103");
            lstLinhas.Add("CAB104");
            lstLinhas.Add("CAB107");
            lstLinhas.Add("CAB108");
            lstLinhas.Add("CAB202");
            lstLinhas.Add("CAB203");
            lstLinhas.Add("CAB204");
            lstLinhas.Add("CAB207");
            lstLinhas.Add("CAB208");
            lstLinhas.Add("CABA06");
            lstLinhas.Add("CABA08");
            lstLinhas.Add("CABBT106");
            lstLinhas.Add("CABBT107");
            lstLinhas.Add("CABBT108");
            lstLinhas.Add("CABBT803");
            lstLinhas.Add("CABBT805");
            lstLinhas.Add("CABBT809");
            lstLinhas.Add("CABBT810");
            lstLinhas.Add("CABBT801");
            lstLinhas.Add("CABBT807");
            lstLinhas.Add("CABBT808");

            Text textDSS = _oDSS._DSSObj.Text;

            List<string> resRmatrix = new List<string>();
            List<string> resXmatrix = new List<string>();

            foreach (string line in lstLinhas)
            {
                textDSS.Command = "? Line." + line + ".Rmatrix";

                resRmatrix.Add(line + "\tRmatrix=" + textDSS.Result);

                textDSS.Command = "? Line." + line + ".Xmatrix";

                resXmatrix.Add(line + "\tXmatrix=" + textDSS.Result);
            }

            ArqManip.GravaListArquivoTXT(resRmatrix, _paramGerais.getArqRmatrix(), _janela);

            ArqManip.GravaListArquivoTXT(resXmatrix, _paramGerais.getArqXmatrix(), _janela);
        }
    }
}

#define ENGINE
#if ENGINE
using OpenDSSengine;
#else
using dss_sharp;
#endif

using ExecutorOpenDSS.Classes;
using System.Collections.Generic;
using System.IO;
using ExecutorOpenDSS.ClassesPrincipais;

namespace ExecutorOpenDSS.Classes_Principais
{
    class DailyFlow
    {
        // TODO mover para outra classe
        private GeneralParameters _paramGerais;
        private MainWindow _janela;
        public ObjDSS _oDSS;

        private bool _DSSObjCarregadoOtm;
        private readonly bool _soMT;
        private readonly string _nomeAlim;
        private string _tipoDia = "DU";
        private List<string> _lstCommandsDSS;
        public PFResults _resFluxo = new PFResults();

        // TODO refactory 
        private List<int> _lstOfIndexModeloDeCarga = new List<int>();

        public DailyFlow(GeneralParameters paramGerais, MainWindow janela, ObjDSS objDSS, string tipoDia ="DU", bool soMT = false)
        {
            // variaveis da classe
            _paramGerais = paramGerais;
            _janela = janela;
            _oDSS = objDSS;

            // TODO FIX ME da pau quando executa a segunda vez
            // OBS: datapath setado por alim
            string temp = _paramGerais.GetDataPathAlimOpenDSS();
            _oDSS._DSSObj.DataPath = temp;

            // nome alim
            _nomeAlim = _paramGerais.GetNomeAlimAtual();

            // seta variavel
            _soMT = soMT;

            // TODO 
            SetTipoDia(paramGerais._parGUI);
        }

        private void SetTipoDia(GUIParameters parGUI)
        {
            // so faz associacaose tipo fluxo for igual da daily ou hourly 
            if (parGUI._tipoFluxo.Equals("Daily") || parGUI._tipoFluxo.Equals("Hourly"))
            {
                switch (parGUI._tipoDia)
                {
                    case "Sábado":
                        _tipoDia = "SA";
                        break;
                    case "Domingo":
                        _tipoDia = "DO";
                        break;
                    default:
                        _tipoDia = "DU";
                        break;
                }
            }
        }

        //
        internal void AjustaTapsRTs(int Vreg)
        {
            //iterador
            int iReg = _oDSS.GetActiveCircuit().RegControls.First;

            while (iReg !=0 )
            {
                _oDSS.GetActiveCircuit().RegControls.ForwardVreg = Vreg;

                // itera
                iReg = _oDSS.GetActiveCircuit().RegControls.Next;
            }            
        }

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
            _lstCommandsDSS = new List<string>
            {
                "clear" // limpa circuito de objeto recem criado
            };

            // nome arquivo dss completo
            string nomeArqDSScomp = _paramGerais.GetNomeArquivoAlimentadorDSS();

            //Verifica existencia do arquivo DSS
            if (File.Exists(nomeArqDSScomp))
            {
                // Obtem linhas do arquivo cabecalho
                string[] lines = System.IO.File.ReadAllLines(nomeArqDSScomp);

                // TODO refactory. Da pau caso tenha so uma linha. 
                _lstCommandsDSS.Add(lines[1]);
            }
            else
            {
                _janela.ExibeMsgDisplayMW(_nomeAlim + ": Arquivos *.dss não encontrados");

                return false;
            }

            // Redirect arquivo Curva de Carga, OBS: de acordo com o TIPO do dia 
            if (File.Exists(_paramGerais.GetNomeEPathCurvasTxtCompleto(_tipoDia)))
            {
                _lstCommandsDSS.Add("Redirect " + _paramGerais.GetNomeEPathCurvasTxtCompleto(_tipoDia));
            }

            // condutores
            {
                _lstCommandsDSS.Add("Redirect " + _paramGerais.GetNomeArqCondutor());
            }

            string dir = _paramGerais.GetDiretorioAlim();

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
            if (File.Exists(dir + _paramGerais.GetNomeCargaMT_mes()))
            {
                _lstCommandsDSS.Add("Redirect " + _paramGerais.GetNomeCargaMT_mes());
            }

            // se modelo carga Cemig 
            if (_paramGerais._parAvan._modeloCargaCemig)
            {
                // redirect arquivo CargaBT
                if (File.Exists(dir + _paramGerais.GetNomeCargaBTCemig_mes()))
                {
                    // armazena indice do modelo de Carga 
                    _lstOfIndexModeloDeCarga.Add(_lstCommandsDSS.Count);

                    // adiciona comando de definicao do MOdelo de Carga. Defaul 50% Z
                    _lstCommandsDSS.Add("new load.M2constZ pf = 0.92,Vminpu = 0.92,Vmaxpu = 1.5,model = 2,status = variable");

                    // armazena indice do modelo de Carga 
                    _lstOfIndexModeloDeCarga.Add(_lstCommandsDSS.Count);

                    // adiciona comando de definicao do MOdelo de Carga. Defaul 50% Z
                    _lstCommandsDSS.Add("new load.M3constPsqQ pf = 0.92,Vminpu = 0.92,Vmaxpu = 1.5,model = 3,status = variable");

                    _lstCommandsDSS.Add("Redirect " + _paramGerais.GetNomeCargaBTCemig_mes());
                }
            }
            else //MODELO antigo
            {
                // redirect arquivo CargaBT
                if (File.Exists(dir + _paramGerais.GetNomeCargaBT_mes()))
                {
                    _lstCommandsDSS.Add("Redirect " + _paramGerais.GetNomeCargaBT_mes());
                }
            }

            // redirect arquivo GeradorMT
            if (File.Exists(dir + _paramGerais.GetNomeGeradorMT_mes()))
            {
                _lstCommandsDSS.Add("Redirect " + _paramGerais.GetNomeGeradorMT_mes());
            }

            // redirect arquivo Capacitor
            if (_paramGerais._parAvan._incluirCapMT && File.Exists(dir + _paramGerais.GetNomeCapacitorMT()))
            {
                _lstCommandsDSS.Add("Redirect " + _paramGerais.GetNomeCapacitorMT());
            }

            // nome arquivo dss completo
            string nomeArqBcomp = _paramGerais.GetNomeArquivoB();

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

            // BatchEdit
            if ( ! _paramGerais._parAvan._strBatchEdit.Equals("") )
            {
                // adiciona na lista de comandos
                _lstCommandsDSS.Add(_paramGerais._parAvan._strBatchEdit);
            }

            return true;
        }

        // Cria string com o arquivo DSS na memoria
        private bool CarregaArquivoDSS_SoMT()
        {
            _lstCommandsDSS = new List<string>
            {
                // limpa circuito de objeto recem criado
                "clear"
            };

            // nome arquivo dss completo
            string nomeArqDSScomp = _paramGerais.GetNomeArquivoAlimentadorDSS();

            //Verifica existencia do arquivo DSS
            if (File.Exists(nomeArqDSScomp))
            {
                // Obtem linhas do arquivo cabecalho
                string[] lines = System.IO.File.ReadAllLines(nomeArqDSScomp);

                // TODO refactory. Da pau caso tenha so uma linha. 
                // Obtem 2 linha do arquivo  + nivel de tensao em PU
                _lstCommandsDSS.Add(lines[1]);
            }
            else
            {
                _janela.ExibeMsgDisplayMW(_nomeAlim + ": Arquivos *.dss não encontrados");

                return false;            
            }

            // Redirect arquivo Curva de Carga, OBS: de acordo com o TIPO do dia 
            if (File.Exists(_paramGerais.GetNomeEPathCurvasTxtCompleto(_tipoDia)))
            {
                _lstCommandsDSS.Add("Redirect " + _paramGerais.GetNomeEPathCurvasTxtCompleto(_tipoDia));
            }

            // condutores
            {
                _lstCommandsDSS.Add("Redirect " + _paramGerais.GetNomeArqCondutor());
            }

            string dir = _paramGerais.GetDiretorioAlim();

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

            // Reguladores.dss
            string nArqRTs = _nomeAlim + "Reguladores.dss";
            if (File.Exists(dir + nArqRTs))
            {
                _lstCommandsDSS.Add("Redirect " + nArqRTs);
            }

            /* //OBS: comentei ao debugar possivel erro em manobras em SEs inteira. A matriz de incidencia diminui em 1/6.
             * //OLD CODE
            // Transformadores
            string nArqTrafo = _nomeAlim + "Transformadores.dss";
            if (File.Exists(dir + nArqTrafo))
            {
                _lstCommandsDSS.Add("Redirect " + nArqTrafo);
            }*/

            // redirect arquivo CargaMT
            if (File.Exists(dir + _paramGerais.GetNomeCargaMT_mes()))
            {
                _lstCommandsDSS.Add("Redirect " + _paramGerais.GetNomeCargaMT_mes());
            }

            // redirect arquivo GeradorMT
            if (File.Exists(dir + _paramGerais.GetNomeGeradorMT_mes()))
            {
                _lstCommandsDSS.Add("Redirect " + _paramGerais.GetNomeGeradorMT_mes());
            }

            // redirect arquivo Capacitor
            if (_paramGerais._parAvan._incluirCapMT && File.Exists(dir + _paramGerais.GetNomeCapacitorMT()))
            {
                _lstCommandsDSS.Add("Redirect " + _paramGerais.GetNomeCapacitorMT());
            }

            // nome arquivo dss completo
            string nomeArqBcomp = _paramGerais.GetNomeArquivoB();

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

            // Executa fluxo
            bool ret = ExecutaFluxoDiarioOpenDSSPvt(null);

            //informa usuario convergencia
            if (ret)
            {
                _janela.ExibeMsgDisplayMW(GetMsgConvergencia(null, _nomeAlim));
            }
            return ret;
        }

        // Executa fluxo diario OU horario caso seja passado string hora
        public bool ExecutaFluxoDiario(string hora=null)
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
                _janela.ExibeMsgDisplayMW(GetMsgConvergencia(null, _nomeAlim));
            }
            return ret;
        }

        // Executa fluxo horario caso seja passado string hora
        public bool ExecutaFluxoHorario(string hora = null)
        {
            //Verifica se foi solicitado o cancelamento.
            if (_janela._cancelarExecucao)
            {
                return false;
            }

            // carrega objeto OpenDSS
            CarregaDSS();

            // variavel de retorno;
            bool ret = ExecutaFluxoDiarioOpenDSSPvt(hora);
            
            //informa usuario convergencia
            if (ret)
            {
                _janela.ExibeMsgDisplayMW(GetMsgConvergencia(null, _nomeAlim));
            }
            return ret;
        }

        // Executa fluxo horario caso seja passado string hora
        public bool ExecutaFluxoMaximaDiaria()
        {
            //Verifica se foi solicitado o cancelamento.
            if (_janela._cancelarExecucao)
            {
                return false;
            }

            // carrega objeto OpenDSS
            CarregaDSS();

            // variavel de retorno;
            bool ret = ExecutaFluxoDiarioOpenDSSPvt(null);

            //informa usuario convergencia
            if (ret)
            {
                _janela.ExibeMsgDisplayMW(GetMsgConvergencia(null, _nomeAlim));
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
                _janela.ExibeMsgDisplayMW(GetMsgConvergencia(null, _nomeAlim));
            }

            //nivel pu
            string nivelTensaoPU = _oDSS.GetActiveCircuit().Vsources.pu.ToString("0.###");

            //Plota perdas na tela
            _janela.ExibeMsgDisplayMW(_resFluxo.GetResultadoFluxoToConsole(
                nivelTensaoPU, _paramGerais.GetNomeAlimAtual()));

            return ret;
        }

        // Executa fluxo snap 
        private bool ExecutaFluxoSnapPvt()
        {
            // Interfaces
            Circuit DSSCircuit = _oDSS._DSSObj.ActiveCircuit;
            Solution DSSSolution = _oDSS._DSSObj.ActiveCircuit.Solution;

            // realiza ajuste das cargas 
            double loadMult = _paramGerais.GetLoadMult();

            if (loadMult == 0 )
            {
                _janela.ExibeMsgDisplayMW("LoadMult igual a 0");
                return false;
            }

            DSSSolution.LoadMult = loadMult;

            // usuario escolheu tensao barramento
            if (_paramGerais._parGUI._usarTensoesBarramento)
            {
                DSSCircuit.Vsources.pu = double.Parse(_paramGerais._parGUI._tensaoSaidaBarUsuario);
            }

            // TODO erro em reconfiguracao
            // seta algorithm Normal ou Newton
            _oDSS._DSSText.Command = "Set Algorithm = " + _paramGerais._AlgoritmoFluxo;

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
            Solution DSSSolution = _oDSS.GetActiveCircuit().Solution;

            //get loadMult da struct paramGerais
            double loadMult = _paramGerais.GetLoadMult();

            if (loadMult == 0)
            {
                _janela.ExibeMsgDisplayMW("LoadMult igual a 0");
                return false;
            }

            DSSSolution.LoadMult = loadMult;

            // usuario escolheu tensao barramento
            if (_paramGerais._parGUI._usarTensoesBarramento)
            {
                DSSCircuit.Vsources.pu = double.Parse(_paramGerais._parGUI._tensaoSaidaBarUsuario);
            }

            // TODO da erro no modulo reconfiguracao
            // TODO da erro caso tente rodar em alim inexistente.
            switch (_paramGerais._parGUI._tipoFluxo)
            {
                case "Hourly":
                    _oDSS._DSSText.Command = "Set mode=daily hour=" + hora + " number=1 stepsize=1h";
                    break;

                default: // "daily"
                    _oDSS._DSSText.Command = "Set mode=daily  hour=0 number=24 stepsize=1h";
                    break;
             }  

            // resolve circuito 
            DSSSolution.Solve();

            // se nao convergiu, retorna
            if (!DSSCircuit.Solution.Converged)
            {
                return false;
            }

            // grava valores do EnergyMeter 
            bool ret = GetValoresEnergyMeter();

            // se valores EnergyMeter estao consistentes
            if (ret)
            {
                _resFluxo._energyMeter.GravaLoadMult(loadMult);

                // verifica saida e grava perdas em arquivo OU alimentador que nao tenha convergido 
                GravaPerdasArquivo();

                // verifica geracao de relatorios
                GeraRelatorios();
            }
            return ret;
        }

        // relatorios modo Hourly
        private void GeraRelatorios()
        {
            // condicao de saida
            if (!_paramGerais._parGUI._tipoFluxo.Equals("Hourly"))
            {
                return;
            }

            if (_paramGerais._parAvan._verifTapsRTs)
            {
                //
                VoltageReguladorAnalysis obj = new VoltageReguladorAnalysis(_oDSS._DSSText, _oDSS._DSSObj.ActiveCircuit, _paramGerais);
                obj.PlotaTapRTs(_janela);
            }

            if (_paramGerais._parAvan._calcDRPDRC)
            {
                //
                CalculaDRPDRC();
            }

            // calcula tensao PU no primario trafos
            if (_paramGerais._parAvan._calcTensaoBarTrafo)
            {
                // obtem indices de tensao nos trafos
                TransformerVoltageLevelAnalysis obj2 = new TransformerVoltageLevelAnalysis(_oDSS._DSSText, _oDSS._DSSObj.ActiveCircuit, _paramGerais);
                obj2.PlotaNiveisTensaoBarras(_janela);
            }

            // verifica cargas isoladas
            if (_paramGerais._parAvan._verifCargaIsolada)
            {
                //analise cargas isoladas
                IsolatedLoads obj = new IsolatedLoads(_oDSS._DSSObj.ActiveCircuit, _paramGerais);
                obj.PlotaCargasIsoladasArq(_janela);
            }

            // TODO criar interface
            //queryLineCode(_DSSObj.ActiveCircuit);

            // TODO criar interface 
            //queryLine( _oDSS._DSSCircuit);

            //IteraSobreLine(_oDSS._DSSCircuit);

            //IteraSobreLineCode(_oDSS._DSSCircuit);

        }

        private void IteraSobreLine(Circuit dSSCircuit)
        {
            //DEBUG
            //int debug = dSSCircuit.Lines.First;

            do
            {
                string nome = dSSCircuit.Lines.Name;

                string lineCode = dSSCircuit.Lines.LineCode;

                double rho = dSSCircuit.Lines.Rho;
                double Xg = dSSCircuit.Lines.Xg;

                double r0 = dSSCircuit.Lines.R0;
                double r1 = dSSCircuit.Lines.R1;

            } while (dSSCircuit.Lines.Next != 0);
             
        }

        private void IteraSobreLineCode(Circuit dSSCircuit)
        {
            //DEBUG
            //int debug = dSSCircuit.LineCodes.First;

            do
            {
                string nome = dSSCircuit.LineCodes.Name;
                double r0 = dSSCircuit.LineCodes.R0; //0.1784  valores 336
                double x0 = dSSCircuit.LineCodes.X0; //0.4047
                double r1 = dSSCircuit.LineCodes.R1; //0.0580
                double x1 = dSSCircuit.LineCodes.X1; //0.1206

            } while (dSSCircuit.LineCodes.Next != 0);

        }

        // get mensagem convergencia 
        public string GetMsgConvergencia(string hora, string nomeAlim)
        {
            string str;

            if (hora != null)
            {
                str = nomeAlim + " Hour: " + Add1toHour(hora) + " -> Solution Converged";
            }
            else
            {
                str = nomeAlim + " -> Solution Converged";
            }
            return str;
        }

        //adiciona 1hora a string hora
        private static string Add1toHour(string hora)
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
                TxtFile.GravaLstAlimNaoConvergiram(_paramGerais, _janela);
            }
            // Grava Perdas de acordo com o tipo de fluxo
            else
            {
                string nomeAlim = _paramGerais.GetNomeAlimAtual();

                // obtem o nome do arquivo de perdas, conforme o tipo do fluxo 
                string arquivo = _paramGerais.GetNomeArquivoPerdas();

                // Grava Perdas
                TxtFile.GravaPerdas(_resFluxo, nomeAlim, arquivo, _janela);
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
                VoltageLevelAnalysis indTensao = new VoltageLevelAnalysis(DSSCircuit, DSSText);

                // Calcula num Clientes com DRP e DRC 
                indTensao.CalculaNumClientesDRPDRC();

                // grava em arquivo
                indTensao.ImprimeNumClientesDRPDRC(_paramGerais, _janela);

                //
                indTensao.ImprimeBarrasDRPDRC(_paramGerais, _janela);
            }
        }

        // Calcula DRP e DRC
        public List<string> GetBarrasDRPDRC()
        {
            // Interfaces
            Circuit DSSCircuit = _oDSS._DSSObj.ActiveCircuit;
            Text DSSText = _oDSS._DSSObj.Text;

            // se convergiu 
            if (DSSCircuit.Solution.Converged)
            {
                // cria objeto indice tensao
                VoltageLevelAnalysis indTensao = new VoltageLevelAnalysis(DSSCircuit, DSSText);

                // Calcula num Clientes com DRP e DRC 
                indTensao.CalculaNumClientesDRPDRC();
            }

            return VoltageLevelAnalysis._lstBarrasDRCeDRP;
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
            VerifGerUsinasGDMT();

            return ret;
        }

        // verifica se usina do alimentador gerou energia, informando no display da tela.
        private void VerifGerUsinasGDMT()
        {
            // Se existe gerador, obtem gerador
            if (File.Exists(_paramGerais.GetDiretorioAlim() + _paramGerais.GetNomeGeradorMT_mes()))
            {
                // para cada gerador do alimentador
                do
                {
                    string[] nomeGen = _oDSS.GetActiveCircuit().Generators.AllNames;

                    //
                    string[] genRegisterNames = _oDSS.GetActiveCircuit().Generators.RegisterNames;

                    // para cada gerador
                    double[] genRegisterValues = _oDSS.GetActiveCircuit().Generators.RegisterValues;

                    if (genRegisterValues == null)
                    {
                        _janela.ExibeMsgDisplayMW("Usina desconectada!");
                    }

                } while (_oDSS.GetActiveCircuit().Generators.Next != 0);

            }
        }

        // TODO refactory 
        private void QueryLineCode(Circuit dssCircuit)
        {
            List<string> lstCabos = new List<string>
            {
                "CAB102",
                "CAB103",
                "CAB104",
                "CAB107",
                "CAB108",
                "CAB202",
                "CAB203",
                "CAB204",
                "CAB207",
                "CAB208",
                "CABA06",
                "CABA08",
                "CAB2021",
                "CAB1031",
                "CAB1021",
                "CAB2031",
                "CABA061",
                "CABBT106",
                "CABBT107",
                "CABBT108",
                "CABBT803",
                "CABBT805",
                "CABBT809",
                "CABBT810",
                "CABBT801",
                "CABBT807",
                "CABBT808"
            };

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

            TxtFile.GravaListArquivoTXT(resRmatrix, _paramGerais.GetArqRmatrix(), _janela);

            TxtFile.GravaListArquivoTXT(resXmatrix, _paramGerais.GetArqXmatrix(), _janela);
        }

        // TODO refactory 
        private void QueryLine(Circuit dssCircuit)
        {
            List<string> lstLinhas = new List<string>
            {
                "CAB102",
                "CAB103",
                "CAB104",
                "CAB107",
                "CAB108",
                "CAB202",
                "CAB203",
                "CAB204",
                "CAB207",
                "CAB208",
                "CABA06",
                "CABA08",
                "CABBT106",
                "CABBT107",
                "CABBT108",
                "CABBT803",
                "CABBT805",
                "CABBT809",
                "CABBT810",
                "CABBT801",
                "CABBT807",
                "CABBT808"
            };

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

            TxtFile.GravaListArquivoTXT(resRmatrix, _paramGerais.GetArqRmatrix(), _janela);

            TxtFile.GravaListArquivoTXT(resXmatrix, _paramGerais.GetArqXmatrix(), _janela);
        }
    }
}

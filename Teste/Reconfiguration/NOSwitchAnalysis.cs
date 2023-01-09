//#define ENGINE
#if ENGINE
using OpenDSSengine;
#else
using dss_sharp;
#endif

using ExecutorOpenDSS.Classes;
using System;
using System.Collections.Generic;

namespace ExecutorOpenDSS.Classes_Principais
{
    class NOSwitchAnalysis
    {
        public GeneralParameters _paramGerais;

        private DailyFlow _fluxoSoMT;
        private MonthlyPowerFlow _fluxoMensal;

        // armazena resultado do fluxo do Caso Base i.e. alim na posicao original
        private PFResults _resCasoBase;

        // armazena melhor execucao por chave NA
        private PFResults _MelhorResultadoPorNA;

        // numero de cargas isoladas
        private int _numCargasIsoladas;

        private List<Switch> _lstChavesNA;
        private List<Switch> _lstChavesNF;
        private Dictionary<string, Switch> _dicChavesNF;

        // lista de chaves otimizadas
        private List<string> _lstParDeChavesOtm;

        public FeederGraph _grafo;

        public NOSwitchAnalysis(GeneralParameters par, List<string> lstAlimentadores) 
        {           
            // inicializa variaveis de classe
            _paramGerais = par;  

            // analisa chave NA de cada alimentador
            foreach (string nomeAlim in lstAlimentadores)
            {
                AnaliseChavesNAsPvt(nomeAlim);
            }

            // Grava Log
            _paramGerais._mWindow.GravaLog();
        }

        private void AnaliseChavesNAsPvt(string nomeAlim)
        {
            // atribui nomeAlim
            _paramGerais.SetNomeAlimAtual(nomeAlim);

            // Carrega arquivos DSS so MT
            _fluxoSoMT = new DailyFlow(_paramGerais, "DU", true);

            // executa fluxo snap
            bool ret = _fluxoSoMT.ExecutaFluxoSnap();
            
            if (!ret)
            {
                // plota numero de FPs
                _paramGerais._mWindow.ExibeMsgDisplay("Fluxo nao convergiu!");
                return;
            }

            // Open the monophases lines, as it can contain a path to substation
            _fluxoSoMT.RemoveMonophaseLineSegments();
            
            // executa fluxo snap
            ret = _fluxoSoMT.ExecutaFluxoSnapSemRecarga();
            
            // se exibeConvergencia 
            if (ret)
            {
                // verifica cancelamento usuario 
                if (_paramGerais._mWindow._cancelarExecucao)
                {
                    return;
                }

                // get chaves NAs conjunto/alimentador 
                GetChaves();
             
                // cria objeto grafo, juntamente com as matrizes de incidencia
                _grafo = new FeederGraph( _paramGerais, _fluxoSoMT._oDSS );

                // verifica se extremidades das chaves NAs estao no conjunto escolhido
                // OBS: no momento esta filtrando chaves monofasicas tb
                FiltraLstChavesNAs();

                // DEBUG plota chaves 
                // plotaChavesNA();

                // Creates monthly PF obj.
                _fluxoMensal = new MonthlyPowerFlow(_paramGerais);

                // fluxo mensal que servira de referencia para a otimizacao
                bool ret2 = _fluxoMensal.ExecutaFluxoMensalAproximacaoDU();

                if (ret2)
                {
                    // preeenche variavel cargas isoladas
                    _numCargasIsoladas = _fluxoMensal.GetNumCargasIsoladas();

                    // armazena resultado do fluxo em variavel temporaria
                    _resCasoBase = new PFResults(_fluxoMensal._resFluxoMensal);

                    // para cada chave NA, escolhe 1 chave NF
                    BranchExchange();

                    //ajusta numero de FPS, considerando o inicial (so na MT)
                    _fluxoMensal._nFP++;

                    // plota numero de FPs
                    _paramGerais._mWindow.ExibeMsgDisplay("Número de FPs: " + _fluxoMensal._nFP.ToString());

                    // plota pares otimizados
                    PlotaChavesOtimizadas();
                }
            }
         }

        // plota pares de chaves otimizados
        private void PlotaChavesOtimizadas()
        {
            foreach (string parChaves in _lstParDeChavesOtm)
            {
                _paramGerais._mWindow.ExibeMsgDisplay(parChaves);
            }
        }

        // filtra chaves NAs do conjunto de acordo com o seguinte criterio: ambas as extremidades das chaves (vertices) 
        // devem estar presentes no map de vertices do alimentador
        private void FiltraLstChavesNAs()
        {
            List<Switch> novaLstChavesNA = new List<Switch>();

            // para cada chave NA
            foreach (Switch chaveNA in _lstChavesNA)
            {

                bool contem1 = _grafo._mapNomeVertice2Indice.ContainsKey(chaveNA.bus1);
                bool contem2 = _grafo._mapNomeVertice2Indice.ContainsKey(chaveNA.bus2);

                //adiciona na nova lista somente chaves com as duas extremidades no conjunto
                if (contem1 && contem2)
                {
                    novaLstChavesNA.Add(chaveNA);
                }

                /* //OBS: ja esta sendo filtrado automaticamente no loop acima
                // mantem somente chaves trifasica
                if (chaveNA.phases.Equals(3))
                {
                    novaLstChavesNA.Add(chaveNA);
                }*/
            }

            //
            _lstChavesNA = novaLstChavesNA;

            // plota numero chaves NA (filtradas)
            _paramGerais._mWindow.ExibeMsgDisplay("\n N. chaves NA: " + _lstChavesNA.Count.ToString() );
        }

        // faz permutacao de ramos 
        private void BranchExchange()
        {
            // inicializa _lstParDeChavesOtm
            _lstParDeChavesOtm = new List<string>
            {
                "Chave NA atual \tNovaNA \tRedução(kWh/mês) \tTensão antes (kV) \tTensão Depois (kV) \tVariação(%)"
            };

            // para cada chave NA
            foreach (Switch chaveNA in _lstChavesNA)
            {
                // 
                string novaChaveNA = DefineNovaChaveNA(chaveNA);

                // se houve sucesso, avisa usuario
                if (novaChaveNA != null)
                {
                    // analisa melhoria de tensao nas monabora, verificando as chaves NF (antes e depois)
                    string strTensoes = VoltageVerificationIn_NO_Switches(chaveNA, novaChaveNA);

                    string plot = _paramGerais.GetNomeAlimAtual() + "\t" + chaveNA.nome + "\t" + novaChaveNA + "\t" + chaveNA.redPerda_kWh.ToString("0.##") + "\t" + strTensoes;

                    _lstParDeChavesOtm.Add(plot);
                    // DEBUG
                    // Informa manobra ao usuario
                    //_janela.ExibeMsgDisplay("\n Fechar chaveNA: " + chaveNA.nome + " Abrir chaveNF: " + novaChaveNA);
                }
            }
        }

        // verifica os niveis de tensao antes e depois da manobra, as 18horas
        private string VoltageVerificationIn_NO_Switches(Switch chaveNA, string novaChaveNA)
        {
            // 1 fluxo potencia as 18horas
            _fluxoMensal._fluxoDU.ExecutaFluxoHorario_SemRecarga("18");

            // Tensao antes: Chave NF original (novaNA)
            _fluxoMensal.GetObjDSS().GetActiveCircuit().SetActiveElement("Line." + novaChaveNA);
            double[] SeqVoltages = _fluxoMensal.GetObjDSS().GetActiveCircuit().get_CktElements("Line." + novaChaveNA).SeqVoltages;
            double tensaoAntes = SeqVoltages[1];

            /* //DEBUG
            double[] VoltagesMagAng = _fluxoMensal.GetObjDSS()._DSSCircuit.get_CktElements("Line." + novaChaveNA).VoltagesMagAng;
            double[] Voltages = _fluxoMensal.GetObjDSS()._DSSCircuit.get_CktElements("Line." + novaChaveNA).Voltages; //ComplexVoltages
            */

            // faz manobra string nomeChaveNF, Switch chaveNA
            FazManobra(novaChaveNA, chaveNA);

            // 2 fluxo potencia as 18horas
            _fluxoMensal._fluxoDU.ExecutaFluxoHorario_SemRecarga("18");

            // Tensao DEPOIS: Nova NF (NA original)
            _fluxoMensal.GetObjDSS().GetActiveCircuit().SetActiveElement("Line." + chaveNA.nome);
            SeqVoltages = _fluxoMensal.GetObjDSS().GetActiveCircuit().get_CktElements("Line." + chaveNA.nome).SeqVoltages;
            double tensaoDepois = SeqVoltages[1];

            double variacao = (tensaoDepois/tensaoAntes -1)*100;

            // desfaz manobra 
            DesfazManobra(novaChaveNA, chaveNA);

            // Informa manobra ao usuario
            //_janela.ExibeMsgDisplay("\n Tensão antes: " + tensaoAntes.ToString("0.##") + " kV Tensão depois: " + tensaoDepois.ToString("0.##") + "kV \t");

            return ("\t" + tensaoAntes.ToString("0.##") + "\t" + tensaoDepois.ToString("0.##") + "\t" + variacao.ToString("0.##") );
        }

        // define nova chave NA, entre as chaves NFs 
        private string DefineNovaChaveNA(Switch chaveNAinicial)
        {
            // copia o resultado do caso para para a analisa da chave NA.  
            _MelhorResultadoPorNA = new PFResults(_resCasoBase);

            // obtem lstChaves no ciclo da chaveNA analisada
            bool ret = _grafo.MenorCaminho(chaveNAinicial);

            // nova chaveNA
            string nomeNovaChaveNA = null;

            // se obteve menor caminho ate a fonte
            if (ret)
            {
                // analisa caminho1 
                nomeNovaChaveNA = AnalisaChavesNF(_grafo._lstNomeChavesNFcam1, chaveNAinicial);

                // se nao otimizou, analisa chaves NF reverso
                if (nomeNovaChaveNA == null)
                {
                    // inverte caminho do grafo
                    _grafo._lstNomeChavesNFcam1.Reverse();

                    nomeNovaChaveNA = AnalisaChavesNF(_grafo._lstNomeChavesNFcam1, chaveNAinicial);
                }
            }

            return nomeNovaChaveNA;
        }

        // analisa lista de "Nome De Chaves NF" (obtida pelo grafo) 
        private string AnalisaChavesNF(List<string> lstNomeChavesNFcam1, Switch chaveNA)
        {
            bool ret;
                        
            // no inicio, nova ChaveNA eh null
            string nomeNovaChaveNA = null;

            // para cada chaveNF verifica se obtem configuracao melhor 
            foreach (string nomeChaveNF in lstNomeChavesNFcam1)
            {
                // verifica cancelamento usuario 
                if (_paramGerais._mWindow._cancelarExecucao)
                {
                    return null;
                }

                /* // DEBUG 
                if (nomeChaveNF.Equals("ctr853573"))
                {
                    int debug = 0;
                }*/

                // analisa 1 chave NF por vez
                ret = AnalisaChavesNFPvt(nomeChaveNF, chaveNA);

                if (ret)
                {
                    nomeNovaChaveNA = nomeChaveNF;
                }
                else // se retorno igual a false (chave NF aumentou as perdas) interrompe a analise
                { 
                    break;
                }
            }

            return nomeNovaChaveNA;
        }

        // analisa chave NF, buscando no
        private bool AnalisaChavesNFPvt(string nomeChaveNF, Switch chaveNA)
        {
            bool ret = false;

            /*
            if (nomeChaveNF.Equals("ctr487866"))
            {
                int debug = 0;
            }*/
            
            // verifica se chave NF eh trifasica
            if (GetNumFases(nomeChaveNF) != 3) 
            {
                _paramGerais._mWindow.ExibeMsgDisplay("Tentativa de reconfigurar chave 1# " + nomeChaveNF);
                return ret;
            }

            // Faz Manobra
            FazManobra(nomeChaveNF, chaveNA);

            // executa fluxo 
            ret = AvaliaFluxoPot(chaveNA);

            // desfaz a manobra para continuar analisando outras chaves NF
            DesfazManobra(nomeChaveNF, chaveNA);

            /*
            // DEBUG verificacao se apos desfazer a manobra temos os mesmos resultados anteriores
            // resolve circuito 
            _fluxoMensal.ExecutaFluxoMensalSimples();
            double verifPerdas = _fluxoMensal._resFluxoMensal.getPerdasEnergia();
            */
            return ret;
        }

        // FAZ Manobra
        private void FazManobra(string nomeChaveNF, Switch chaveNA)
        {
            // fecha chave NA
            FechaChavePorObjSwitch(chaveNA);

            // abre chave NF 
            AbreChavePorString(nomeChaveNF);
        }

        // DESFAZ Manobra
        private void DesfazManobra(string nomeChaveNA, Switch chaveNF)
        {
            // FECHA chave NA  
            FechaChavePorString(nomeChaveNA);

            // ABRE chave NF
            AbreChaveParObjSwitch(chaveNF);
        }

        // Abre chave NA  
        private void FechaChavePorObjSwitch(Switch chaveNA)
        {
            chaveNA.Fecha( _fluxoMensal.GetObjDSS()._DSSText, chaveNA.nome);

            // DEBUG verifica status
            // bool debug = chaveNA.GetStatusAberto(_fluxoMensal.getObjDSS()._DSSCircuit);
        }

        // Fecha chave NF
        private void FechaChavePorString(string nomeChaveNF)
        {
            Switch chaveTmp = _dicChavesNF[nomeChaveNF];

            // TODO Nome composto
            nomeChaveNF = "line." + nomeChaveNF;

            chaveTmp.FechaPorNomeComp(_fluxoMensal.GetObjDSS()._DSSText, nomeChaveNF);

            // DEBUG verifica status
            //bool debug = chaveTmp.GetStatusAberto( _fluxoMensal.getObjDSS()._DSSCircuit );
        }

        // ABRE chave (parametro chaveNF)
        private void AbreChavePorString(string nomeChaveNF)
        {
            Switch chaveTmp = _dicChavesNF[nomeChaveNF];

            // TODO nome composto
            nomeChaveNF = "line." + nomeChaveNF;

            // Abre Chave
            chaveTmp.Abre(_fluxoMensal.GetObjDSS()._DSSText, nomeChaveNF);

            // DEBUG verifica status
            //bool debug = chaveTmp.GetStatusAberto(_fluxoMensal.getObjDSS()._DSSCircuit);
        }

        // Fecha chave NF
        private void AbreChaveParObjSwitch(Switch chaveNA)
        {            
            chaveNA.Abre(_fluxoMensal.GetObjDSS()._DSSText, chaveNA.GetNomeCompostoChave() );

            // DEBUG verifica status 
            //bool debug = chaveNA.GetStatusAberto(_fluxoMensal.getObjDSS()._DSSCircuit);
        }

        // OLD CODE substituido por metodo OpenDSS C#
        // obtem status da chave 
        private bool GetStatusChave(Switch chaveNA)
        {
            // documentacao
            // https://sourceforge.net/p/electricdss/discussion/861976/thread/6c0c5113/

            //%Here the sw1 is set as active element
            _fluxoMensal.GetObjDSS().GetActiveCircuit().SetActiveElement("Line." + chaveNA.nome);
            
            //%Here we ask if the sw1 is open in the term 2
            bool statusAbertoFech = _fluxoMensal.GetObjDSS().GetActiveCircuit().get_CktElements("Line." + chaveNA.nome).IsOpen(1, 0);
            
            return statusAbertoFech;
        }

        // ABRE chave (parametro chaveNF)
        private int GetNumFases(string nomeChaveNF)
        {
            Switch chaveTmp = _dicChavesNF[nomeChaveNF];

            return chaveTmp.phases;
        }

        // avalia fluxo de potencia para cada chavesNF da lista.
        private bool AvaliaFluxoPot(Switch chaveNA)
        {
            // resolve circuito 
            _fluxoMensal.ExecutaFluxoMensalAproximacaoDU_SemRecarga();

            double novaPerdas_kWh = double.PositiveInfinity;
            double novaEnergForn_kWh;            
            double relacaoEnergForn = 1;

            // se convergiu, obtem novos valores de perdas e energia fornecida
            if (_fluxoMensal._resFluxoMensal._convergiuBool)
            {
                novaPerdas_kWh = _fluxoMensal._resFluxoMensal.GetPerdasEnergia();
                novaEnergForn_kWh = _fluxoMensal._resFluxoMensal.GetEnergia();
                
                // variacao da energia fornecida nao pode ser maior que 5%
                relacaoEnergForn = Math.Abs( (novaEnergForn_kWh - _resCasoBase.GetEnergia())/ _resCasoBase.GetEnergia() );
            }            

            // verifica se isolou cargas
            if (_fluxoMensal.GetNumCargasIsoladas() != _numCargasIsoladas)
            {
                _paramGerais._mWindow.ExibeMsgDisplay("Problema Isolamento de cargas!");
            }

            // se alcancou valor menor de perdas & variacao da energia fornecida nao pode ser maior que 5%
            if (novaPerdas_kWh < _MelhorResultadoPorNA.GetPerdasEnergia() && relacaoEnergForn < 0.05 )
            {
                chaveNA.chaveOtimizada = true;

                //  atualiza _MelhorResultadoPorNA
                _MelhorResultadoPorNA = new PFResults(_fluxoMensal._resFluxoMensal);

                // armazena nova perdas na chaveNA para referencia futura
                chaveNA.perda_kWh = novaPerdas_kWh;
                chaveNA.energiaForn_kWh = _fluxoMensal._resFluxoMensal.GetEnergia();
                chaveNA.perdaPercentual = 100 * chaveNA.perda_kWh / chaveNA.energiaForn_kWh;
                chaveNA.redPerda_kWh = _resCasoBase.GetPerdasEnergia() - chaveNA.perda_kWh;

                return true;
            }
            return false;
        }

        // get chaves NAs conjunto/alimentador 
        private void GetChaves()
        {
            Circuit alim = _fluxoSoMT._oDSS.GetActiveCircuit();

            int iterLinha = alim.Lines.First;

            _lstChavesNF = new List<Switch>();
            _dicChavesNF = new Dictionary<string,Switch>();
            _lstChavesNA = new List<Switch>();

            while (iterLinha != 0)
            {
                // verifica se eh Chave
                if ( alim.Lines.IsSwitch) //#if ENGINE
                //if ( true ) //#if ENGINE
                {
                    // criar objeto chave, armazenando nao so o nome, mas tambem os nodos
                    Switch ch = new Switch(alim);

                    // verifica se chave NA
                    if (ch._estaAberta)
                    {
                        // so adiciona chave NA se for trifasica
                        if (ch.phases.Equals(3))
                        {
                            _lstChavesNA.Add(ch);
                        }
                    }
                    else
                    {
                        // so adiciona chave NF se for trifasica
                        if (ch.phases.Equals(3))
                        { 
                            _lstChavesNF.Add(ch);
                            _dicChavesNF.Add(ch.nome, ch);
                        }
                    }
                } 
                iterLinha = alim.Lines.Next;
            }
        }

        // OLD CODE
        //Plota niveis tensao nas barras dos trafos
        public void PlotaChavesNA()
        {
            //
            string linha = "Chaves NAs: ";

            // para cada key value
            foreach (Switch chave in _lstChavesNA)
            {
                // evita plotagem do null em _lstCargasIsoladas
                if (chave == null)
                    break;

                // TODO tratar retirada \n ultima linha 
                linha += chave.nome + "\t";
            }

            _paramGerais._mWindow.ExibeMsgDisplay(linha);
        }       
    }
}

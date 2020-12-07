#define ENGINE
#if ENGINE
using OpenDSSengine;
#else
using dss_sharp;
#endif

using System;
using System.Collections.Generic;
using System.IO;
using ExecutorOpenDSS.Classes;
using ExecutorOpenDSS.Classes_Principais;

namespace ExecutorOpenDSS
{
    class RunPowerFlow
    {
        public static MainWindow _janela;
        public GeneralParameters _paramGerais;

        public ObjDSS _oDSS;
        public DailyFlow _fluxoDiario;
        public MonthlyPowerFlow _fluxoMensal;

        public RunPowerFlow(MainWindow janela, GeneralParameters par)
        {          
            // variaveis de classe
            _janela = janela;
            _paramGerais = par;

            // Instancia ObjDSS
            _oDSS = new ObjDSS(_paramGerais);

            //Executa o Fluxo
            _janela.ExibeMsgDisplayMW("Executando Fluxo...");

            //Verifica se foi solicitado o cancelamento.
            if (_janela._cancelarExecucao)
            {
                _janela.FinalizaProcesso(true);
                return;
            }
        }

        // executa modo Snap
        public void ExecutaSnap(List<string> lstAlimentadoresCemig)
        {
            //Limpa Arquivos
            _paramGerais.DeletaArqResultados();

            //Roda Fluxo para cada alimentador
            foreach (string nomeAlim in lstAlimentadoresCemig)
            {
                // Verifica se foi solicitado o cancelamento.
                if (_janela._cancelarExecucao)
                {
                    break;
                }
                
                // atribui novo alimentador
                _paramGerais.SetNomeAlimAtual(nomeAlim);

                //cria objeto fluxo diario
                _fluxoDiario = new DailyFlow(_paramGerais, _janela, _oDSS);

                // carrega alimentador
                _fluxoDiario.CarregaAlimentador();

                //Execução com otimização demanda energia
                if (_paramGerais._parGUI._otmPorDemMax)
                {
                    Otimiza();
                }
                //Execução padrão
                else
                {
                    Snap(nomeAlim);
                }
            }

            // TODO testar
            //Se modo otimiza grava arquivo load mult
            if (_paramGerais._parGUI._otmPorDemMax)
            {
                _paramGerais.GravaMapAlimLoadMultExcel();
            }
        }

        // executa modo Mensal
        public void ExecutaMensal(List<string> lstAlimentadoresCemig)
        {
            //Limpa Arquivos
            _paramGerais.DeletaArqResultados();

            //Roda fluxo para cada alimentador
            foreach (string alim in lstAlimentadoresCemig)
            {
                if (_janela._cancelarExecucao)
                {
                    return;
                }

                // atribui nomeAlim
                _paramGerais.SetNomeAlimAtual(alim);

                //cria objeto fluxo diario
                _fluxoMensal = new MonthlyPowerFlow(_paramGerais, _janela, _oDSS);

                // se carregou alimentador
                if (_fluxoMensal.CarregaAlimentador())
                {
                    //Execução com otimização
                    if (_paramGerais._parGUI._otmPorEnergia)
                    {
                        // Otimiza por energia
                        Otimiza();

                        continue;
                    }
                    // Otimiza PU saida
                    if (_paramGerais._parAvan._otimizaPUSaidaSE)
                    {
                        // Calcula nivel otimo PU
                        OtimizaPUSaidaSE();

                        continue;
                    }
                    //Execução padrão
                    Mensal();
                }
            }

            //Se modo otimiza grava arquivo load mult
            if (_paramGerais._parGUI._otmPorEnergia)
            {
                _paramGerais.GravaMapAlimLoadMultExcel();
            }
        }

        //Tradução da função executaFluxoSnapOpenDSS
        internal void Snap(string nomeAlim)
        {
            //Verifica se foi solicitado o cancelamento.
            if (_janela._cancelarExecucao)
            {
                return;
            }

            // Entra no diretório ArquivosDss e no subdiretorio 'nomeAlm'
            string nomeArqDSSCompleto = _paramGerais.GetNomeArquivoAlimentadorDSS();

            // Verifica existencia xml
            if (File.Exists(nomeArqDSSCompleto))
            {                
                _fluxoDiario.ExecutaFluxoSnap();
            }
            else
            {
                // get nome Alim
                string alimTmp = _paramGerais.GetNomeAlimAtual();

                //Exibe mensagem de erro, caso não encontrem os arquivos de resultados
                _janela.ExibeMsgDisplayMW(alimTmp + ": Arquivos *.dss não encontrados");
            }
        }

        //Tradução da função otimizaLoadMult
        internal void Otimiza()
        {
            // Verifica se foi solicitado o cancelamento.
            if (_janela._cancelarExecucao)
            {
                return;
            }

            // otimiza PVT 
            double loadMult = OtimizaPvt();

            // Atualiza map loadMultAlim
            _janela._medAlim.AtualizaMapAlimLoadMult(loadMult);
        }

        // Otimiza Pvt
        internal double OtimizaPvt()
        {
            //set Nome Alim atual 
            string alimTmp = _paramGerais.GetNomeAlimAtual();

            // loadMult inicial 
            double loadMultInicial = 0;

            loadMultInicial = _janela._medAlim._reqLoadMultMes.GetLoadMult();

            if ( loadMultInicial.Equals(double.NaN) )
            {
                _janela.ExibeMsgDisplayMW(alimTmp + ": Alimentador não encontrado no arquivo de ajuste");

                return loadMultInicial;
            }

            // OBS: condicao de retorno: Skipa alimentadores com loadMult igual a zero se loadMult == 0 
            if (loadMultInicial == 0)
            {
                _janela.ExibeMsgDisplayMW(alimTmp + ": Alimentador não otimizado! LoadMult=0");

                return loadMultInicial;  
            }

            // OBS: condicao de retorno: Skipa alimentadores com loadMult > 10 
            if ((loadMultInicial > 10) || (loadMultInicial < 0.1))
            {
                _janela.ExibeMsgDisplayMW(alimTmp + ": Alimentador não otimizado! LoadMult = " + loadMultInicial);
                
                return _paramGerais._parGUI._loadMultAlternativo;
            }

            double loadMult;

            // modo otimiza LoadMult energia 
            if (_paramGerais._parGUI._otmPorEnergia)
            {
                loadMult = OtimizaLoadMultEnergiaPvt(alimTmp, loadMultInicial);
            }
            //modo otimiza loadMult potencia
            else
            {
                loadMult = OtimizaLoadMultPvt(loadMultInicial);
            }
            return loadMult;   
        }

        //Tradução da função calculaFluxoDiario
        public void ExecutaDiario(List<string> lstAlimentadoresCemig)
        {
            //Limpa arquivos
            _paramGerais.DeletaArqResultados();

            // cria arquivo e preenche Cabecalho, caso modo _calcDRPDRC
            if (_paramGerais._parAvan._calcDRPDRC)
            {
                // Cria arquivo cabecalho
                VoltageLevelAnalysis.CriaArqCabecalho(_paramGerais, _janela);
            } 

            //Roda o fluxo para cada alimentador
            foreach (string nomeAlim in lstAlimentadoresCemig)
            {
                //Verifica se foi solicitado o cancelamento.
                if (_janela._cancelarExecucao)
                {
                    return;
                }

                // atribui nomeAlim
                _paramGerais.SetNomeAlimAtual(nomeAlim);

                //cria objeto fluxo diario
                _fluxoDiario = new DailyFlow(_paramGerais, _janela, _oDSS);             

                // se carregou alimentador
                if (_fluxoDiario.CarregaAlimentador())
                {
                    //Executa fluxo diário openDSS
                    ExecutaFluxoDiarioOpenDSS();
                }
            }
        }

        //Tradução da função calculaFluxoMensal
        internal void Mensal()
        {
            // Executa Fluxo Mensal 
            bool ret = _fluxoMensal.ExecutaFluxoMensal();

            // se nao convergiu 
            if (!ret)
            {
                _janela.ExibeMsgDisplayMW(_paramGerais.GetNomeAlimAtual() + " alimentador não convergiu!");
            }
        }

        // Fluxo anual
        public void ExecutaAnual(List<string> lstAlimentadoresCemig)
        {
            //Limpa Arquivos
            _paramGerais.DeletaArqResultados();

            //Seta modo anual
            _paramGerais._parGUI._modoAnual = true;

            //Roda fluxo para cada alimentador
            foreach (string alim in lstAlimentadoresCemig)
            {
                // atribui alim 
                _paramGerais.SetNomeAlimAtual(alim);

                //Vetor _resultadoFluxo
                List<PFResults> lstResultadoFluxo = new List<PFResults>();

                // para cada mes executa um fluxo mensal
                for (int mes = 1; mes < 13; mes++)
                {
                    // Verifica se foi solicitado o cancelamento.
                    if (_janela._cancelarExecucao)
                    {
                        return;
                    }          
                    
                    // set mes
                    _paramGerais._parGUI.SetMes(mes);

                    //cria objeto fluxo diario
                    _fluxoMensal = new MonthlyPowerFlow(_paramGerais, _janela, _oDSS);

                    // se nao carregou alimentador retorna
                    if (_fluxoMensal.CarregaAlimentador())
                    {
                        // Otimiza
                        if (_paramGerais._parGUI._otmPorEnergia)
                        {
                            Otimiza();
                        }
                        else
                        {
                            // Executa Fluxo Mensal 
                            _fluxoMensal.ExecutaFluxoMensal();
                        }
                        PFResults resTmp = new PFResults(_fluxoMensal._resFluxoMensal);

                        //Armazena Resultado
                        lstResultadoFluxo.Add(resTmp);
                    }
                    // FIX ME
                    // se nao carregou alimentador, forca mes = 13 para terminar o for 
                    else 
                    {
                        mes = 13;
                        break;
                    }
                }

                // se convergiu, Calcula resultado Ano 
                if (_fluxoMensal._resFluxoMensal._convergiuBool )
                {
                    //Calcula resultado Ano
                    _fluxoMensal._resFluxoMensal.CalculaResAno(lstResultadoFluxo, alim, _paramGerais.GetNomeComp_arquivoResPerdasAnual(), _janela);
                }
            }
        }

        //Tradução da função calculaFluxoMensal
        internal void OtimizaPUSaidaSE()
        {
            // 
            double puSaidaSE = 0.96;

            while (puSaidaSE < 1.05)
            {
                //Verifica se foi solicitado o cancelamento.
                if (_janela._cancelarExecucao)
                {
                    return;
                }

                // atribui pu Saida SE
                _paramGerais.SetTensaoSaidaSE(puSaidaSE.ToString());

                // Executa Fluxo Mensal 
                _fluxoMensal.ExecutaFluxoMensal();

                // Atualiza PU
                puSaidaSE += 0.02;
            }            
        }

        // Tradução da função otimizaLoadMultPvt
        internal double OtimizaLoadMultPvt(double loadMult)
        {
            // alimTmp
            string alimTmp = _paramGerais.GetNomeAlimAtual();

            //referência de geração (semana típica ou medição do MECE)
            double refGeracao;
            if (_janela._medAlim._mapDadosDemanda.ContainsKey(alimTmp))
            {
                refGeracao = _janela._medAlim._mapDadosDemanda[alimTmp];
            }
            else
            {
                _janela.ExibeMsgDisplayMW(alimTmp + ": mapDadosDemanda não encontrado");
                return double.NaN;
            }

            //calcula geração inicial
            double geracao = ExecutaFluxoSnapOtm(loadMult);

            // contador fluxo de potência
            // OBS: considera a execução acima
            int contFP = 1;

            // Se geração inicial for maior que referência OU fluxo não convergiu
            // retorna função
            if (double.IsNaN(geracao) || geracao > refGeracao)
            {
                return _paramGerais._parGUI._loadMultAlternativo;
            }

            // Inicializa loadMult anterior com LoadMult
            double loadMultAnt = loadMult;

            // TODO testar
            while ( Math.Abs(geracao - refGeracao) > _paramGerais._parGUI.GetPrecisao() )
            {
                //Verifica se foi solicitado o cancelamento.
                if (_janela._cancelarExecucao)
                {
                    return loadMult;
                }

                // atualiza loadMult
                loadMultAnt = loadMult;
                loadMult = _paramGerais._parGUI.GetIncremento() * loadMult;

                // calcula geração
                geracao = ExecutaFluxoSnapOtm(loadMult);

                // incrementa contador fluxo
                contFP = contFP + 1;

                // se geracao nao eh vazia
                if (double.IsNaN(geracao))
                {
                    break;
                }
                // condicao saida while: se executou 50 FP
                if ((contFP == 10) || (geracao > refGeracao))
                {
                    break;
                }
            }
            // retorna loadmult anterior
            return loadMultAnt;
        }

        // Tradução da função otimizaLoadMultEnergiaPvt
        internal double OtimizaLoadMultEnergiaPvt(string alimentador, double loadMult)
        {
            // obtem referência de EnergiaMes (medição do MECE)
            double refEnergiaMes = _janela._medAlim._reqEnergiaMes.GetRefEnergia(alimentador, _paramGerais._parGUI.GetMes());

            // verifica ausencia de medicao de energia
            if (refEnergiaMes.Equals(0)||refEnergiaMes.Equals(double.NaN))
            {
                _janela.ExibeMsgDisplayMW(alimentador + ": refEnergiaMes igual a 0 ou NaN!");

                return 0;
            }

            //calcula energia inicial
            if ( !ExecutaFluxoMensalOtm(loadMult) )
            {
                _janela.ExibeMsgDisplayMW(alimentador + " alimentador não convergiu!");

                return _paramGerais._parGUI._loadMultAlternativo;
            }

            // condicao de saida da funcao
            // energia simulada (energiaMes) esta proxima da energia medida (refEnergiaMes)
            if (VerificaEnergiaSimulada(refEnergiaMes))
            {
                _janela.ExibeMsgDisplayMW(alimentador + " ajustado com precisão " + _paramGerais._parGUI.GetPrecisao().ToString() + "KWh.");

                return loadMult;
            }

            // Se fluxo não convergiu retorna loadMult alternativo
            if ( !_fluxoMensal._resFluxoMensal._convergiuBool )
            {
                _janela.ExibeMsgDisplayMW(alimentador + ": erro valor energia!");

                return _paramGerais._parGUI._loadMultAlternativo;
            }

            // 
            double energiaMesTmp = _fluxoMensal._resFluxoMensal.GetEnergia();

            // sentido de busca 
            int sentidoBusca = 1;

            // Se geração inicial for MAIOR que referência
            if (energiaMesTmp > refEnergiaMes)
            {
                // sentido  // retroceder o loadmult
                sentidoBusca = -1;
            }

            // ajusta modelo de carga se opcao _paramGerais._parAvan._modeloCargaCemig = true
            _fluxoMensal.AjustaModeloDeCargaCemig(sentidoBusca);

            // calcula novo LoadMult
            loadMult = AjustaLoadMult(energiaMesTmp, refEnergiaMes, loadMult, sentidoBusca);

            return loadMult;
        }

        // condicao de saida da funcao
        // energia simulada (energiaMes) esta proxima da energia medida (refEnergiaMes)
        internal bool VerificaEnergiaSimulada(double refEnergiaMes)
        {
            // se aproximacao de energia estiver dentro do limite de precisao ja calculado, ignora nova otimizacao
            if (GetDiferencaEnergia(refEnergiaMes) < _paramGerais._parGUI.GetPrecisao())
            {
                return true;
            }
            return false;
        }

        //obtem diferenca de energia de acordo com o modo
        internal double GetDiferencaEnergia(double refEnergiaMes)
        {
            double delta;

            delta = Math.Abs((_fluxoMensal._resFluxoMensal.GetEnergia() - refEnergiaMes));

            return delta;
        }

        // ajusta loadMult incrementalmente
        internal double AjustaLoadMult(double energiaMes, double refEnergiaMes, double loadMult, int sentidoBusca = 1)
        {
            // contador fluxo de potência
            // OBS: considera a execução acima
            int contFP = 1;

            // variavel que armazena o loadMult anterior
            double loadMultAnt = loadMult;
            int sentidoBuscaAnt = sentidoBusca;

            // alimTmp
            string alimTmp = _paramGerais.GetNomeAlimAtual();

            // enquanto erro da energia for maior que precisao
            while (GetDiferencaEnergia(refEnergiaMes) > _paramGerais._parGUI.GetPrecisao())
            {
                //Verifica se foi solicitado o cancelamento.
                if (_janela._cancelarExecucao)
                {
                    return loadMultAnt;
                }

                // detecta se mudou sendido de busca, interrompendo o processo e retornando loadMult apropriado. 
                if (sentidoBuscaAnt != sentidoBusca)
                {
                    _janela.ExibeMsgDisplayMW(alimTmp + " mudança sentido de busca. Diminua o Incremento/Precisão!");

                    // se sentido de busca era decrescente, admite-se retornar loadMult 
                    if (sentidoBuscaAnt == -1)
                        return loadMult;
                    else
                        return loadMultAnt;
                }

                // condicao saida while: se executou 10 FP
                if (contFP == 10)
                {
                    _janela.ExibeMsgDisplayMW(alimTmp + " não ajustado em " + contFP.ToString() + " execuções de fluxo. Repita o processo!");

                    return loadMultAnt;
                }

                // 
                // atualiza loadMult, de acordo com o sentido de ajuste
                loadMultAnt = loadMult;
                if (sentidoBusca == 1)
                {
                    loadMult = loadMult * _paramGerais._parGUI.GetIncremento();
                }
                // retrocede ajuste 
                else
                {
                    loadMult = loadMult / _paramGerais._parGUI.GetIncremento();
                }

                // calcula energia mes. Se modo fluxo mensal Aproximado
                if (!ExecutaFluxoMensalOtm(loadMult))
                {
                    _janela.ExibeMsgDisplayMW(alimTmp + " não convergiu! Retornando loadMult anterior.");

                    return loadMultAnt;
                }

                // incrementa contador fluxo
                contFP = contFP + 1;

                //atualiza sentido de busca
                if ((_fluxoMensal._resFluxoMensal.GetEnergia() > refEnergiaMes) && (sentidoBusca == 1) ||
                     (_fluxoMensal._resFluxoMensal.GetEnergia() < refEnergiaMes) && (sentidoBusca == -1))
                {
                    // guarda sentido busca anterior
                    sentidoBuscaAnt = sentidoBusca;

                    // inverte sentido 
                    sentidoBusca = -sentidoBusca;
                }
            }
            //saida 
            _janela.ExibeMsgDisplayMW(alimTmp + " ajustado nos limites escolhidos.");

            return loadMult;
        }

        // ExecutaFluxoMensalOtm
        internal bool ExecutaFluxoMensalOtm(double loadMult)
        {
            // adiciona loadMult ao paramento gerais           
            _paramGerais._parGUI.loadMultAtual = loadMult;

            bool ret = false;

            // fluxo mensal aproximado
            if (_paramGerais._parGUI.GetAproximaFluxoMensalPorDU())
            {
                // Executa fluxo diario
                ret = _fluxoMensal.ExecutaFluxoMensalSimples();
            }
            else
            {
                // Executa fluxo mensal
                ret = _fluxoMensal.ExecutaFluxoMensal();
            }
            return ret;
        }

        //Tradução da função executaFluxoSnapOpenDSSLoadMult
        internal double ExecutaFluxoSnapOtm(double loadMult)
        {
            // altera o loadMult
            _paramGerais._parGUI.loadMultAtual = loadMult;

            // Chama fluxo snap
            Snap(_paramGerais.GetNomeAlimAtual());

            //alimTmp
            string alimTmp = _paramGerais.GetNomeAlimAtual();

            //informa usuario convergencia
            _janela.ExibeMsgDisplayMW(_fluxoDiario.GetMsgConvergencia(null, alimTmp));

            // retorno
            return _fluxoDiario._resFluxo.GetMaxKW();
        }

        // ExecutaFluxoDiarioOpenDSS
        internal void ExecutaFluxoDiarioOpenDSS()
        {
            // Executa fluxo diario
            _fluxoDiario.ExecutaFluxoDiario();

            //nivel pu
            string nivelTensaoPU = _oDSS.GetActiveCircuit().Vsources.pu.ToString("0.###");

            //Plota perdas na tela
            _janela.ExibeMsgDisplayMW(_fluxoDiario._resFluxo.GetResultadoFluxoToConsole(
                nivelTensaoPU, _paramGerais.GetNomeAlimAtual()));
        }
    }
}

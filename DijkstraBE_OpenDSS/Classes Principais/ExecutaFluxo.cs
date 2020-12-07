//#define ENGINE
#if ENGINE
using OpenDSSengine;
#else
using dss_sharp;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using OfficeOpenXml;
using ExecutorOpenDSS.Classes;
using ExecutorOpenDSS.Classes_Principais;
using System.Runtime.InteropServices;
using ExecutorOpenDSS.Classes_Auxiliares;

namespace ExecutorOpenDSS
{
    class ExecutaFluxo
    {
        public static MainWindow _janela;
        public ParamGeraisDSS _paramGerais;

        public ObjDSS _oDSS;
        public FluxoDiario _fluxoDiario;
        public FluxoMensal _fluxoMensal;

        public ExecutaFluxo(MainWindow janela, ParamGeraisDSS par)
        {          
            // variaveis de classe
            _janela = janela;
            _paramGerais = par;

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
        public void executaSnap(List<string> lstAlimentadoresCemig)
        {
            //Limpa Arquivos
            _paramGerais.deletaArqResultados();

            //Roda Fluxo para cada alimentador
            foreach (string nomeAlim in lstAlimentadoresCemig)
            {
                // Verifica se foi solicitado o cancelamento.
                if (_janela._cancelarExecucao)
                {
                    break;
                }
                
                // atribui novo alimentador
                _paramGerais.setNomeAlimAtual(nomeAlim);

                // cria servidor DSS
                _oDSS = new ObjDSS(_paramGerais);

                //cria objeto fluxo diario
                _fluxoDiario = new FluxoDiario(_paramGerais, _janela, _oDSS);

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
        public void executaMensal(List<string> lstAlimentadoresCemig)
        {
            //Limpa Arquivos
            _paramGerais.deletaArqResultados();

            //Roda fluxo para cada alimentador
            foreach (string alim in lstAlimentadoresCemig)
            {
                if (_janela._cancelarExecucao)
                {
                    return;
                }

                // atribui nomeAlim
                _paramGerais.setNomeAlimAtual(alim);

                // cria servidor openDSS
                _oDSS = new ObjDSS(_paramGerais);

                //cria objeto fluxo diario
                _fluxoMensal = new FluxoMensal(_paramGerais, _janela, _oDSS);

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

            // get nome Alim
            string alimTmp = _paramGerais.getNomeAlimAtual();

            // Entra no diretório ArquivosDss e no subdiretorio 'nomeAlm'
            string nomeArqDSSCompleto = _paramGerais.getNomeArquivoAlimentadorDSS();

            // Verifica existencia xml
            if (File.Exists(nomeArqDSSCompleto))
            {
                
                _fluxoDiario.ExecutaFluxoSnap();

            }
            else
            {
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
            _paramGerais._reqLoadMultMes.AtualizaMapAlimLoadMult(_paramGerais.getNomeAlimAtual(), loadMult, _paramGerais._parGUI.getMes());
        }

        // Otimiza Pvt
        internal double OtimizaPvt()
        {
            //set Nome Alim atual 
            string alimTmp = _paramGerais.getNomeAlimAtual();

            // loadMult inicial 
            double loadMultInicial = 0;

            // mes
            int mes = _paramGerais._parGUI.getMes();

            // loadMultInicial
            if (_paramGerais._reqLoadMultMes._mapAlimLoadMult[mes].ContainsKey(alimTmp))
            {
                loadMultInicial = _paramGerais._reqLoadMultMes._mapAlimLoadMult[mes][alimTmp];
            }
            else
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
        public void executaDiario(List<string> lstAlimentadoresCemig)
        {
            //Limpa arquivos
            _paramGerais.deletaArqResultados();

            // cria arquivo e preenche Cabecalho, caso modo _calcDRPDRC
            if (_paramGerais._parAvan._calcDRPDRC)
            {
                // Cria arquivo cabecalho
                AnaliseIndiceTensao.CriaArqCabecalho(_paramGerais, _janela);
            } 

            //Roda o fluxo para cada alimentador
            foreach (string nomeAlim in lstAlimentadoresCemig)
            {
                // atribui nomeAlim
                _paramGerais.setNomeAlimAtual(nomeAlim);

                // cria servidor DSS
                _oDSS = new ObjDSS(_paramGerais);

                //cria objeto fluxo diario
                _fluxoDiario = new FluxoDiario(_paramGerais, _janela, _oDSS);
                
                // carrega alimentador
                _fluxoDiario.CarregaAlimentador();

                //Verifica se foi solicitado o cancelamento.
                if (_janela._cancelarExecucao)
                {
                    return;
                }

                //Executa fluxo diário openDSS
                ExecutaFluxoDiarioOpenDSS();
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
                _janela.ExibeMsgDisplayMW(_paramGerais.getNomeAlimAtual() + " alimentador não convergiu!");
            }
        }

        // Fluxo anual
        public void executaAnual(List<string> lstAlimentadoresCemig)
        {
            //Limpa Arquivos
            _paramGerais.deletaArqResultados();

            //Seta modo anual
            _paramGerais._parGUI._modoAnual = true;

            //Roda fluxo para cada alimentador
            foreach (string alim in lstAlimentadoresCemig)
            {
                // atribui alim 
                _paramGerais.setNomeAlimAtual(alim);

                // cria servidor DSS
                _oDSS = new ObjDSS(_paramGerais);

                //Vetor _resultadoFluxo
                List<ResultadoFluxo> lstResultadoFluxo = new List<ResultadoFluxo>();

                // para cada mes executa um fluxo mensal
                for (int mes = 1; mes < 13; mes++)
                {
                    // Verifica se foi solicitado o cancelamento.
                    if (_janela._cancelarExecucao)
                    {
                        return;
                    }          
                    
                    // set mes
                    _paramGerais._parGUI.setMes(mes);

                    //cria objeto fluxo diario
                    _fluxoMensal = new FluxoMensal(_paramGerais, _janela, _oDSS);

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
                        ResultadoFluxo resTmp = new ResultadoFluxo(_fluxoMensal._resFluxoMensal);

                        //Armazena Resultado
                        lstResultadoFluxo.Add(resTmp);
                    }
                    // TOD0 fix me
                    // se nao carregou alimentador, forca mes = 13 para terminar o for 
                    else 
                    {
                        mes = 13;
                    }
                }

                // se convergiu, Calcula resultado Ano 
                if (_fluxoMensal._resFluxoMensal._convergiuBool )
                {
                    //Calcula resultado Ano
                    _fluxoMensal._resFluxoMensal.CalculaResAno(lstResultadoFluxo, alim, _paramGerais.getNomeArqPerdasAno(), _janela);
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
            string alimTmp = _paramGerais.getNomeAlimAtual();

            //referência de geração (semana típica ou medição do MECE)
            double refGeracao;
            if (_janela._mapDadosDemanda.ContainsKey(alimTmp))
            {
                refGeracao = _janela._mapDadosDemanda[alimTmp];
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
            while ( Math.Abs(geracao - refGeracao) > _paramGerais._parGUI.getPrecisao() )
            {
                //Verifica se foi solicitado o cancelamento.
                if (_janela._cancelarExecucao)
                {
                    return loadMult;
                }

                // atualiza loadMult
                loadMultAnt = loadMult;
                loadMult = _paramGerais._parGUI.getIncremento() * loadMult;

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
            double refEnergiaMes = _janela._reqEnergiaMes.getRefEnergia(alimentador, _paramGerais._parGUI.getMes());

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
            if (verificaEnergiaSimulada(refEnergiaMes))
            {
                _janela.ExibeMsgDisplayMW(alimentador + " ajustado com precisão " + _paramGerais._parGUI.getPrecisao().ToString() + "KWh.");

                return loadMult;
            }

            // Se fluxo não convergiu retorna loadMult alternativo
            if ( !_fluxoMensal._resFluxoMensal._convergiuBool )
            {
                _janela.ExibeMsgDisplayMW(alimentador + ": erro valor energia!");

                return _paramGerais._parGUI._loadMultAlternativo;
            }

            // 
            double energiaMesTmp = _fluxoMensal._resFluxoMensal.getEnergia();

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
            loadMult = ajustaLoadMult(energiaMesTmp, refEnergiaMes, loadMult, sentidoBusca);

            return loadMult;
        }

        // condicao de saida da funcao
        // energia simulada (energiaMes) esta proxima da energia medida (refEnergiaMes)
        internal bool verificaEnergiaSimulada(double refEnergiaMes)
        {
            // se aproximacao de energia estiver dentro do limite de precisao ja calculado, ignora nova otimizacao
            if (getDiferencaEnergia(refEnergiaMes) < _paramGerais._parGUI.getPrecisao())
            {
                return true;
            }
            return false;
        }

        //obtem diferenca de energia de acordo com o modo
        internal double getDiferencaEnergia(double refEnergiaMes)
        {
            double delta;

            delta = Math.Abs((_fluxoMensal._resFluxoMensal.getEnergia() - refEnergiaMes));

            return delta;
        }

        // ajusta loadMult incrementalmente
        internal double ajustaLoadMult(double energiaMes, double refEnergiaMes, double loadMult, int sentidoBusca = 1)
        {
            // contador fluxo de potência
            // OBS: considera a execução acima
            int contFP = 1;

            // variavel que armazena o loadMult anterior
            double loadMultAnt = loadMult;
            int sentidoBuscaAnt = sentidoBusca;

            // alimTmp
            string alimTmp = _paramGerais.getNomeAlimAtual();

            // enquanto erro da energia for maior que precisao
            while (getDiferencaEnergia(refEnergiaMes) > _paramGerais._parGUI.getPrecisao())
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
                    loadMult = loadMult * _paramGerais._parGUI.getIncremento();
                }
                // retrocede ajuste 
                else
                {
                    loadMult = loadMult / _paramGerais._parGUI.getIncremento();
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
                if ((_fluxoMensal._resFluxoMensal.getEnergia() > refEnergiaMes) && (sentidoBusca == 1) ||
                     (_fluxoMensal._resFluxoMensal.getEnergia() < refEnergiaMes) && (sentidoBusca == -1))
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
            Snap(_paramGerais.getNomeAlimAtual());

            //alimTmp
            string alimTmp = _paramGerais.getNomeAlimAtual();

            //informa usuario convergencia
            _janela.ExibeMsgDisplayMW(_fluxoDiario.getMsgConvergencia(null, alimTmp));

            // retorno
            return _fluxoDiario._resFluxo.getMaxKW();
        }

        // ExecutaFluxoDiarioOpenDSS
        internal void ExecutaFluxoDiarioOpenDSS()
        {
            // Executa fluxo diario
            _fluxoDiario.ExecutaFluxoDiario();

            //Plota perdas na tela
            _janela.ExibeMsgDisplayMW(_fluxoDiario._resFluxo.getResultadoFluxoToConsole(
                _paramGerais.GetTensaoSaidaSE(), _paramGerais.getNomeAlimAtual()));
        }
    }
}

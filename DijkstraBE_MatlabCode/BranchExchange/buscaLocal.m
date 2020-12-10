% encapsula busca local
function [populacao, fxi] = buscaLocal(populacao,fxi,alim,geracao)

global paramAG;

% TODO 
% OBS: necessario desligar a Codificacao Inteira Reduzida no Branch Exchange
oldVAr = paramAG.indTSred;
paramAG.indTSred =0;

% fitness Inicial
fit1 = fxi(1);

sis = getSistema(alim.Fnome);

% resumo dos testes

% se rede 4 ou 6, executa 2 Buscas locais em serie  sis == 6) 
if ( (sis==6) || (sis==4) ) %
     
    %[populacao, fxi] = bestSetup(populacao, fxi,alim);

     [populacao, fxi] = setup2(populacao, fxi,alim,geracao);
    
else
    
    % busca local
    [populacao, fxi] = buscaLocalElite(populacao,fxi,alim);
    
end

% % TODO 
paramAG.indTSred = oldVAr;

end

function [populacao, fxi] = setup2(populacao, fxi,alim,geracao)

global paramAG;

% fitness Inicial
fitOld = fxi(1);

    paramAG.tipoOrdCiclos = 'tamCiclo';

    % busca local
    [populacao, fxi] = buscaLocalElite(populacao,fxi,alim);
       
    paramAG.tipoOrdCiclos = 'aleatorio';

    % busca local
    [populacao, fxi] = buscaLocalElite(populacao,fxi,alim); 

% OBS: testar se nao otimizou a BL testa individuo aleatorio.
if (fitOld == fxi(1))
    
    % busca local por ciclos
    [populacao, fxi] = buscaLocalClusterCiclos(populacao,fxi,alim); 

    % OBS: se desligar ocorre piora na rede4 (otimo sai da 10geracao p/19)
    paramAG.tipoOrdCiclos = 'aleatorio';

    % busca local
    [populacao, fxi] = buscaLocalIndAleatorio(populacao,fxi,alim);    
end

end

function [populacao, fxi] = bestSetup(populacao, fxi,alim)

global paramAG;

% fitness Inicial
fit1 = fxi(1);

% OBS: melhores resultados para rede 4 eh executar 2 BL 'tamCiclo' e
% aleatorio
paramAG.tipoOrdCiclos = 'tamCiclo';

% busca local
[populacao, fxi] = buscaLocalElite(populacao,fxi,alim);

paramAG.tipoOrdCiclos = 'aleatorio';

% busca local
[populacao, fxi] = buscaLocalElite(populacao,fxi,alim);  

% OBS: testar se nao otimizou a BL testa individuo aleatorio.
if (fit1 == fxi(1))

    % busca local por ciclos
    [populacao, fxi] = buscaLocalClusterCiclos(populacao,fxi,alim); 

    % OBS: se desligar ocorre piora na rede4 (otimo sai da 10geracao p/19)
    paramAG.tipoOrdCiclos = 'aleatorio';

    % busca local
    [populacao, fxi] = buscaLocalIndAleatorio(populacao,fxi,alim); 

end

end

% busca loca ciclo a ciclo
function [populacao, fxi] = buscaLocalClusterCiclos(populacao,fxi,alim)

global paramAG;
paramAG.tipoOrdCiclos = 'cluster';

% seleciona UM individuos p/ realizar busca local
[individuo, indFxi] = selecionaIndividuoBuscaLocal(populacao,fxi);

% correcao de lacos da populacao 
[newPop,newFxi] = buscaLocalBranchExchangeClusterCiclos(individuo, alim);

if (isempty(newPop))
   return; 
end

% adiciona na nova populacao os individuos radiais
[populacao,fxi] = adicionaSeNaoExistir(newPop,newFxi,populacao,fxi,individuo,alim);

% % poda tam populacao
[populacao, fxi] = podaTamPopulacao(populacao,fxi,alim);

end

function ind = getIndividuoAleatorio(populacao,alim)

sis = getSistema(alim.Fnome);
global paramAG;

% retira elite
populacao = populacao(2:end,:);

% OBS: melhor resultado para rede 4
if ((sis == 4)||(sis==6))
    
    % escolhe individuo do tamanho da populacao inicial
    vecAle = randperm(paramAG.tamPopulacao);

else
    % escolhe individuo aleatorio da populacao
    vecAle = randperm(size(populacao,1));
end

ind = populacao(vecAle(1),:);

end

% encapsula busca local
function [populacao, fxi] = buscaLocalIndAleatorio(populacao,fxi,alim)

% obtem individuo aleatorio (que nao seja o elite)
individuo = getIndividuoAleatorio(populacao,alim);

% busca local PVT 
[populacao, fxi] = buscaLocalPvt(individuo, populacao,fxi,alim);

end

% encapsula busca local
function [populacao, fxi] = buscaLocalElite(populacao,fxi,alim)

% seleciona UM individuos p/ realizar busca local
individuo = selecionaIndividuoBuscaLocal(populacao,fxi);

% busca local PVT 
[populacao, fxi] = buscaLocalPvt(individuo, populacao,fxi,alim);

end

% encapsula busca local
function [populacao, fxi] = buscaLocalPvt(individuo,populacao,fxi,alim)

% condicao de retorno
if ( isempty(individuo) )    
    return;    
end

% correcao de lacos da populacao 
[newPop,newFxi] = buscaLocalBranchExchange(individuo, alim);

if (isempty(newPop))
    return;
end

% adiciona na nova populacao os individuos radiais
[populacao,fxi] = adicionaSeNaoExistir(newPop,newFxi,populacao,fxi,individuo,alim);

% % poda tam populacao
[populacao, fxi] = podaTamPopulacao(populacao,fxi,alim);

end


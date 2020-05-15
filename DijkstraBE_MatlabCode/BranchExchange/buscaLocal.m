% encapsula busca local
function [populacao, fxi] = buscaLocal(populacao,fxi,alim)

global paramAG;

% TODO 
% OBS: necessario desligar a Codificacao Inteira Reduzida no Branch Exchange
oldVAr = paramAG.indTSred;
paramAG.indTSred =0;

% fitness Inicial
fit1 = fxi(1);

sis = getSistema(alim.Fnome);

% se rede 4 ou 6, executa 2 Buscas locais em serie  sis == 6) 
if ( (sis==6) || (sis==4) ) % || (sis==4)

    % OBS: melhores resultados para rede 4 eh executar 2 BL 'tamCiclo' e
    % aleatorio
    paramAG.tipoOrdCiclos = 'tamCiclo';

    % busca local
    [populacao, fxi] = buscaLocalPvtElite(populacao,fxi,alim);

    paramAG.tipoOrdCiclos = 'aleatorio';

    % busca local
    [populacao, fxi] = buscaLocalPvtElite(populacao,fxi,alim);    
    
else
    
%  OBS: escolhido em setaConfiguracoesAG
% %     paramAG.tipoOrdCiclos = 'aleatorio';

    % busca local
    [populacao, fxi] = buscaLocalPvtElite(populacao,fxi,alim);
    
end

% OBS: testar se nao otimizou a BL testa individuo aleatorio.
if (fit1 == fxi(1))
    
%     paramAG.tipoOrdCiclos = 'cluster';
% 
%     % busca local por ciclos
%     [populacao, fxi] = buscaLocalClusterCiclos(populacao,fxi,alim); 

    % OBS: se desligar ocorre piora na rede4 (otimo sai da 10geracao p/19)
    paramAG.tipoOrdCiclos = 'aleatorio';

    % busca local
    [populacao, fxi] = buscaLocalPvt(populacao,fxi,alim); 
    
end

% % TODO 
paramAG.indTSred = oldVAr;

end

% busca loca ciclo a ciclo
function [populacao, fxi] = buscaLocalClusterCiclos(populacao,fxi,alim)

% % elite 
% individuo = populacao(1,:);

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
function [populacao, fxi] = buscaLocalPvt(populacao,fxi,alim)

% obtem individuo aleatorio (que nao seja o elite)
individuo = getIndividuoAleatorio(populacao,alim);

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

% encapsula busca local
function [populacao, fxi] = buscaLocalPvtElite(populacao,fxi,alim)

% seleciona UM individuos p/ realizar busca local
[individuo, indFxi] = selecionaIndividuoBuscaLocal(populacao,fxi);

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

% % OBS: performance ruim rede4
% % poda tam populacao
[populacao, fxi] = podaTamPopulacao(populacao,fxi,alim);

end


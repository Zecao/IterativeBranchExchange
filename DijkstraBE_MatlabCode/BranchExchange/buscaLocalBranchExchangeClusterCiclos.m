% correcao de lacos da populacao
function [novosIndividuos, newFxi] = buscaLocalBranchExchangeClusterCiclos(individuo, alim)

% init vars
newFxi = [];

% chama otimiza ciclo
novosIndividuos = otimizaCicloIndividuo(individuo,alim);

% se criou novos individuos 
if ( ~isempty(novosIndividuos) )

    % introduz todos os individuos na POP ORIGINAL
    % avalia populacao
    newFxi = avaliaPopulacao(novosIndividuos, alim, []);   

    % seta status de BuscaLocal do individuo p/ true 
    setStatusBuscaLocalInd(individuo);

    % ordena decrescente
    [newFxi, ind] = sort(newFxi);
    novosIndividuos = novosIndividuos(ind,:);

end

end

% correcao de lacos
% retorna lista de individuos com ciclos 
function lstIndividuos = otimizaCicloIndividuo(indBin,alim)

% transforma binario p/ TS 
indTS = binario2tieSwitch(indBin,alim);

% OBS: 
lstIndividuos = otmCicloPorCluster(indTS,indBin,alim);
 
% transforma p/ binario
lstIndividuos = tieSwitch2binario(lstIndividuos,alim);

% verifica radialidade dos novos individuos
radial = verificaRadialidadeBGL(alim, lstIndividuos);

% retorna somente os individuos radiais
lstIndividuos = lstIndividuos(radial,:);

end

% 
function lstIndividuos = otmCicloPorCluster(indTS,indBinOriginal,alim)

% detecta os clusters. i.e. conjunto de ciclos com mesmas saidas da SE ("arestas pais")
% isto eh feito por meio da matriz matrizTSxArestasOrigem
% com as arestas finais (mais proximas a SE) de cada chaveTS 
matrizTSxArestasOrigem = criaMapArestasOrigem(indTS,alim);

% cria map TS x Ciclos com mesmas Arestas da SE. i.e "Ciclos internos"
mapNumClusterXtieSwitchs = criaMapClusterCiclosComMesmasArestasSE(matrizTSxArestasOrigem);

% realiza Branch Exchange por ciclo
lstIndividuos = otimizaCicloCluster(mapNumClusterXtieSwitchs,indBinOriginal,alim);

end

% correcao de lacos por cluster
function [lstNovosInds] = otimizaCicloCluster(mapNumClusterXtieSwitchs,indBinOriginal,alim)

lstNovosInds=[];

% obtem individuo TS
indTSOriginal = binario2tieSwitch(indBinOriginal,alim);

chaves = cell2mat(mapNumClusterXtieSwitchs.keys);

% OBS: nao faz sentido "aleatorizar" por cluster, pq um cluster nao altera o outro. 
% Os beneficios sao capturados somente em outra iteracao para cada cluster
for i=chaves

    % chaves TS que serao otimizadas
    indTS = mapNumClusterXtieSwitchs(i);

    % 1 analise, por cluster, eh realizada com os outros ciclos FECHADOS
    indBinOriginal(:) = 1;
    
    % OBS: TODO testar tambem com a abertura dos ciclos do mesmo cluster
%     indBinOriginal(indTS) = 0;

    % otimizaCicloClusterPvt
    novoInd = otimizaCicloClusterPvt(indTS,indBinOriginal,indTSOriginal,alim);

    % TODO testar comentar na rede 4
    % analisa oredem reversa
    novoInd2 = otimizaCicloClusterPvt(flip(indTS),indBinOriginal,indTSOriginal,alim);

    % se criou ind na ordem reversa
    if (any (novoInd ~= novoInd2))
        novoInd = [novoInd; novoInd2];
    end

    % lstIndividuos
    lstNovosInds = [lstNovosInds; novoInd];

end

% filtra lstIndividuos
lstNovosInds = unique(lstNovosInds, 'rows');

end

% correcao de lacos por cluster
function novoInd = otimizaCicloClusterPvt(indTS,indBinOriginal,indTSOriginal,alim)

% inicializa novo ind
novoInd = indTSOriginal;

% % % faz as buscas por ciclo em ordem aleatoria.
indices = randperm(size(indTS,2));

% TODO rede4: analisar ciclo 147 e 150, pois substitui para 150 e 150
% numero de TS 

% TODO rede 4  USA  length(indTS) 

for j=1:indices % length(indTS) 

    % abre chave NA 
    indBinOriginal(indTS(j)) = 0;
    
    % DEBUG
    if (indTS(j)==158)
        debug=0;
    end

    % obtem a chave da condicao de menor perda p/ dada chave indTS(i)
    chaveMP = otimizacaoAberturaCiclo(indTS(j), indBinOriginal, alim);

    % se alterou a TS
    if(chaveMP ~= indTS(j))

        % altera indBin (para analise da proximo ciclo)
        indBinOriginal(chaveMP) = 0;

        % procura TS antiga no indTSOriginal
        ind = find(indTSOriginal==indTS(j));

        % atualiza novo Ind
        novoInd(ind) = chaveMP;

    end

    % fecha chave NA (analisada)
    indBinOriginal(indTS(j)) = 1;

end

end

% cria map TS x Ciclos com mesmas Arestas da SE. Ciclos internos
function mapNumClusterXtieSwitchs = criaMapClusterCiclosComMesmasArestasSE(matrizTSxArestasOrigem)

% vetor TS 
vecTS = matrizTSxArestasOrigem(:,1);
mxArestas = matrizTSxArestasOrigem(:,2:3);

% init vars
iCluster=1;
mapNumClusterXtieSwitchs = containers.Map('KeyType', 'double', 'ValueType', 'any');


% percorre hash, ate contador ficar maior que o vecTS
while ( size(vecTS,1) > 1 )

    % pega a 1a TS em analise
    TS = vecTS(1);

    % paega arestas da 1a em analise
    arestasDaTS = mxArestas(1,:);

    % procura outra TS com a aresta 1
    [i1,j1] = find(mxArestas(:,1)==arestasDaTS(1));

    % procura outra TS com a aresta 2
    [i2,j2] = find(mxArestas(:,2)==arestasDaTS(2));
    
    % a intersecao dos indices indica a ocorrencia do laço com mesmas saidas
    % (arestas) da SE
    ind = intersect(i1,i2);

    % se ocorreu intercao maior que 1 (1=ele mesmo)
    if (size(ind,1)>1)

        data =vecTS(ind)';
        
        mapNumClusterXtieSwitchs(iCluster) = data;

        % inc. contador
        iCluster = iCluster + 1;

    end

    % remove TS e arestas apos a adicao no Map
    vecTS(ind)=[];
    mxArestas(ind,:)=[]; 

end

end


function matrizTSxArestasOrigem = criaMapArestasOrigem(indTS,alim)

% cria alim sparse 
branchesSparse = criaSparseMBranch(alim);

% cria map para armazenar arestas de origem (SE)
hashTSxArestasOrigem = containers.Map('KeyType', 'double', 'ValueType', 'any');
matrizTSxArestasOrigem =[];

for TS=indTS;

    % obtem vertices chave (oBS: i e j tem 2 elementos porque grafo UNDIRECTED
    [i,j] = find(alim.FmapVerticesArestas==TS);

    cabAlim = alim.FbarraIdCab;

    % % calcula o menor caminho (ciclo) entre os vertices da chave ate a cabAlim
    % [d1, pred1] = shortest_paths(branchesSparse, cabAlim, struct('target',i(1)) );
    % [d2, pred2] = shortest_paths(branchesSparse, cabAlim, struct('target',j(1)) );

    [d1, pred1] = shortest_paths(branchesSparse, i(1), struct('target', cabAlim));
    [d2, pred2] = shortest_paths(branchesSparse, j(1), struct('target', cabAlim));

    % substitui Inf por -1 
    arest1 = pred1(cabAlim);
    arest2 = pred2(cabAlim);

    % armazena arestas no map
    hashTSxArestasOrigem(TS)=[arest1,arest2];

    matrizTSxArestasOrigem = [matrizTSxArestasOrigem; TS, arest1, arest2];
end



end

% OLD CODE
% % correcao de lacos com outras chaves fechadas 
% function indTS = otmCicloPorClusterPvt(indTS,indBinOriginal,alim)
% 
% % cont 
% cont = 1;
% 
% for TS=indTS
% 
%     % fecha todas as chaves NAs
%     indBinOriginal(:)=1;
% 
%     % abre chave NA
%     indBinOriginal(TS)=0;
% 
%     % obtem a chave da condicao de menor perda p/ dada chave indTS(i)
%     chaveMP = otimizacaoAberturaCiclo(TS, indBinOriginal, alim);
% 
%     % se alterou a TS
%     if(chaveMP ~= TS)
% 
% %         % fecha chave NA
% %         indBinOriginal(TS)=1;
% % 
% %         % abre NOVA chave NA
% %         indBinOriginal(chaveMP)=0;
%     
%         % altera retorno funcao
%         indTS(cont) = chaveMP;
% 
%     end
% 
%     cont = cont + 1;
% 
% end
% 
% end

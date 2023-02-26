% correcao de lacos da populacao
function [novosIndividuos, newFxi] = buscaLocalBranchExchangeClusterCiclos(indBin, alim)

% init vars
newFxi = [];

% chama otimiza ciclo
novosIndividuos = buscaLocalBranchExchangeClusterPvt(indBin,alim);

% se criou novos individuos 
if ( ~isempty(novosIndividuos) )

    % introduz todos os individuos na POP ORIGINAL
    % avalia populacao
    newFxi = avaliaPopulacao(novosIndividuos, alim, []);   

    % seta status de BuscaLocal do individuo p/ true 
    setStatusBuscaLocalInd(indBin);

    % ordena decrescente
    [newFxi, ind] = sort(newFxi);
    novosIndividuos = novosIndividuos(ind,:);

end

end

% correcao de lacos
% retorna lista de individuos com ciclos 
function lstIndividuos = buscaLocalBranchExchangeClusterPvt(indBin,alim)

% atualiza alim
alim = setaBranchsAtivos(alim,indBin);

% transforma binario p/ TS 
indTS = binario2tieSwitch(indBin,alim);

% separa as TS por Clusters do mesmo ciclo
lstIndividuos = branchExchangeClusters(indTS,indBin,alim);

% transforma p/ binario
lstIndividuos = tieSwitch2binario(lstIndividuos,alim);

% verifica radialidade dos novos individuos
radial = verificaRadialidadeBGL(alim, lstIndividuos);

% retorna somente os individuos radiais
lstIndividuos = lstIndividuos(radial,:);

end

% 
function lstIndividuos = branchExchangeClusters(indTS,indBinOriginal,alim)

% detecta os clusters. i.e. conjunto de ciclos com mesmas saidas da SE ("arestas pais")
% isto eh feito por meio da matriz matrizTSxArestasOrigem
% com as arestas finais (mais proximas a SE) de cada chaveTS 
hashTSxArestasDoCiclo = criaMapTS_VerticesProxSE(indTS,alim);

% cria map(cluster) de VerticesProxSe X TS, definindo as TS que "compartilham" mesmos ciclos
hashClustersXTieSwitches = criaMapClusterCiclosComMesmasArestasSE2021(hashTSxArestasDoCiclo);

% OBS: versao antiga que nao armazena os ciclos sozinhos (i.e. sem
% clusters)
% hashClustersXTieSwitchesOLD = criaMapClusterCiclosComMesmasArestasSE_OLD(hashTSxArestasDoCiclo);

% realiza Branch Exchange por ciclo
% lstIndividuos = branchExchangeClustersCiclo_OLD(hashClustersXTieSwitches,indBinOriginal,alim);
lstIndividuos = branchExchangeClustersCiclo2021(hashClustersXTieSwitches,indBinOriginal,alim);

end

% correcao de lacos por cluster
function [lstNovosInds] = branchExchangeClustersCiclo2021(hashClustersXTieSwitches,indBinOri,alim)

lstNovosInds=[];
novoInd=[];

% obtem individuo TS
indTSOriginal = binario2tieSwitch(indBinOri,alim);

% 1 analise, por cluster, eh realizada com os outros ciclos FECHADOS
indBinFechado = indBinOri;
indBinFechado(:) = 1;

newFxi = avaliaPopulacao(indBinFechado, alim, []); 

% sistema
sis = getSistema(alim.Fnome);

% % setup rede6 
if (sis==6)
    
%     % DEBUG
%     debug = avaliaPopulacao(indBinOri, alim, []); 
    
    % ###################################
    % TODO otimizar um ciclo por geracao 
    
    % OBS: nao faz sentido "aleatorizar" por cluster, pq um cluster nao altera o outro. 
    % Os beneficios sao capturados somente em outra iteracao para cada cluster
    for i=hashClustersXTieSwitches.keys

%         disp('Analisando cluster:') 
%         disp(i)
        
        % chaves TS que serao otimizadas
        indTS = hashClustersXTieSwitches(char(i));

        % se contem 1 ciclo
        if (size(indTS,2)==1)
            
            continue;
%             % OBS piora o desempenho com rede 4 (de 290.4 FP p/ 317 FP)
%             novoInd = otimizaCicloClusterPvt2021(indTS,indBinOriginal,indTSOriginal,alim);

        else
            
%             % TODO testando
%             %abre somente as TS do ciclo analisado.
% OBS: analisar todas as chaves fechadas pode causar repeticao de TS... 
%             indBinFechado(indTS) = 0; 

            novoInd = otimizaCicloCluster2021_Pvt(indTS,indBinFechado,indTSOriginal,alim);

        end 
            % lstIndividuos
            lstNovosInds = [lstNovosInds; novoInd];  
    end

else

    % OBS: nao faz sentido "aleatorizar" por cluster, pq um cluster nao altera o outro. 
    % Os beneficios sao capturados somente em outra iteracao para cada cluster
    for i=hashClustersXTieSwitches.keys

        % chaves TS que serao otimizadas
        indTS = hashClustersXTieSwitches(char(i));

        global param;
        param.NCALBL_iter = param.NCALBL;    
        
        % se contem 1 ciclo
        if (size(indTS,2)==1)

        % 	 obs2023
%              novoInd = otimizaCicloClusterPvt2021(indTS,indBinOri,indTSOriginal,alim);

        else

            novoInd = otimizaCicloCluster2021_Pvt(indTS,indBinFechado,indTSOriginal,alim);
            
%             % lstIndividuos
%             lstNovosInds = [lstNovosInds; novoInd];  

        end 
%              lstIndividuos % obs2023
           lstNovosInds = [lstNovosInds; novoInd];  
    end
end

% filtra lstIndividuos
lstNovosInds = unique(lstNovosInds, 'rows');

end

% nao da bons resultados, otimizar maiores ciclos primeiro.
function setup_417barras_r1

    % otimiza somente os clusters com mais de 1 ciclo
    while (hashClustersXTieSwitches.length() > 1)      

        % debug
        % procura ciclo 
        if (hashClustersXTieSwitches.isKey('123-190'))
            disp('Criou cluster 123-190');
        end
        
        % TODO testando.
        chaveMaiorCiclo = getMaiorCiclo_hashTSxArestasDoCiclo(hashClustersXTieSwitches);
    
        % chaves TS que serao otimizadas
        indTS = hashClustersXTieSwitches(chaveMaiorCiclo);
        
        % debug 
        if (strcmp(chaveMaiorCiclo,'364-364'))&& all(indTS == [163,179,191,195,392,395,403,404]) 
            % remove ciclo ja analisado
            hashClustersXTieSwitches.remove(chaveMaiorCiclo);
            continue;
        end
        % debug 
        if (strcmp(chaveMaiorCiclo,'123-123'))&& all(indTS == [96,136,156,158,168]) 
            % remove ciclo ja analisado
            hashClustersXTieSwitches.remove(chaveMaiorCiclo);
            continue;
        end
            
        % se contem 1 ciclo, sai do while
        if (size(indTS,2)==1)
           break; 
        end
        
        % OBS: desempenho cai p/ rede 4  
%     % reabre chaves NA do ciclo somente
        clusterAberto = indBinFechado; 
        clusterAberto(indTS) =0;
        
        % analisa com cluster aberto
        novoInd = otimizaCicloCluster2021_Pvt(indTS,clusterAberto,indTSOriginal,alim);
        
        % se individuo diferente
        if (all (novoInd == indTSOriginal))
            lstNovosInds = [lstNovosInds; novoInd];
        end
        
        % analisa com cluster fechado 
        novoInd2 = otimizaCicloCluster2021_Pvt(indTS,indBinFechado,indTSOriginal,alim);

        % se individuo diferente
        if (all (novoInd2 == indTSOriginal))
            lstNovosInds = [lstNovosInds; novoInd2];
        end

        % remove ciclo ja analisado
        hashClustersXTieSwitches.remove(chaveMaiorCiclo);
        
    end


end

function [lstNovosInds] = branchExchangeClustersCiclo_OLD(mapNumClusterXtieSwitchs,indBinOriginal,alim)

lstNovosInds=[];

% obtem individuo TS
indTSOriginal = binario2tieSwitch(indBinOriginal,alim);

clusters = cell2mat(mapNumClusterXtieSwitchs.keys);

% 1 analise, por cluster, eh realizada com os outros ciclos FECHADOS
indBinOriginal(:) = 1;

% OBS: nao faz sentido "aleatorizar" por cluster, pq um cluster nao altera o outro. 
% Os beneficios sao capturados somente em outra iteracao para cada cluster
for i=clusters

    % chaves TS que serao otimizadas
    indTS = mapNumClusterXtieSwitchs(i);

    % se contem 1 ciclo
    if (size(indTS,2)==1)

%             % TODO testar otmCicloNormal.
%             novoInd = otimizaCicloClusterPvt2021(indTS,indBinOriginal,indTSOriginal,alim);

    else

        novoInd = otimizaCicloCluster2021_Pvt(indTS,indBinOriginal,indTSOriginal,alim);

    end 

    % lstIndividuos
    lstNovosInds = [lstNovosInds; novoInd];    

end

% filtra lstIndividuos
lstNovosInds = unique(lstNovosInds, 'rows');

end

% ordenar pelos maiores ciclos.
function  [chaveMaiorCiclo, hashTSxArestasDoCiclo] = getMaiorCiclo_hashTSxArestasDoCiclo(hashTSxArestasDoCiclo)

% vetor TS 
lstTS = hashTSxArestasDoCiclo.keys();

% atribui a maior chave como primeira.
chaveMaiorCiclo = char(lstTS(1));
numArestas1 = size(hashTSxArestasDoCiclo(chaveMaiorCiclo),2);

% remove 1a ocorrencia
lstTS(1)=[]; 

while (~isempty(lstTS))
    
    key2 = char(lstTS(1));
    numArestas2 = size(hashTSxArestasDoCiclo(key2),2);
    
    % sobre escreve chave maior ciclo
    if ( numArestas2 > numArestas1 )
    
        numArestas1 = numArestas2;
        chaveMaiorCiclo = key2;
    end
    
    % remove 1a ocorrencia
    lstTS(1)=[]; 
end

end

%
function pop = otimizaCicloCluster2021_Pvt(indTS,indBinOriginal,indTSOriginal,alim)

sis = getSistema(alim.Fnome);
pop =[];

% versao otimizada p/ rede 136barras
if (sis == 4)

    % otimizaCicloCluster
    pop = otimizaCicloClusterPvt2021(indTS,indBinOriginal,indTSOriginal,alim);
    
% % OBS: nao da bons resultados
%     % otimizaCicloCluster
%     pop = otimizaCicloCluster_ChaveAbertas(indTS,indBinOriginal,indTSOriginal,alim);

% OBS: FP 378.5
%     % otimizaCicloCluster todas as Permutacoes
%     pop = otimizaCicloClusterPvt2021_Perm(indTS,indBinOriginal,indTSOriginal,alim);

else

    % otimizaCicloCluster
%     pop = otimizaCicloClusterPvt2021(indTS,indBinOriginal,indTSOriginal,alim);
    pop = otimizaCicloClusterPvt2021_rede6(indTS,indBinOriginal,indTSOriginal,alim);
% 9 erros Erro de interligacão entre chaves

end

end

% OBS: nao utilizada no momento
% analisa TODAS as possibilidades de 1 ciclo
function pop = otimizaCicloClusterPvt2021_Perm(indTS,indBinOriginal,indTSOriginal,alim)

pop=[];

% cria todas as permutacoes de indTS
% Ex:
%    118   106    96
%    118    96   106
%    106   118    96
%    106    96   118
%     96   118   106
%     96   106   118
ordemTS = perms(indTS);

% % OBS: piora desempenho rede4 (passa de 10 p/ 7 sucessos)
% indBinFechadoExcetoCiclo = indBinOriginal;
% indBinFechadoExcetoCiclo(:) = 1;
% indBinFechadoExcetoCiclo(indTS) = 0;

for i=1:size(ordemTS,1)

    novoInd = otimizaCicloClusterPvt2021(ordemTS(i,:),indBinOriginal,indTSOriginal,alim);

    pop = [pop; novoInd];

end

end

% correcao de lacos por cluster
function lstIndividuos = otimizaCicloClusterPvt2021_rede6(indTS,indBinAbertoOuFechado,indTSOriginal,alim)

% inicializa novo ind
novoInd = indTSOriginal;

% % % faz as buscas por ciclo em ordem aleatoria.
indices = randperm(size(indTS,2));

% vetor que guarda info se TS foi otimizada
otimizouTS(indTS) = 0;

% cria lista de individuos 
lstIndividuos = [];

% abre chave NA 
indBinAbertoOuFechado(indTS) = 0;

% numero de TS 
for j=indices 

%     % abre chave NA 
%     indBinAbertoOuFechado(indTS(j)) = 0;
    
    debug = indTS(j);
    
    % DEBUG rede6
%     if (debug == 195) % a chave195 esta retornando ERRADAMENTE a chave 163 
    if (debug == 330)
       debug2 = 0;
    end
    
%     % se nao otimizou TS  % TODO testar
%     if ( ~otimizouTS(indTS(i)) )
    
    % obtem a chave da condicao de menor perda p/ dada chave indTS(i)
    chaveMP = otimizacaoAberturaCiclo(indTS(j), indBinAbertoOuFechado, alim);
    
% end

    % se nao membro do indTS (isto eh foi alterada)
    if ( ~ismember(chaveMP,indTS) )
    
        % altera indBin (para analise da proximo ciclo)
        indBinAbertoOuFechado(indTS(j)) = 1;  % fecha chave NA (analisada)
        indBinAbertoOuFechado(chaveMP) = 0;

        % procura TS antiga no indTSOriginal
        ind = find(indTSOriginal==indTS(j));
  
        % atualiza novo Ind
        novoInd(ind) = chaveMP;

%     else % TODO testar
%         
%         % seta vetor otimizado
%         otimizouTS(indTS(i)) = 1;
    end

    % lstIndividuos
    lstIndividuos = [lstIndividuos; novoInd];

end

end

% correcao de lacos por cluster
function novoInd = otimizaCicloClusterPvt2021(indTS,indBinAbertoOuFechado,indTSOriginal,alim)

% TODO testar
% % escolhe ordem de otimizacao das chaves
% indTS = defineOrdemDeCiclosOtm(indTS,indBinOriginal,alim);

% inicializa novo ind
novoInd = indTSOriginal;

% % % faz as buscas por ciclo em ordem aleatoria.
indices = randperm(size(indTS,2));

% %     DEBUG rede 4
% % analisar ciclo 147 e 150, pois substitui para 150 e 150
%     if (indTS(1)==147)
%         debug=0;
%     end

% TODO ao analisar o cluster com as outras chaves fechadas, pode ocorrer 
% de repetir alguma TS mais de 1 vez. 

% numero de TS 
for j=indices 

    % abre chave NA 
    indBinAbertoOuFechado(indTS(j)) = 0;
    
    debug = indTS(j);
    
    % DEBUG rede6
%     if (debug == 195) % a chave195 esta retornando ERRADAMENTE a chave 163 
    if (debug == 330)
       debug2 = 0;
    end
    
    % obtem a chave da condicao de menor perda p/ dada chave indTS(i)
    chaveMP = otimizacaoAberturaCiclo(indTS(j), indBinAbertoOuFechado, alim);

    % se nao membro do indTS (isto eh foi alterada)
    if ( ~ismember(chaveMP,indTS) )
    
        % altera indBin (para analise da proximo ciclo)
        indBinAbertoOuFechado(chaveMP) = 0;

        % procura TS antiga no indTSOriginal
        ind = find(indTSOriginal==indTS(j));

        % TODO ?? FAZER FUNCAO QUE SUBSTITUI NOVO TS NO IND TS COMPLETO.
        
        % atualiza novo Ind
        novoInd(ind) = chaveMP;

    end

    % fecha chave NA (analisada)
    indBinAbertoOuFechado(indTS(j)) = 1;

end

end

% correcao de lacos por cluster
function novoInd = otimizaCicloCluster_ChaveAbertas(indTS,indBinOriginal,indTSOriginal,alim)

% inicializa novo ind
novoInd = indTSOriginal;

% % % faz as buscas por ciclo em ordem aleatoria.
indices = randperm(size(indTS,2));

% abre TODAS chaves NAs do ciclo 
indBinOriginal(indTS) = 0;
    
% numero de TS 
for j=indices

    % obtem a chave da condicao de menor perda p/ dada chave indTS(i)
    chaveMP = otimizacaoAberturaCiclo(indTS(j), indBinOriginal, alim);

    % se alterou a TS
    if(chaveMP ~= indTS(j))

        % fecha chave NA (analisada)
        indBinOriginal(indTS(j)) = 1;
        
        % abre nova NA
        indBinOriginal(chaveMP) = 0;

        % procura TS antiga no indTSOriginal/novoInd
        ind = find(novoInd==indTS(j));

        % atualiza novoInd
        novoInd(ind) = chaveMP;

    end

%     % TODO testar.
%     % fecha chave NA (analisada)
%     indBinOriginal(indTS(j)) = 1;
    
end

end

% cria map TS x Ciclos com mesmas Arestas da SE. Ciclos internos
function hashClustersXTieSwitches = criaMapClusterCiclosComMesmasArestasSE2021(matrizTSxArestasOrigem)

% vetor TS 
vecTS = matrizTSxArestasOrigem(:,1);
mxArestas = matrizTSxArestasOrigem(:,2:3);

% % cria map para armazenar arestas de origem (SE)
hashClustersXTieSwitches = containers.Map('KeyType', 'char', 'ValueType', 'any');

for i=1:size(vecTS,1)
    
    % a chave do hash eh uma composicao dos 2 vertices apos a SE.
    hashKey = strcat( num2str(mxArestas(i,1)),'-',num2str(mxArestas(i,2)) );
    
    % se hashKey ainda nao existe, armazena TS no hash 
    if (~ hashClustersXTieSwitches.isKey(hashKey))
        
        hashClustersXTieSwitches(hashKey) = vecTS(i);
        
    % append da TS em dados e recoloca no hash    
    else
        
        dados = hashClustersXTieSwitches(hashKey);
        
        dados = [dados, vecTS(i)];
        
        hashClustersXTieSwitches(hashKey) = dados;
    end
    
end

end

% cria map TS x Ciclos com mesmas Arestas da SE. Ciclos internos
function mapNumClusterXtieSwitchs = criaMapClusterCiclosComMesmasArestasSE_OLD(matrizTSxArestasOrigem)

% vetor TS 
vecTS = matrizTSxArestasOrigem(:,1);
mxArestas = matrizTSxArestasOrigem(:,2:3);

% init vars
iCluster=1;
mapNumClusterXtieSwitchs = containers.Map('KeyType', 'double', 'ValueType', 'any');

% percorre hash, ate contador ficar maior que o vecTS
while ( size(vecTS,1) > 1 ) % OLD CODE 2021
% while ( ~ isempty(vecTS) ) 

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
    if (size(ind,1)>1) % OLD CODE 2021

        data = vecTS(ind)';
        
        mapNumClusterXtieSwitchs(iCluster) = data;

        % inc. contador
        iCluster = iCluster + 1;

    end

    % remove TS e arestas apos a adicao no Map
    vecTS(ind)=[];
    mxArestas(ind,:)=[]; 

end

end

% OBS: nao utilizada
% monta matriz que informa as arestas de origem (isto é mais proximas a SE) de cada TS 
function hashTSxArestasOrigem = criaMapArestasOrigem(indTS,alim)

%OBS: atualiza individuo
indBinTmp = tieSwitch2binario(indTS,alim);
alim = setaBranchsAtivos(alim,indBinTmp); 

% cria map para armazenar arestas de origem (SE)
hashTSxArestasOrigem = containers.Map('KeyType', 'double', 'ValueType', 'any');

for TS=indTS;

    % public getArestasDoCiclo
    indicesArestas = getArestasDoCiclo(TS, alim);    

    % armazena arestas no map
    hashTSxArestasOrigem(TS)=indicesArestas;
    
end

end

% monta matriz que informa as arestas de origem (isto é mais proximas a SE) de cada TS 
function matrizTSxSegundoVerticeDaOrigem = criaMapTS_VerticesProxSE(indTS,alim)

indBinTmp = tieSwitch2binario(indTS,alim);
alim = setaBranchsAtivos(alim,indBinTmp); 

% cria alim sparse 
branchesSparse = criaSparseMBranch(alim);

matrizTSxSegundoVerticeDaOrigem =[];

for TS=indTS;

    % OLD CODE
%     % obtem vertices chave (oBS: i e j tem 2 elementos porque grafo UNDIRECTED
%     [i,j] = find(alim.FmapVerticesArestas==TS);

%     % obtem vertices chave
    [i,j] = getNos(TS,alim);
        
    cabAlim = alim.FbarraIdCab;

    % % calcula o menor caminho (ciclo) entre os vertices da chave ate a cabAlim
    % [d1, pred1] = shortest_paths(branchesSparse, cabAlim, struct('target',i(1)) );
    % [d2, pred2] = shortest_paths(branchesSparse, cabAlim, struct('target',j(1)) );

    [d1, pred1] = shortest_paths(branchesSparse, i, struct('target', cabAlim));
    [d2, pred2] = shortest_paths(branchesSparse, j, struct('target', cabAlim));

%     % DEBUG 
%     if (TS == 256)
%         debug=0;
%     end
    
    % substitui Inf por -1 
    vertice1 = pred1(cabAlim);
    vertice2 = pred2(cabAlim);
    
    % 2023. order vertex 
    vertex = sort([vertice1, vertice2]);    
    matrizTSxSegundoVerticeDaOrigem = [matrizTSxSegundoVerticeDaOrigem; TS, vertex];
%     matrizTSxSegundoVerticeDaOrigem = [matrizTSxSegundoVerticeDaOrigem;
%     TS, vertice1, vertice2]; % OLD CODE
end

end
% escolhe ordem de otimizacao das chaves
function indTS = defineOrdemDeCiclosOtm(indTS,indBinOriginal,alim)

global paramAG;

switch paramAG.tipoOrdCiclos
    
    case 'aleatorio'
        
        % 1 ordem aleatoria 
        indTS = indTS(randperm(size(indTS,2)));

    case 'tamCiclo'

        % 2 ordena por tamanho das sub-redes
        indTS = defOrdemCiclosPorTamanhoDosCiclos(indTS,indBinOriginal,alim);

    case 'revTamCiclo'

        % 2 ordena por tamanho das sub-redes
        indTS = defOrdemCiclosPorTamanhoDosCiclos(indTS,indBinOriginal,alim);
        
        % reverse
        indTS = fliplr(indTS);
                
    case 'distEletrica'
        
        % 2 ordena por tamanho das sub-redes
        indTS = defOrdemCiclosPorTamanhoDosCiclosDistEletrica(indTS,indBinOriginal,alim);
              
    otherwise
        
        % 1 ordem aleatoria 
        indTS = indTS(randperm(size(indTS,2)));   
end

end

% define a ordem dos ciclos por tamaho dos ciclos 
function novoIndTS = defOrdemCiclosPorTamanhoDosCiclos(indTS,indBinOriginal,alim)

% map com o numero de arestas (~tamanho) de cadas chave TS
mapNumArestasXChave = containers.Map('KeyType', 'single', 'ValueType', 'single');

% seta individuo corrente 
alim = setaBranchsAtivos(alim,indBinOriginal);   
    
% obtem tamanho, em arestas, de cada ciclo
for chaveI=indTS

    % get arestas do ciclo 
    indicesArestas = getArestasDoCiclo(chaveI, alim);
    
    % numero de arestas
    numArestas = length(indicesArestas);
    
    % map NumArestas X Chave
    mapNumArestasXChave = adicionaNumArestaMap(mapNumArestasXChave,numArestas,chaveI);
    
end

% ordena ciclos
vetorNumArestas  = mapNumArestasXChave.keys();

% ordena, em ordem decrescente, o numero de arestas
vetorNumArestasOrd = sort(cell2mat(vetorNumArestas),'descend');

novoIndTS =[];

% cria novo vetor TS
for i=vetorNumArestasOrd

    % vetor linha
    novoIndTS = [novoIndTS, mapNumArestasXChave(i)];
    
end
    
end

% obtem arestas do ciclo, considerando a resistencia dos ramos
function novoIndTS = defOrdemCiclosPorTamanhoDosCiclosDistEletrica(indTS,indBinOriginal,alim)

% map com o numero de arestas (~tamanho) de cadas chave TS
mapNumArestasXChave = containers.Map('KeyType', 'single', 'ValueType', 'single');

% seta individuo corrente 
alim = setaBranchsAtivos(alim,indBinOriginal);   
    
% obtem tamanho, em arestas, de cada ciclo
for chaveI=indTS

    % get arestas do ciclo 
    indicesArestas = getArestasDoCicloDistEletrica(chaveI, alim);
    
    % numero de arestas
    numArestas = length(indicesArestas);
    
    % map NumArestas X Chave
    mapNumArestasXChave = adicionaNumArestaMap(mapNumArestasXChave,numArestas,chaveI);
    
end

% ordena ciclos
vetorNumArestas  = mapNumArestasXChave.keys();

% ordena, em ordem decrescente, o numero de arestas
vetorNumArestasOrd = sort(cell2mat(vetorNumArestas),'descend');

novoIndTS =[];

% cria novo vetor TS
for i=vetorNumArestasOrd

    % vetor linha
    novoIndTS = [novoIndTS, mapNumArestasXChave(i)];
    
end
    
end
 
function mapNumArestasXChave = adicionaNumArestaMap(mapNumArestasXChave,numArestas,chaveI)

% TODO testar map inverso. Pois ha um erro neste algoritmo, na ocorrencia
% de 2 TSs com mesmo tamanho, uma delas ficara de fora do map

% a solucao paliativa (ELSE) foi adicionar + 1 no numero de arestas.

% se numArestas jah existe no map, adciona   
if ( ~mapNumArestasXChave.isKey(numArestas) )

    mapNumArestasXChave(numArestas) = chaveI;

else        
    
    mapNumArestasXChave = adicionaNumArestaMap(mapNumArestasXChave,numArestas+1,chaveI);

end

end
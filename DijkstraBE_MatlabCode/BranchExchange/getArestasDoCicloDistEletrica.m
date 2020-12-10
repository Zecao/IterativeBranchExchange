% public getArestasDoCiclo
% OBS: a diferenca entre esta funcao e a getArestasDoCiclo eh a chamada 
% a funcao criaSparseMBranchResistencia(alim);
function indicesArestas = getArestasDoCicloDistEletrica(chave, alim)

% TODO nao funciona com Cod. Inteira Compacta. Uma vez que a nomenclatura
% oficial das chaves faz referencia a branchesSparse

% obtem nos (pai e filho) da chave
[noPai,noFilho] = getNos(chave,alim);

% cria alim sparse 
%branchesSparse = criaSparseMBranch(alim);

% cria alim sparse 
branchesSparse = criaSparseMBranchResistencia(alim);

% cria arestas temporarias interligando geradores
branchesSparse = interligaGeradores2(branchesSparse,alim);

% calcula o menor caminho (ciclo) entre as duas 
[d, pred] = shortest_paths(branchesSparse, noPai, struct('target',noFilho) );

% DEBUG 
% debug = graph(branchesSparse);

% retorna indices das arestas do ciclo obtido com o fechamento de uma chave de interconexao
indicesArestas = getCicloChaveInterconexao(d,pred,noFilho,alim);

% verifica se encontrou ciclo 
if ( isempty(indicesArestas) )

    disp('Erro de interligacão entre chaves');
    
    return;
    
end

end

% retorna indices das arestas do ciclo obtido com o fechamento de uma chave de interconexao
function indArestas = getCicloChaveInterconexao(d,pred,noInicial,alim)

% init vars
indArestas = [];
distancia = d(noInicial);
proxNo = pred(noInicial);

% distancia noPai/noFilho nao chegar a 0
% OBS: a distancia pode ser Inf, por isto uso proximo no tb.
while (distancia ~= 0)&&(distancia ~= Inf) %(proxNo ~= 0)
          
    % getChavePorNos(noPai,noFilho,alim)
    arestas = getChavePorNos(noInicial,proxNo,alim);
    
    % append arestas
    indArestas = [indArestas; arestas];
    
    % atualiza contadores
    distancia = d(proxNo);
    noInicial = proxNo;
    proxNo = pred(proxNo);
    
end

end
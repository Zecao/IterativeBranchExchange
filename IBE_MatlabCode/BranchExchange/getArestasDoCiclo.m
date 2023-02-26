% TODO nao funciona com Cod. Inteira Compacta. Uma vez que a nomenclatura
% oficial das chaves faz referencia a branchesSparse

% public getArestasDoCiclo
function indicesArestas = getArestasDoCiclo(chave, alim)

% obtem nos (pai e filho) da chave
[noPai,noFilho] = getNos(chave,alim);

% cria alim sparse 
branchesSparse = criaSparseMBranch(alim);

% cria arestas temporarias interligando geradores
branchesSparse = interligaGeradores2(branchesSparse,alim);

% calcula o menor caminho (ciclo) entre as duas 
[d, pred] = shortest_paths(branchesSparse, noPai, struct('target',noFilho) );

% DEBUG 
% debug = graph(branchesSparse);

% retorna matriz esparsa contendo os indices das arestas do ciclo obtido com o fechamento de uma chave de interconexao
indicesArestas = getCicloChaveInterconexao(d,pred,noFilho,alim);

% verifica se encontrou ciclo 
if ( isempty(indicesArestas) )

    disp('Erro de interligacão entre chaves');
    indicesArestas=chave;
    return;
    
end

end

% % OLD CODE OBS: manter
% % obtem os nos geradores, atraves de DFS, dado um os nos de chave 
% function noGerador1 = getGeradorAlimPorDFS(noPai,grafoRede,alim)
% 
% % init vars
% noGerador1 = [];
% 
% % realiza DFS p/ verificar se todos sao energizados
% nosVisitadosDFS = dfs(grafoRede,noPai);
% 
% % nos raizes (geradores)
% [lstNoRaizes, indNosGeradores] = getNoRaizes(alim);
% 
% % ao fazer nosVisitados(indices), estou obtendo os nos geradores que foram
% % visitados pela DFS. 
% % nosGerVisitados = nosVisitadosDFS(indNosGeradores);
% 
% % para cada gerador, verifica se foi visitado
% for i=lstNoRaizes
%    
%     visitado = nosVisitadosDFS(i);
%     
%     % um no eh visitado tiver numero diferente de -1
%     if (visitado ~= -1)
%     
%         noGerador1 = i;
%     
%     end
%     
% end
% 
% % if (isempty(noGerador1))
% %    debug = 0; 
% % end
%     
% end

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
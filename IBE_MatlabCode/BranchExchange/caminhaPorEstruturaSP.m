% caminha pela estrutura retornada no algoritmo de menor caminho, 
% retornando os indices das arestas
% d vetor distancia
% pred 
% noInicial alim 
function indArestas = caminhaPorEstruturaSP(d,pred,noInicial,alim)

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
% verifica limites (indices das chaves) superior e inferior
function populacao = corrigeLimitesPopulacao(alim,populacao)

% get idTrechos manobraveis
[idTrechosManobraveis,indChavesManobraveis] = getTrechosChavesIEEE(alim);

% % trata indice maior que chave
% retorna numero de chaves totais alimentador 
nVars = size(idTrechosManobraveis,1);

for i=1:size(populacao,1)

    % restaura lst de indChavesLivres
    indChavesLivres = indChavesManobraveis;
    
    % remove chave da lista de individuos possiveis
    for j=1:length( populacao(i,:) )
        
        chave = populacao(i,j);

        indChavesLivres = removeLst(chave,indChavesLivres);

    end
    
    for j=1:length( populacao(i,:) )
        
        chave = populacao(i,j);

        % verifica limite superior
        if (chave > nVars)
        
            populacao(i,j)= max(indChavesLivres);

        end
        
        % verifica limite inferior
        if (chave < 1)
            
            populacao(i,j) = min(indChavesLivres);
            
        end

    end

end

end


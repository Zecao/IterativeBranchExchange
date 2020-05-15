% arredonda e trata individuo 
function pop = arredondaPopulacaoTS(pop,alim)

% arredonda
pop = round(pop);

% corrigeLimitesPopulacao
if ( strcmp(alim.Ftipo,'ieee') )
    
    pop = corrigeLimitesPopulacao(alim,pop);

    % corrigeLimitesPopulacaoCemig
else
    
    pop = corrigeLimitesPopulacaoCemig(alim,pop);
        
end

end

% verifica limites (indices das chaves) superior e inferior
function populacao = corrigeLimitesPopulacaoCemig(alim,populacao)

% get idTrechos manobraveis
[idTrechosManobraveis,indChavesManobraveis] = getTrechosChavesIEEE(alim);

% TODO otimizar!!
% OBS: unica diferencao da funcao 
% retorna numero de chaves totais alimentador 
nVars = size(alim.FmChavesIEEE,1);

% % trata indice maior que chave
for i=1:size(populacao,1)
    
    % get individuo
    individuo = populacao(i,:);
    
    % restaura lst de indChavesLivres
    indChavesLivres = indChavesManobraveis;
    
    for j=1:length(individuo)
        
        chave = individuo(j);
        
        % remove chave da lista de individuos possiveis
        indChavesLivres = removeLst(chave,indChavesLivres);
        
        % verifica limite superior
        if (chave > nVars)
        
            individuo(j)= max(indChavesLivres);

        end
        
        % verifica limite inferior
        if (chave < 1)
            
            individuo(j) = min(indChavesLivres);
            
        end
        
        % remove também individuo j (caso o mesmo tenha sido modificado) da lista de individuos possiveis
        indChavesLivres = removeLst( individuo(j), indChavesLivres);
        
    end
    
    % adiciona individuo na populacao
    populacao(i,:) = individuo;

end

end



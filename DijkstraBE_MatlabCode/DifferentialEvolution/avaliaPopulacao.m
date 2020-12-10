% Avalia a funcao objetivo para a Populacao.
function [ fitness, alim ] = avaliaPopulacao(populacao, alim, fitness)

% init var
avaliadoBool = false;

tamPopulacao = size(populacao,1);

% opcao fluxo 
if ( ~ strcmp(alim.funcao,'fluxo') )

    % decodifica populacao binaria 
    populacao = decodificaPopBinario(populacao, alim.funcao);

end

for i=1:tamPopulacao
    
    individuo = populacao(i, :);
    
    if (strcmp(alim.Ftipo,'ieee'))
    
        [avaliadoBool, fit] = getStatusAvaliacaoInd(individuo);
    
    end
    
    % verifica se individuo ja foi avaliado
    if ( ~avaliadoBool )

        [ fit, alim ] = avaliaIndividuo( individuo, alim);
    
    end

    % preenche fitness
    fitness(i,1) = fit;
    
end

end
%   Avalia funcao objetivo para cada individuo 
function [perdas, alim, results] = avaliaIndividuoBuscaLocal(individuo, alim)

% contador de avaliacoes das funcoes
global param
param.NCALBL = param.NCALBL + 1;

% inicio codigo especifico FluxoPotencia
if ( strcmp (alim.funcao, 'fluxo') )
 
    % cria nome do alimentador manobrado
    strAlim = strcat('Individuo',num2str(param.NCALBL));

    % configura Branchs de acordo com chaves
    alim = setaBranchsAtivos(alim,individuo);
    
    % roda fluxo
    [ results, perdas ] = rodaFluxoPotencia(strAlim, alim);
    
    % seta bool status avaliacao individuo
    setStatusAvaliacaoInd(individuo,perdas);
    
% inicio codigo
else
        
    perdas  = feval(alim.funcao, individuo);
    
end

end
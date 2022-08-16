% Evolucao Diferencial
function [ populacao, fxi ] = evolucaoDiferencialElite521(alim, populacao, fxi, geracao )

% minimizacao local na primeira geracao
if (geracao == 1)
   
    % minimizacao local na primeira geracao
    [populacao, fxi] = buscaLocal(populacao,fxi,alim,geracao);

end

% busca local
[populacao, fxi] = buscaLocal(populacao,fxi,alim,geracao);

end
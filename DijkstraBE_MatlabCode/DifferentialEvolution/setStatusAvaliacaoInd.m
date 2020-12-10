% seta bool status avaliacao individuo
function setStatusAvaliacaoInd(chave,perdas,results)

% calcula Index Individuo
index = calculaIndexIndividuo(chave);

% seta bool p/ true
global paramAG
paramAG.hashIndividuosCalculados(index) = perdas;
paramAG.hashResultsCalculados(index) = results;
end
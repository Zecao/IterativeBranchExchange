% seta bool status avaliacao individuo
function setStatusAvaliacaoInd(chave,perdas)

% calcula Index Individuo
index = calculaIndexIndividuo(chave);

% seta bool p/ true
global paramAG
paramAG.hashIndividuosCalculados(index) = perdas;

end
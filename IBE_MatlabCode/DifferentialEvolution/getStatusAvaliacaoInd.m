% verifica se individuo ja foi avaliado
function [avaliadoBool, fitness, results] = getStatusAvaliacaoInd(chave)

% init vars
fitness = Inf;
avaliadoBool = false;
results = [];

% calcula Index Individuo
index = calculaIndexIndividuo(chave);

% obtem bool se ja avaliou ou nao individuo
global paramAG;

% OBS: passou a dar pau qnd passei chave para char.
% chaves = cell2mat(paramAG.hashIndividuosCalculados.keys());

%
chaves = paramAG.hashIndividuosCalculados.keys();

if (ismember(index,chaves))

    fitness = paramAG.hashIndividuosCalculados(index);
    avaliadoBool = true; 
     
    results = paramAG.hashResultsCalculados(index);
    
end
 
end
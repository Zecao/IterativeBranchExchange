% verifica se individuo ja foi avaliado
function otimizadoBool = getStatusBuscaLocalIndCluster(individuo)

% calcula Index Individuo
index = calculaIndexIndividuo(individuo);

% obtem bool se ja avaliou ou nao individuo
global paramAG;

% % chaves do hashIndividuosOtmBuscaLocal    
chaves = paramAG.hashIndividuosOtmBuscaLocalCluster.keys();
 
%  por logica, se index do individuo jah for membro das chaves do hashIndividuosOtmBuscaLocal
if (ismember(index,chaves))
    
    otimizadoBool = true;
     
else

    otimizadoBool = false;
    
end
 
end
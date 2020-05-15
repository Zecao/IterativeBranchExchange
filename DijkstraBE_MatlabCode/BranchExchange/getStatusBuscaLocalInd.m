% verifica se individuo ja foi avaliado
function otimizadoBool = getStatusBuscaLocalInd(individuo)

% calcula Index Individuo
index = calculaIndexIndividuo(individuo);

% obtem bool se ja avaliou ou nao individuo
global paramAG;

% chaves do hashIndividuosOtmBuscaLocal
if (strcmp(paramAG.tipoOrdCiclos,'cluster'))
    
    chaves = paramAG.hashIndividuosOtmBuscaLocalCluster.keys();
    
else
    
    chaves = paramAG.hashIndividuosOtmBuscaLocal.keys();
    
end

%  por logica, se index do individuo jah for membro das chaves do hashIndividuosOtmBuscaLocal
if (ismember(index,chaves))
    
    otimizadoBool = true;
     
else

    otimizadoBool = false;
    
end
 
end
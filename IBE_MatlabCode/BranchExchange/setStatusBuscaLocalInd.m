% seta status de BuscaLocal do individuo p/ true  
function setStatusBuscaLocalInd(individuo)

% calcula Index Individuo
index = calculaIndexIndividuo(individuo);

global paramAG;

if ( strcmp(paramAG.tipoOrdCiclos,'cluster'))
    
    paramAG.hashIndividuosOtmBuscaLocalCluster(index) = true;

else
    
    paramAG.hashIndividuosOtmBuscaLocal(index) = true;

end

end
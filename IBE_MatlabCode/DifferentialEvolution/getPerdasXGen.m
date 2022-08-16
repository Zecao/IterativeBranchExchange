% obtem vetor perdas(elite) por geracao 
function perdasXGen = getPerdasXGen(arrayStructElite)

% global numGeracoes;
global paramAG;

perdasXGen = [];

for i=1:paramAG.numGeracoes
    
    perdasXGen = [perdasXGen; arrayStructElite(i).FFxXGen ];
  
end

end

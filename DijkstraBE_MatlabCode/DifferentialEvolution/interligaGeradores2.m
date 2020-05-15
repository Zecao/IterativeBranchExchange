% cria arestas temporarias interligando geradores
function branches = interligaGeradores2(branches,alim)

% obtem barras geradoras lstNoRaizes
noRaizes = getNoRaizes(alim); 

% cria arestas
for i=1:length(noRaizes)-1
   
    x = noRaizes(i);
    y = noRaizes(i+1);
    
    % coloca um peso pequeno na aresta para manter os os alimentadores 
    % interligados, no caso de rodar uma MST
    branches( x,y ) = 0.001;
    branches( y,x ) = 0.001; 

end

end

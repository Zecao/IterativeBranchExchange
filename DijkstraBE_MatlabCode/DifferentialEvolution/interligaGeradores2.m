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

% OLD CODE
% %´OBS: trecho p/ garantir compatibilidade c/ redes c/ mais de 1 gerador
% % interliga geradores, se necessitar
% nosRaizes = getNoRaizes(alim);
% 
% if ( length(nosRaizes) > 1 )
% 
%     comb = nchoosek(nosRaizes,2);
%     
%     for i=comb'
%         
%         % interliga geradores p/ poder utilizar o shortest_path    
%         branchesSparse = interligaGeradores(i(1),i(2),branchesSparse);   
%         
%     end
%     
% end
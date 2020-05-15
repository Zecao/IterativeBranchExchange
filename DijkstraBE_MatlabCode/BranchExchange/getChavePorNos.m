% obtem chave (indice aresta) por meio dos nos pais e filhos.
function chave = getChavePorNos(noPai,noFilho,alim)

% consulta map 
chave = alim.FmapVerticesArestas(noPai,noFilho);

% transforma para matriz normal
chave = full(chave);
 
% OBS: no caso dos alientadores com mais de um gerador, onde foi necessário 
% a interligação dos mesmos, o FmapVerticesArestas retorna 0 (i.e. matriz
% esparca). Assim eh necessario remove-lo, pois 0 nao pode representar o
% indice de uma chave
if (chave==0)

    chave = [];

end

end



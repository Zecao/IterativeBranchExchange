% cria arestas temporarias interligando geradores
function branches = interligaGeradores(gen1,gen2,branches)

% alterar peso p/ 1
branches(gen1,gen2) = 1;
branches(gen2,gen1) = 1;

end


% cria struct Alimentador Vazio
function alimentador = criaAlimVazio()

alimentador = struct;

% tipo: cemig ou ieee
alimentador = setfield(alimentador,'Ftipo', []);

alimentador = setfield(alimentador,'funcao', 'fluxo');
alimentador = setfield(alimentador,'Fnome', []);
alimentador = setfield(alimentador,'FbarraIdCab', []);
alimentador = setfield(alimentador,'FmTrafos', []); 
alimentador = setfield(alimentador,'FmChaves', []);
alimentador = setfield(alimentador,'FmBus', []);
alimentador = setfield(alimentador,'FmBranch', []);
alimentador = setfield(alimentador,'FmGerador', [] );

% campos formato Carlos IEEE
alimentador = setfield(alimentador,'FmBarras', [] );
alimentador = setfield(alimentador,'FmLinhas', [] );
alimentador = setfield(alimentador,'FmChavesIEEE', [] );

% campos alimentadores Cemig 
alimentador = setfield(alimentador,'FmapTrechoBarraMontante', []);
alimentador = setfield(alimentador,'FmapTrechoBarraJusante', []);
alimentador = setfield(alimentador,'FmapVerticesArestas', []);
alimentador = setfield(alimentador,'FlstAlimentadores',false);

end


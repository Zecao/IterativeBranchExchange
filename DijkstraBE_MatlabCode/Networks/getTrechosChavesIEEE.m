% get idTrechos das ChavesIEEE e seus indices
function [idTrechosManobraveis,indices] = getTrechosChavesIEEE(alim)

idTrechosManobraveis = alim.FmChavesIEEE(:,1);

indicesTrechosManobraveis = cast(alim.FmChavesIEEE(:,5),'logical');

idTrechosManobraveis = idTrechosManobraveis(indicesTrechosManobraveis);

% % indices
indices = 1:1:size(idTrechosManobraveis,1);
indices = indices';

end

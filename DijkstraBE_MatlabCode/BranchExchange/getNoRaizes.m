% funcao que retotna lst de nos raizes (geradores)
function [lstNoRaizes,indices] = getNoRaizes(alim)

bus = alim.FmBus;

% indices c/ tipo barra == 3 (gerador)
indices = bus(:,2) == 3;

% barras dos nos raizes
lstNoRaizes = bus(indices,1);

% transforma em vetor linha
lstNoRaizes = lstNoRaizes';

end

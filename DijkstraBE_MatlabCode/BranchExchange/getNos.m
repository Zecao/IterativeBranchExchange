% obtem nos (pai e filho) de um indice da matriz branch
function [noPai,noFilho] = getNos(indiceMBranch,alim)

noPai = alim.FmBranch(indiceMBranch,1);
noFilho = alim.FmBranch(indiceMBranch,2);

end
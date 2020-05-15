% ative Branchs de acordo com chaves
function [ alim ] = setaBranchsAtivos(alim,individuo)

%transpoe chaves
individuo = individuo';

% ativa Branch caso chave esteja fechada  
alim.FmBranch(:,11) =  individuo;

end


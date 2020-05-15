% seleciona os N melhores individuos da populacao de pais e filhos
function [ popNova, fitnessNovaOrd ] = selecaoUnique(popPais,popFilhos,alim)

global param;

membros = ismember(popFilhos, popPais, 'rows');
naoMembros = ~ membros;
param.SucessoSEL = param.SucessoSEL + sum(naoMembros);

% conjuntos totais
popNova = [popPais;popFilhos];

% unique
[popNova, ind] = unique(popNova,'rows');

% avalia populacao dos filhos.
fitnessNova = avaliaPopulacao(popNova, alim);

% ordena fitnessNova em ordem crescente
[fitnessNovaOrd, index ] = sort(fitnessNova);

popNova = popNova(index,:);

end


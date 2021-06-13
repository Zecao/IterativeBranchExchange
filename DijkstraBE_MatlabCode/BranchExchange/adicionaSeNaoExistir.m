% filtra novos individuos
function [populacao, fxi] = adicionaSeNaoExistir(e,f_e,populacao,fxi,alim)

% OBS: SE filtrar por fitness (e nao por individuo), poss estar excluindo
% individuos diferentes  e importantes que tem mapeada a mesma fitness
% adiciona novos individuos (nao existentes) em populacao 
% naoMembros = ~ ismember(e, populacao, 'rows');

membros = ismember(f_e, fxi);

% % TODO testando. 
% sis = getSistema(alim.Fnome);
% 
% if (sis == 4)
%     % se individuos criados ja existem na populacao, marca indviduo analisado 
%     if (all(membros))
% 
%         % seta status de BuscaLocal do individuo p/ true 
%         setStatusBuscaLocalInd(indAnalisado);
% 
%         return;
%     end
% end

% 
naoMembros = ~ membros;

% se nao existir nenhum membro
if ( ~ any(naoMembros) )
        
    return;

end

% adiciona novos individuos (nao existentes) em populacao 
populacao = [populacao; e(naoMembros,:)];
fxi = [fxi;f_e(naoMembros)];

% DEBUG
% sort(f_e(naoMembros))

% ordena decrescente
[fxi, ind] = sort(fxi);
populacao = populacao(ind,:);

end


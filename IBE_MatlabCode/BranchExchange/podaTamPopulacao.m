% poda tamanho da populacao excluindo piores individuos
function [populacao, fxi] = podaTamPopulacao(populacao,fxi,alim)

% poda populacao, mantendo o tamanho fixo
global paramAG;

% % TODO testar FIX ERRO ind = ind(1:paramAG.maxPop-1);
% sis = getSistema(alim.Fnome);
% if (sis == 6 )
%     
%     % ordena crescente
%     [fxi, ind] = sort(fxi);
%     
%     % elite
%     elite = populacao(ind(1),:);
%     
%     % escolhe tamPopulacao-1 individuos, aleatoriamente
%     ind = randperm(length(fxi))'; 
%     
%     ind = ind(1:paramAG.maxPop-1);
%     
% %     % unique
% %     [pop2, ind] = unique(populacao,'rows');
% %     fxi2 = fxi(ind);
% 
%     populacao = [elite; populacao(ind,:)];
%     fxi = [fxi(1); fxi(ind) ];
%     return;
% end

if (length(fxi) > paramAG.maxPop)
    
    populacao = populacao(1:paramAG.maxPop,:);
    fxi = fxi(1:paramAG.maxPop,1);
    
end

end
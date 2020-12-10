% poda tamanho da populacao excluindo piores individuos
function [populacao, fxi] = podaTamPopulacao(populacao,fxi,alim)

% poda populacao, mantendo o tamanho fixo
global paramAG;

% % TODO testar 
% sis = getSistema(alim.Fnome);
% if ( (sis == 6 )  ) % (sis == 4 )
%     
%     % unique
%     [pop2, ind] = unique(populacao,'rows');
%     fxi2 = fxi(ind);
% 
%     %se NAO diminuiu tamPop, mantem por anterior
%     if ( length(fxi2) > paramAG.tamPopulacao)    
%         populacao = pop2;
%         fxi = fxi2;
%     end
%     
%     % ordena crescente
%     [fxi, ind] = sort(fxi);
%     populacao = populacao(ind,:);
% 
% end

if (length(fxi) > paramAG.maxPop)
    
    populacao = populacao(1:paramAG.maxPop,:);
    fxi = fxi(1:paramAG.maxPop,1);
    
end

end
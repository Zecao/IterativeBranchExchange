% seleciona 1 individuo p/ busca local 
function [individuo, indFxi] = selecionaIndividuoBuscaLocal(populacao,fxi)

% % TODO testando
% % obtem individuo e fitness
% individuo = populacao(1,:);
% indFxi = fxi(1);
% 
% return;

global paramAG;
global param;
    
% % seleciona individuos diferentes
[popDiv, indSupP] = unique(populacao, 'rows');

if ( (size(popDiv,1) == 1) && paramAG.tamPopulacao ~= 1 ) 
    disp('Todos individuos da Populacao são Iguais!');
    
    % retorna unico individuo
    individuo = populacao(1,:);
    indFxi = fxi(1);

    % seta variavel global para abortar
    param.Abortar = 1;
    
    return;
    
end

% analisa todos os membros da populacao
for i=1:size(populacao,1)  
    
    % obtem individuo e fitness
    individuo = populacao(i,:);
    indFxi = fxi(i);
    
    % se analisou 
    if (getStatusBuscaLocalInd(individuo))
        continue;
    else
        return; % retornando o individuo
    end
        
end

% TODO
% % se chegou aqui, limpa hash de analise
% global paramAG;
% paramAG.hashIndividuosOtmBuscaLocal = containers.Map('KeyType', 'char', 'ValueType', 'logical');
% paramAG.hashIndividuosOtmBuscaLocalTamCiclo = containers.Map('KeyType', 'char', 'ValueType', 'logical');

% obtem individuo e fitness
individuo = populacao(1,:);
indFxi = fxi(1);

end
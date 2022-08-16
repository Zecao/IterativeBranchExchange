% correcao de lacos
function [lstIndividuos] = otimizaCicloIndividuo(indTS,indBinOriginal,alim)

lstIndividuos = otimizaCicloIndividuo2020(indTS,indBinOriginal,alim);

% lstIndividuos = otimizaCicloIndividuoPvp(indTS,indBinOriginal,alim);

% incrementa contador OAC
contaOAC();

end

% correcao de lacos 
% OBS: mantendo como historico, pois nao da bons resultados
function lstIndividuos = otimizaCicloIndividuoPvp(indTS,indBinOriginal,alim)

% escolhe ordem de otimizacao das chaves
indTS = defineOrdemDeCiclosOtm(indTS,indBinOriginal,alim);

% novo individuo inicialmente eh igual as Tie Switchs antigas. Ele vai
% sendo preenchido chave por chave.
novoIndividuo = indTS;

% cria lista de individuos. Cada individuo representara um passo (loop de
% for) da transicao do indTS p/ o novoIndividuo.
lstIndividuos = zeros( size(indTS,2),size(indTS,2)  );

% para cada chave, i, do individuo
for i=1:size(indTS,2); 

    % obtem a chave da condicao de menor perda p/ dada chave indTS(i)
    chaveMP = otimizacaoAberturaCiclo(indTS(i), indBinOriginal, alim);
    
    % se chave menor perda jah esta no individuo, mantem chave anterior
    if ( ismember(chaveMP, novoIndividuo) )
    
        % mantem chave anterior
        novoIndividuo(i) = indTS(i); 

    else
        
        % preenche novo individuo     
        novoIndividuo(i) = chaveMP; 

    end
    
    lstIndividuos(i,:) = novoIndividuo;
    
end

% filtra lstIndividuos
lstIndividuos = unique(lstIndividuos, 'rows');

end


function lstIndividuos = otimizaCicloIndividuo2020(indTS,indBinOriginal,alim)

% escolhe ordem de otimizacao das chaves
indTS = defineOrdemDeCiclosOtm(indTS,indBinOriginal,alim);

% vetor que guarda info se TS foi otimizada
otimizouTS(indTS) = 0;

% cria lista de individuos do tamanho do numero de TS (ciclos). 
lstIndividuos = zeros( size(indTS,2),size(indTS,2)  );

% atribui novoIndividuo TS atual 
novoInd = indTS;

% numero de TS 
for i=1:size(indTS,2)

    % se nao otimizou TS 
    if ( ~otimizouTS(indTS(i)) )

        % obtem a chave da condicao de menor perda p/ dada chave indTS(i)
        chaveMP = otimizacaoAberturaCiclo(indTS(i), indBinOriginal, alim);
    
    % se ja tiver otimizado pega info no vetor
    else
        
        chaveMP = deParaTS(indTS(i));
        
    end
    
    % se alterou a TS
    if(chaveMP ~= indTS(i))
        
        % altera indBin (para analise da proximo ciclo)
        indBinOriginal(indTS(i)) = 1;
        indBinOriginal(chaveMP) = 0;
        
        % atribui chave ao novo individuo     
        novoInd(i) = chaveMP;
    
    else
        
        % seta vetor otimizado
        otimizouTS(indTS(i)) = 1;
    
    end

    % lstIndividuos
    lstIndividuos(i,:) = novoInd;
    
end

% filtra lstIndividuos
lstIndividuos = unique(lstIndividuos, 'rows');

% % % OBS: nao faz sentido marcar o individuo como avalidado, devido a natureza estocastica
% % % da Branch Exchange em sistemas com um ciclo dentro do outro.
% sis = getSistema(alim.Fnome);
% 
% if ( ~(sis==4) || (sis==6) )  
% 
% % OBS: ('Busca Local esgotada!');
% % se todas as chavesMP == indTS(i)
% if (all(otimizouTS(indTS)))
%     
%     disp('Busca Local esgotada p/ este IND!');
% 
%     % seta status de BuscaLocal do individuo p/ true 
%     setStatusBuscaLocalInd(indBinOriginal);
%     
%     lstIndividuos = [];
%     
%     return
% 
% end
% 
% end

end

% incrementa contador OAC
function contaOAC()

global param;
param.NOAC = param.NOAC + 1;

end